using System;
using System.Collections.Generic;
using MailGames.Models;

namespace MailGames.Context
{
    public class Player
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Mail { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<PlayerGameRanking> Rankings { get; set; }
    }

    public class PlayerGameRanking
    {
        public int id { get; set; }
        public GameType GameType { get; set; }
        public float Ranking { get; set; }
    }
}