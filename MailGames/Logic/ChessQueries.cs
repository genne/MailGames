using System;
using System.Linq;
using MailGames.Context;
using WebMatrix.WebData;

namespace MailGames.Logic
{
    public class ChessQueries
    {
        public static IQueryable<ChessBoard> Boards(MailGamesContext db)
        {
            return
                db.ChessBoards.Where(
                    b => b.FirstPlayer.Id == WebSecurity.CurrentUserId || b.SecondPlayer.Id == WebSecurity.CurrentUserId);
        }

        public static ChessBoard Find(MailGamesContext db, Guid board)
        {
            return Boards(db).Single(b => b.Id == board);
        }
    }
}