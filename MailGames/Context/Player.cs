using System;

namespace MailGames.Context
{
    public class Player
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Mail { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}