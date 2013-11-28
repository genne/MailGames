using System;
using System.Collections.Generic;
using System.Web.Mvc;
using GameBase;
using MailGames.Controllers;

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
            public GameType GameType { get; set; }
            public string OpponentName { get; set; }
            public Guid Id { get; set; }

            public DateTime? LastActive { get; set; }

            public GameState GameState { get; set; }
        }
    }
}