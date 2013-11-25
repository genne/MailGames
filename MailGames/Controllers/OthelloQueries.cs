using System;
using MailGames.Context;

namespace MailGames.Controllers
{
    public class OthelloQueries
    {
        public static OthelloBoard Find(MailGamesContext db, Guid board)
        {
            return db.OthelloBoards.Find(board);
        }
    }
}