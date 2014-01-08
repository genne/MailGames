using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;

namespace TicTacToe
{
    public static class TicTacToeLogic
    {
        public static WinnerState? GetWinner(TicTacToeState state)
        {
            var winningColor = GetWinnerColor(state);
            if (winningColor.HasValue)
                return winningColor.Value == GamePlayer.FirstPlayer
                           ? WinnerState.FirstPlayer
                           : WinnerState.SecondPlayer;
            return state.IsFull() ? (WinnerState?) WinnerState.Tie : null;
        }

        public static GamePlayer? GetWinnerColor(TicTacToeState state)
        {
            foreach (var line in AllInterrestingLines(state))
            {
                var winner = GetWinner(state, line);
                if (winner.HasValue) return winner;
            }
            return null;
        }

        public static IEnumerable<InterrestingLine> AllInterrestingLines(TicTacToeState state)
        {
            return state.GetInterrestingLines();
            //foreach(var pos in AllInterrestingPositions(state))
            //{
            //    var y = pos.Y;
            //    var x = pos.X;

            //    var height = state.Height;
            //    var width = state.Width;
            //    var length = state.NumInRow;
            //    if (y <= height - length)
            //        yield return new Line(x, y, 0, 1, length);
            //    if (x <= width - length)
            //        yield return new Line(x, y, 1, 0, length);
            //    if (y <= height - length && x <= width - length)
            //        yield return new Line(x, y, 1, 1, length);
            //    if (y >= length && x <= width - length)
            //        yield return new Line(x, y, 1, -1, length);
            //}
        }

        private static GamePlayer? GetWinner(TicTacToeState state, InterrestingLine line)
        {
            return line.Count == state.NumInRow ? line.CurrentPlayer : null;
        }

        public static GameState GetGameState(TicTacToeState state, GamePlayer loggedInPlayer)
        {
            var winner = GetWinnerColor(state);
            if (winner.HasValue)
                return winner == loggedInPlayer ? GameState.PlayerWon : GameState.OpponentWon;
            if (state.IsFull())
                return GameState.Tie;
            return state.CurrentPlayer == loggedInPlayer ? GameState.YourTurn : GameState.OpponentsTurn;
        }

        public static IEnumerable<Position> AllInterrestingPositions(TicTacToeState state)
        {
            for (int x = Math.Max(0, state.MinX - state.NumInRow); x < Math.Min(state.Width, state.MaxX + state.NumInRow); x++)
            {
                for (int y = Math.Max(0, state.MinY - state.NumInRow); y < Math.Min(state.Height, state.MaxY + state.NumInRow); y++)
                {
                    yield return new Position(x, y);
                }
            }
        }
    }
}