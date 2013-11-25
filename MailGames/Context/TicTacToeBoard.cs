using System;
using System.Collections.Generic;
using GameBase;
using TicTacToe;

namespace MailGames.Context
{
    public class TicTacToeBoard : IGameBoard
    {
        public Guid Id { get; set; }

        public TicTacToeVariant Variant { get; set; }

        public virtual ICollection<TicTacToeMove> Moves { get; set; }

        public virtual Player FirstPlayer { get; set; }
        public virtual Player SecondPlayer { get; set; }
        public DateTime? LastReminded { get; set; }

        public WinnerState? WinnerState { get; set; }
    }
}