using System.Collections.Generic;
using System.Linq;
using GameBase;

namespace TicTacToe
{
    public static class TicTacToeLogic
    {
        public static TicTacToeWinner GetWinner(TicTacToeState state)
        {
            var winningColor = GetWinnerColor(state);
            if (winningColor.HasValue)
                return winningColor.Value == TicTacToeColor.X
                           ? TicTacToeWinner.FirstPlayer
                           : TicTacToeWinner.SecondPlayer;
            if (state.IsFull()) return TicTacToeWinner.Tie;
            return TicTacToeWinner.None;
        }
        public static TicTacToeColor? GetWinnerColor(TicTacToeState state)
        {
            foreach (var line in AllLines())
            {
                var winner = GetWinner(state, line);
                if (winner.HasValue) return winner;
            }
            return null;
        }

        private static IEnumerable<Line> AllLines()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new Line(0, i, 1, 0);
            }
        }

        private static TicTacToeColor? GetWinner(TicTacToeState state, Line line)
        {
            var color = state.Get(line.Cells.First());
            if (!color.HasValue) return null;
            foreach (var cell in line.Cells.Skip(1))
            {
                if (state.Get(cell) != color)
                {
                    return null;
                }
            }
            return color;
        }

        public static GameState GetGameState(TicTacToeState state, TicTacToeColor loggedInPlayer)
        {
            var winner = GetWinnerColor(state);
            if (winner.HasValue)
            {
                if (winner == loggedInPlayer)
                    return GameState.PlayerWon;
                return GameState.OpponentWon;
            }
            if (state.IsFull())
                return GameState.Tie;
            if (state.CurrentPlayer == loggedInPlayer)
                return GameState.YourTurn;
            return GameState.OpponentsTurn;
        }
    }
}