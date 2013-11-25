using GameBase;
using MailGames.Context;
using TicTacToe;

namespace MailGames.Models
{
    public class GameTicTacToeViewModel : GameViewModel
    {
        public GameTicTacToeViewModel(IGameBoard board) : base(board)
        {
        }

        public TicTacToeColor?[,] Colors { get; set; }

        public Position LastMove { get; set; }
    }
}