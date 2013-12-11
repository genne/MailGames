using System;
using System.Collections;
using System.Collections.Generic;
using JapaneseWhist;

namespace MailGames.Context
{
    public class JapaneseWhistBoard : IGameBoard
    {
        public Guid Id { get; set; }
        public virtual Player FirstPlayer { get; set; }
        public virtual Player SecondPlayer { get; set; }
        public DateTime? LastReminded { get; set; }
        public WinnerState? WinnerState { get; set; }

        public int Seed { get; set; }

        public virtual ICollection<JapaneseWhistMove> Moves { get; set; }
    }

    public class JapaneseWhistMove
    {
        public int Id { get; set; }
        public PlayerDeck PlayerDeck { get; set; }
        public int CardIndex { get; set; }
        public CardColor? Trumf { get; set; }
        public DateTime DateTime { get; set; }
    }
}