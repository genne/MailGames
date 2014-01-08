using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;

namespace Othello
{
    public static class OthelloLogic
    {
        public const int Size = 8;

        public static void Play(OthelloState state, Position position)
        {
            if (state.Get(position) != null) throw new ArgumentException("Position not empty", "position");
            var takableDirections = GetTakableDirections(state, position).ToArray();
            if (!takableDirections.Any()) throw new ArgumentException("No pieces taken", "position");
            foreach(var dir in takableDirections)
            {
                TakeRow(state, position, dir);
            }
            state.Set(position);
            var nextPlayer = GameBaseLogic.GetNextPlayer(state.CurrentPlayer);
            if (GetValidTargets(state, nextPlayer).Any())
            {
                state.CurrentPlayer = nextPlayer;    
            }
        }

        private static void TakeRow(OthelloState state, Position fromPos, Position dir)
        {
            var otherPlayer = GameBaseLogic.GetNextPlayer(state.CurrentPlayer);
            var pos = fromPos;
            while (true)
            {
                pos = pos.Add(dir);
                if (state.Get(pos) == otherPlayer)
                    state.Set(pos);
                else
                {
                    break;
                }
            }
        }

        private static IEnumerable<Position> GetTakableDirections(OthelloState state, Position position, GamePlayer? player = null)
        {
            var originalPlayer = player ?? state.CurrentPlayer;
            var otherPlayer = GameBaseLogic.GetNextPlayer(originalPlayer);
            foreach (var dir in Position.AllDirections())
            {
                var pos = position;
                GamePlayer? currentPlayer;
                var pieceTaken = false;
                while (true)
                {
                    pos = pos.Add(dir);
                    currentPlayer = state.Get(pos);
                    if (currentPlayer != otherPlayer)
                        break;
                    pieceTaken = true;
                }
                if (currentPlayer == originalPlayer && pieceTaken)
                    yield return dir;
            }
        }

        public static IEnumerable<Position> GetValidTargets(OthelloState state, GamePlayer? player = null)
        {
            var allPositions = GetAllPositions();
            var emptyPositions = allPositions.Where(p => state.Get(p) == null);
            return emptyPositions.Where(pos => GetTakableDirections(state, pos, player).Any());
        }

        public static IEnumerable<Position> GetAllPositions()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    yield return new Position(x, y);
                }
            }
        }

        public static WinnerState? GetWinner(OthelloState state)
        {
            var count = new int[2];
            if (GetValidTargets(state).Any()) return null;
            foreach (var player in GetAllPositions().Select(state.Get))
            {
                //if (player == null)
                //    return null;
                if (player != null)
                    count[(int) player]++;
            }
            if (count[0] == count[1]) return WinnerState.Tie;
            return count[0] > count[1] ? WinnerState.FirstPlayer : WinnerState.SecondPlayer;
        }
    }
}