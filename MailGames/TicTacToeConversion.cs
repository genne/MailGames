using MailGames.Context;
using TicTacToe;

namespace MailGames
{
    internal class TicTacToeConversion
    {
        public static TicTacToeState GetState(TicTacToeBoard board)
        {
            var state = new TicTacToeState();
            foreach (var move in board.Moves)
            {
                state.Play(move.X, move.Y);
            }
            return state;
        }
    }
}