using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chess;
using GameBase;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Logic;
using MailGames.Models;
using Othello;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class OthelloController : GameControllerBase
    {
        public JsonResult GetAvailableTargets(Guid board, int selected)
        {
            var db = new MailGamesContext();
            var othelloBoard = OthelloQueries.Find(db, board);
            var availableCells = OthelloLogic.GetValidTargets(OthelloConversion.GetCurrentState(othelloBoard));
            return Json(availableCells);
        }

        public ActionResult Game(Guid id)
        {
            var db = new MailGamesContext();
            var board = OthelloQueries.Find(db, id);
            var state = OthelloConversion.GetCurrentState(board);
            return View(new GameOthelloViewModel(board)
            {
                OthelloState = state,
                ValidTargets = OthelloLogic.GetValidTargets(state)
            });
        }

        public ActionResult Select(Guid board, int cell)
        {
            var db = new MailGamesContext();
            var boardObj = OthelloQueries.Find(db, board);
            GameLogic.EnsurePlayersTurn(boardObj);
            var currentState = OthelloConversion.GetCurrentState(boardObj);
            OthelloLogic.Play(currentState, Position.FromInt(cell));
            
            boardObj.Moves.Add(new OthelloMove
            {
                Position = cell,
                DateTime = DateTime.Now
            });

            GameLogic.UpdateWinnerState(boardObj);

            SendOpponentMail(db, boardObj);

            db.SaveChanges();

            return RedirectToAction("Game", new {id = board});
        }
    }
}
