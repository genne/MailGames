using System;
using System.Linq;
using System.Web;
using MailGames.Context;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class PlayerManager
    {
        public static Player GetCurrent(MailGamesContext db)
        {
            return db.Players.Single(p => p.UserName == HttpContext.Current.User.Identity.Name);
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
    }
}