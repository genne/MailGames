using System;
using System.Collections.Generic;
using System.Linq;
using MailGames.Context;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace MailGames.Logic
{
    public class PlayerManager
    {
        public static Player GetCurrent(MailGamesContext db)
        {
            return db.Players.Single(p => p.UserName == WebSecurity.CurrentUserName);
        }

        public static IEnumerable<IGameBoard> FilterPlayerBoards(IEnumerable<IGameBoard> allBoards)
        {
            return allBoards.Where(b => (b.FirstPlayer != null && b.FirstPlayer.Id == WebSecurity.CurrentUserId) || (b.SecondPlayer != null && b.SecondPlayer.Id == WebSecurity.CurrentUserId));
        }

        public static Player FindOrCreatePlayer(MailGamesContext db, string mail)
        {
            if (mail == null) throw new ArgumentNullException("mail");
            if (mail == "") throw new ArgumentException("mail cannot be empty string", "mail");
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
            var opponentPlayer = GetOpponent(board);
            return GetPlayerName(opponentPlayer);
        }

        public static Player GetOpponent(IGameBoard board)
        {
            var opponentPlayer = board.FirstPlayer != null && board.FirstPlayer.Id == WebSecurity.CurrentUserId
                                     ? board.SecondPlayer
                                     : board.FirstPlayer;
            return opponentPlayer;
        }

        public static int? GetOpponentId(IGameBoard board)
        {
            var opponent = GetOpponent(board);
            return opponent!= null ? opponent.Id : (int?) null;
        }

        public static string GetPlayerName(Player player)
        {
            if (player == null) return "Computer";
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

        public static int GetNumWaitingGames()
        {
            if (!WebSecurity.IsAuthenticated) return 0;
            var gameBoards = FilterPlayerBoards(GameLogic.GetAllBoards(new MailGamesContext())).Where(b => b.WinnerState == null && GameLogic.GetCurrentPlayer(b) == GameLogic.GetLoggedInPlayer(b));
            return gameBoards.Count();
        }
    }
}