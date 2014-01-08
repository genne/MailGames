using GameBase;
using MailGames.Context;

namespace MailGames.Models
{
    public class GameTicTacToeViewModel : GameViewModel
    {
        public GameTicTacToeViewModel(IGameBoard board) : base(board)
        {
        }

        public GamePlayer?[,] Colors { get; set; }

        public Position LastMove { get; set; }
    }
}