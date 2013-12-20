using System.Collections;
using System.Collections.Generic;

namespace MailGames.Models
{
    public class StartGameHomeViewModel
    {
        public IEnumerable<Friend> PlayedOpponents { get; set; }

        public class Friend
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public FriendType FriendType { get; set; }
        }

        public enum FriendType
        {
            Opponent,
            Facebook
        }
    }
}