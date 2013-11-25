using System.Collections.Generic;
using System.Linq;
using GameBase;
using MailGames.Context;

namespace TicTacToe
{
    public static class TicTacToeLogic
    {
        public static WinnerState? GetWinner(TicTacToeState state)
        {
            var winningColor = GetWinnerColor(state);
            if (winningColor.HasValue)
                return winningColor.Value == TicTacToeColor.X
                           ? WinnerState.FirstPlayer
                           : WinnerState.SecondPlayer;
            return state.IsFull() ? (WinnerState?) WinnerState.Tie : null;
        }
        public static TicTacToeColor? GetWinnerColor(TicTacToeState state)
        {
            foreach (var line in AllLines(state.Width, state.Height, state.NumInRow))
            {
                var winner = GetWinner(state, line);
                if (winner.HasValue) return winner;
            }
            return null;
        }

        private static IEnumerable<Line> AllLines(int width, int height, int length)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y <= height - length)
                        yield return new Line(x, y, 0, 1, length);
                    if (x <= width - length)
                        yield return new Line(x, y, 1, 0, length);
                    if (y <= height - length && x <= width - length)
                        yield return new Line(x, y, 1, 1, length);
                    if (y >= length && x <= width - length)
                        yield return new Line(x, y, 1, -1, length);
                }
            }
        }

        private static TicTacToeColor? GetWinner(TicTacToeState state, Line line)
        {
            var color = state.Get(line.Cells.First());
            if (!color.HasValue) return null;
            return line.Cells.Skip(1).Any(cell => state.Get(cell) != color) ? null : color;
        }

        public static GameState GetGameState(TicTacToeState state, TicTacToeColor loggedInPlayer)
        {
            var winner = GetWinnerColor(state);
            if (winner.HasValue)
                return winner == loggedInPlayer ? GameState.PlayerWon : GameState.OpponentWon;
            if (state.IsFull())
                return GameState.Tie;
            return state.CurrentPlayer == loggedInPlayer ? GameState.YourTurn : GameState.OpponentsTurn;
        }
    }
}