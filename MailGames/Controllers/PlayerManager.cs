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

        public static string GetOpponentName(IGameBoard board)
        {
            var opponentPlayer = GetOpponentPlayer(board);
            return opponentPlayer.UserName ?? opponentPlayer.Mail;
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
    }
}