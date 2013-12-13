using System.Collections;
using System.Collections.Generic;

namespace MailGames.Models
{
    public class HomeGameRankingsViewModel
    {
        public GameType GameType { get; set; }

        public IEnumerable<TopUser> TopUsers { get; set; }

        public class TopUser
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public float Ranking { get; set; }
        }
    }
}