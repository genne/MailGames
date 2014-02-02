using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using GameBase;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Logic;
using MailGames.Models;
using StackExchange.Profiling;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TicTacToeController : GameControllerBase
    {
        public ActionResult Game(Guid id)
        {
            return View(GetModel(id));
        }

        private static GameTicTacToeViewModel GetModel(Guid id)
        {
            var board = new MailGamesContext().TicTacToeBoards.Find(id);
            var state = TicTacToeConversion.GetState(board);
            var colors = new GamePlayer?[state.Width,state.Height];
            for (int x = 0; x < state.Width; x++)
            {
                for (int y = 0; y < state.Height; y++)
                {
                    colors[x, y] = state.Get(new Position(x, y));
                }
            }
            var model = new GameTicTacToeViewModel(board)
            {
                Colors = colors,
                LastMove = ToPosition(board.Moves.LastOrDefault())
            };
            return model;
        }

        private static Position ToPosition(TicTacToeMove move)
        {
            return move == null ? null : new Position(move.X, move.Y);
        }

        public ActionResult Select(Guid id, int x, int y)
        {
            var db = new MailGamesContext();
            var board = db.TicTacToeBoards.Find(id);
            GameLogic.EnsurePlayersTurn(board);
            var state = TicTacToeConversion.GetState(board);
            state.Play(x, y);
            board.Moves.Add(new TicTacToeMove
            {
                X = x,
                Y = y,
                DateTime = DateTime.Now
            });

            SendOpponentMail(db, board);

            db.SaveChanges();

            return Json(GetModel(id));
        }
    }
}
