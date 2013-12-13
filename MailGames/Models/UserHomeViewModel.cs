using System;
using System.Collections;
using System.Collections.Generic;

namespace MailGames.Models
{
    public class UserHomeViewModel
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }

        public IEnumerable<GameRanking> GameRankings { get; set; }

        public class GameRanking
        {
            public GameType GameType { get; set; }

            public float Ranking { get; set; }

            public int NumWon { get; set; }
            public int NumLost { get; set; }
            public int NumTie { get; set; }
        }
    }
}