using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JapaneseWhist;
using MailGames.Context;

namespace MailGames.Models
{
    public class JapaneseWhistGameViewModel : GameViewModel
    {
        public JapaneseWhistGameViewModel(IGameBoard board) : base(board)
        {
        }

        public Deck OpponentHandCards { get; set; }
        public Deck OpponentHiddenCards { get; set; }
        public Deck OpponentVisibleCards { get; set; }

        public Deck PlayerHandCards { get; set; }
        public Deck PlayerHiddenCards { get; set; }
        public Deck PlayerVisibleCards { get; set; }

        public bool CanPlayFromHand { get; set; }

        public IEnumerable<int> SelectableHandCards { get; set; }

        public IEnumerable<int> SelectableVisibleCards { get; set; }

        public Deck CurrentStick { get; set; }

        public Deck PlayerSticks { get; set; }
        public Deck OpponentSticks { get; set; }

        public int PlayerTotalScore { get; set; }
        public int OpponentTotalScore { get; set; }

        public Deck PlayerLastStick { get; set; }
        public Deck OpponentLastStick { get; set; }
    }
}