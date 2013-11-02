using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Models;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var games = new MailGamesContext().ChessBoards.Select(b => new
                {
                    Board = b,
                    Opponent = (b.WhitePlayer.Id == WebSecurity.CurrentUserId ? b.BlackPlayer : b.WhitePlayer)
                }).Select(b => new
            {
                Finished = b.Board.Winner.HasValue,
                YourTurn = b.Board.ChessMoves.Count()%2 == (b.Board.WhitePlayer.Id == WebSecurity.CurrentUserId ? 0 : 1),
                Game = new IndexHomeViewModel.Game
                {
                    Id = b.Board.Id,
                    OpponentName = b.Opponent.UserName ?? b.Opponent.Mail
                }
            });
            return View(new IndexHomeViewModel
            {
                UserName = WebSecurity.CurrentUserName,
                YourTurnGames = games.Where(g => !g.Finished && g.YourTurn).Select(g => g.Game),
                OpponentTurnGames = games.Where(g => !g.Finished && !g.YourTurn).Select(g => g.Game),
                FinishedGames = games.Where(g => g.Finished).Select(g => g.Game)
            });
        }
    }
}
