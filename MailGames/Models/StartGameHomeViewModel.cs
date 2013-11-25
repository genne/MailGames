using System.Collections;
using System.Collections.Generic;

namespace MailGames.Models
{
    public class StartGameHomeViewModel
    {
        public IEnumerable<Friend> Friends { get; set; }

        public class Friend
        {
            public string Id { get; set; }
            public string Picture { get; set; }
            public string Name { get; set; }
        }
    }
}