using System;
using System.Linq;
using MailGames.Context;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class PlayerManager
    {
        public static Player GetCurrent(MailGamesContext db)
        {
            return db.Players.Single(p => p.Id == WebSecurity.CurrentUserId);
        }

        public static Player FindOrCreatePlayer(string yourmail, MailGamesContext db)
        {
            var player = db.Players.FirstOrDefault(p => p.Mail == yourmail);
            if (player == null)
            {
                player = db.Players.Create();
                player.Mail = yourmail;
                player.Guid = Guid.NewGuid();
                db.Players.Add(player);
            }
            return player;
        }
    }
}