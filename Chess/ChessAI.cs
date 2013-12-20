using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase;

namespace Chess
{
    public static class ChessAI
    {
        public class Move
        {
            public int From { get; set; }
            public int To { get; set; }
            public PieceType? ConvertPawnTo { get; set; }

            public override string ToString()
            {
                return Position.FromInt(From) + " -> " + Position.FromInt(To);
            }
        }
        public static Move GetMove(ChessState state, bool checkOpponent = true)
        {
            var allMoves = GetAllMoves(state);
            var orderByPoints = allMoves.Select(m => new { Move = m, Points = GetPoints(state, m, checkOpponent) }).OrderByDescending(m => m.Points).ToArray();
            return orderByPoints.First().Move;
        }

        private static float GetPoints(ChessState state, Move move, bool checkOpponent)
        {
            var newState = new ChessState(state);
            ChessLogic.ApplyMove(newState, move.From, move.To, move.ConvertPawnTo);

            if (checkOpponent)
            {
                var opponentMove = GetMove(newState, false);
                ChessLogic.ApplyMove(newState, opponentMove.From, opponentMove.To, opponentMove.ConvertPawnTo);
            }

            return ChessLogic.GetPoints(newState, state.CurrentPlayer) - ChessLogic.GetPoints(newState, GameBaseLogic.GetNextPlayer(state.CurrentPlayer));
        }

        private static IEnumerable<Move> GetAllMoves(ChessState state)
        {
            return
                GetPiecePositions(state).SelectMany(
                    from =>
                    ChessLogic.GetAvailableTargets(state, from.Key).Select(to => new Move
                    {
                        From = from.Key, To = to,
                        ConvertPawnTo = IsPawnConversion(@from, to) ? PieceType.Queen : (PieceType?) null
                    })).ToArray();
        }

        private static bool IsPawnConversion(KeyValuePair<int, Piece> @from, int to)
        {
            return @from.Value.PieceType == PieceType.Pawn && (Position.FromInt(to).Y == 0 || Position.FromInt(to).Y == 7);
        }

        private static IEnumerable<KeyValuePair<int, Piece>> GetPiecePositions(ChessState state)
        {
            return state.GetCells().Where(c => c.Value.GamePlayer == state.CurrentPlayer);
        }
    }
}
