using System.Collections.Generic;
using GameBase;
using MailGames.Context;
using Othello;

namespace MailGames.Models
{
    public class GameOthelloViewModel : GameViewModel
    {
        public GameOthelloViewModel(IGameBoard board) : base(board)
        {
        }

        public OthelloState OthelloState { get; set; }

        public IEnumerable<Position> ValidTargets { get; set; }
    }
}