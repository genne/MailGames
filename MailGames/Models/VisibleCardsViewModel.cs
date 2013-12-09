using System;
using System.Collections.Generic;
using JapaneseWhist;

namespace MailGames.Models
{
    public class VisibleCardsViewModel
    {
        public Deck Deck { get; set; }

        public Func<int, string> SelectUrl { get; set; }
    }
}