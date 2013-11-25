using System;
using System.Collections.Generic;

namespace MailGames.Context
{
    public class OthelloBoard : IGameBoard
    {
        public Guid Id { get; set; }
        public virtual Player FirstPlayer { get; set; }
        public virtual Player SecondPlayer { get; set; }
        public DateTime? LastReminded { get; set; }
        public WinnerState? WinnerState { get; set; }

        public virtual ICollection<OthelloMove> Moves { get; set; }
    }

    public class OthelloMove
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public DateTime DateTime { get; set; }
    }
}