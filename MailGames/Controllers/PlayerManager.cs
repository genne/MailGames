using System;
using System.Linq;
using System.Web;
using MailGames.Context;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class PlayerManager
    {
        public static Player GetCurrent(MailGamesContext db)
        {
            return db.Players.Single(p => p.UserName == HttpContext.Current.User.Identity.Name);
        }

        public static Player FindOrCreatePlayer(MailGamesContext db, string mail)
        {
            return db.Players.FirstOrDefault(p => p.Mail == mail) ?? CreatePlayer(db, mail);
        }

        public static Player CreatePlayer(MailGamesContext db, string mail)
        {
            var player = db.Players.Create();
            player.Mail = mail;
            player.UserName = mail;
            player.Guid = Guid.NewGuid();
            db.Players.Add(player);
            return player;
        }

        public static Player CreatePlayer(string mail)
        {
            var db = new MailGamesContext();
            var player = CreatePlayer(db, mail);
            db.SaveChanges();
            return player;
        }

        public static string GetOpponentName(IGameBoard board)
        {
            var opponentPlayer = GetOpponentPlayer(board);
            return GetPlayerName(opponentPlayer);
        }

        private static Player GetOpponentPlayer(IGameBoard board)
        {
            var opponentPlayer = board.FirstPlayer.Id == WebSecurity.CurrentUserId
                                     ? board.SecondPlayer
                                     : board.FirstPlayer;
            return opponentPlayer;
        }

        public static int GetOpponentId(IGameBoard board)
        {
            return GetOpponentPlayer(board).Id;
        }

        public static string GetPlayerName(Player player)
        {
            return player.FullName ?? player.UserName ?? player.Mail;
        }

        public static Player FindOrCreateFBPlayer(MailGamesContext db, long friendId)
        {
            Player player;
            var userName = OAuthWebSecurity.GetUserName("facebook", friendId.ToString());
            if (userName == null)
            {
                player = db.Players.Create();
                player.Guid = Guid.NewGuid();
                player.UserName = friendId + "_temporaryFacebook";
                db.Players.Add(player);
                db.SaveChanges();
                OAuthWebSecurity.CreateOrUpdateAccount("facebook", friendId.ToString(), player.UserName);
            }
            else
            {
                player = db.Players.First(p => p.UserName == userName);
            }
            return player;
        }
    }
}