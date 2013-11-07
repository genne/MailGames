using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Models;
using TicTacToe;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TicTacToeController : GameControllerBase
    {
        public ActionResult StartGame(string opponentsmail)
        {
            var db = new MailGamesContext();
            var board = db.TicTacToeBoards.Create();
            board.Id = Guid.NewGuid();
            string yourmail = PlayerManager.GetCurrent(db).Mail;
            board.FirstPlayer = PlayerManager.FindOrCreatePlayer(yourmail, db);
            board.SecondPlayer = PlayerManager.FindOrCreatePlayer(opponentsmail, db);
            db.TicTacToeBoards.Add(board);
            db.SaveChanges();

            return RedirectToAction("Game", new { id = board.Id });
        }

        public ActionResult Game(Guid id)
        {
            var colors = new TicTacToeColor?[3,3];
            var board = new MailGamesContext().TicTacToeBoards.Find(id);
            var state = TicTacToeConversion.GetState(board);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    colors[x, y] = state.Get(new Position(x, y));
                }
            }
            var loggedInPlayer = board.FirstPlayer.Id == WebSecurity.CurrentUserId ? TicTacToeColor.X : TicTacToeColor.O;
            var model = new GameTicTacToeViewModel
            {
                Id = id, 
                Colors = colors,
                State = TicTacToeLogic.GetGameState(state, loggedInPlayer),
                GameType = GameType.TicTacToe
            };
            return View(model);
        }

        public ActionResult Select(Guid id, int x, int y)
        {
            var db = new MailGamesContext();
            var board = db.TicTacToeBoards.Find(id);
            var state = TicTacToeConversion.GetState(board);
            ValidateMove(board, x, y, state);
            board.Moves.Add(new TicTacToeMove
            {
                X = x, 
                Y = y,
                DateTime = DateTime.Now
            });
            board.Winner = TicTacToeLogic.GetWinner(state);
            db.SaveChanges();

            if (board.Winner == TicTacToeWinner.FirstPlayer || board.Winner == TicTacToeWinner.SecondPlayer)
                SendMail(db, board, "You lost the game", "Game lost :(");
            else if (board.Winner == TicTacToeWinner.Tie)
                SendMail(db, board, "It was a tie...", "Tie");
            else
                SendMail(db, board, "It's your turn to make a move!", "Your turn");

            return RedirectToAction("Game", new {id});
        }

        private void ValidateMove(TicTacToeBoard board, int x, int y, TicTacToeState state)
        {
            if (GetDbPlayer(state.CurrentPlayer, board).Id != WebSecurity.CurrentUserId) throw new InvalidOperationException("Not your turn");
            state.Play(x, y); // Will validate
        }

        private static Player GetDbPlayer(TicTacToeColor currentPlayer, TicTacToeBoard board)
        {
            return currentPlayer == TicTacToeColor.X ? board.FirstPlayer : board.SecondPlayer;
        }

        public ActionResult Rematch(Guid id)
        {
            var board = new MailGamesContext().TicTacToeBoards.Find(id);
            string opponentsMail = board.FirstPlayer.Id == WebSecurity.CurrentUserId ? board.SecondPlayer.Mail : board.FirstPlayer.Mail;
            return RedirectToAction("StartGame", new {opponentsMail});
        }
    }
}
