using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MailGames.Models
{
    public class IndexHomeViewModel
    {
        public string UserName { get; set; }

        public IEnumerable<Game> YourTurnGames { get; set; }
        public IEnumerable<Game> OpponentTurnGames { get; set; }
        public IEnumerable<Game> FinishedGames { get; set; }

        public class Game
        {
            public string OpponentName { get; set; }
            public Guid Id { get; set; }
        }
    }
}