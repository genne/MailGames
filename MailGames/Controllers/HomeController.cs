using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using GameBase;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Logic;
using MailGames.Models;
using TicTacToe;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class HomeController : GameControllerBase
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var db = new MailGamesContext();
            var allBoards = GameLogic.GetAllBoards(db);
            var games = allBoards.Where(b => b.FirstPlayer.Id == WebSecurity.CurrentUserId || b.SecondPlayer.Id == WebSecurity.CurrentUserId).Select(b => new
            {
                Board = b
            }).Select(b => new
            {
                Finished = b.Board.WinnerState.HasValue,
                YourTurn = GameLogic.GetCurrentPlayer(b.Board) == GameLogic.GetLoggedInPlayer(b.Board),
                Game = new IndexHomeViewModel.Game
                {
                    GameType = GameLogic.GetGameType(b.Board),
                    Id = b.Board.Id,
                    OpponentName = PlayerManager.GetOpponentName(b.Board),
                    LastActive = GameLogic.GetLastActive(b.Board),
                    GameState = GameLogic.GetGameState(b.Board)
                }
            }).ToArray();
            return View(new IndexHomeViewModel
            {
                UserName = PlayerManager.GetPlayerName(PlayerManager.GetCurrent(db)),
                YourTurnGames = games.Where(g => !g.Finished && g.YourTurn).Select(g => g.Game).OrderBy(g => g.LastActive),
                OpponentTurnGames = games.Where(g => !g.Finished && !g.YourTurn).Select(g => g.Game).OrderBy(g => g.LastActive),
                FinishedGames = games.Where(g => g.Finished).Select(g => g.Game).OrderByDescending(g => g.LastActive)
            });
        }

        public void UpdateStates()
        {
            var db = new MailGamesContext();
            foreach (var board in GameLogic.GetAllBoards(db).Where(board => board.WinnerState == null))
            {
                board.WinnerState = GameLogic.GetWinnerState(board);
            }
            db.SaveChanges();
        }

        public ActionResult StartGame()
        {
            var model = new StartGameHomeViewModel();
            model.Friends = FacebookApi.Friends();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult StartGame(string opponentsmail, GameType gameType)
        {
            var db = new MailGamesContext();
            var board = GameLogic.CreateGameBoard(gameType, db);
            board.Id = Guid.NewGuid();
            string yourmail = PlayerManager.GetCurrent(db).Mail;
            board.FirstPlayer = PlayerManager.FindOrCreatePlayer(yourmail, db);
            board.SecondPlayer = PlayerManager.FindOrCreatePlayer(opponentsmail, db);
            db.SaveChanges();
            string controller = GameLogic.GetController(board);
            return RedirectToAction("Game", controller, new { id = board.Id });
        }

        public ActionResult Rematch(Guid id, GameType gameType)
        {
            var board = GameLogic.GetBoard(new MailGamesContext(), id, gameType);
            string opponentsMail = board.FirstPlayer.Id == WebSecurity.CurrentUserId ? board.SecondPlayer.Mail : board.FirstPlayer.Mail;
            return RedirectToAction("StartGame", new { opponentsMail, gameType });
        }

        public ActionResult RemindOpponent(Guid id, GameType gametype)
        {
            var db = new MailGamesContext();
            var board = GameLogic.GetBoard(db, id, gametype);
            var currentPlayer = GameLogic.GetCurrentPlayer(board);
            if (board.LastReminded.HasValue)
            {
                board.WinnerState = currentPlayer == GamePlayer.FirstPlayer
                                        ? WinnerState.FirstPlayerPassive
                                        : WinnerState.SecondPlayerPassive;
                SendOpponentMail(db, board, "You lost the game because of inactivity", "Game lost :(");
            }
            else
            {
                SendOpponentMail(db, board, "I'm waiting for you to make a move...", "Still your turn!");
                board.LastReminded = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Game", gametype.ToString(), new{ id });
        }

        public ActionResult Surrender(Guid id, GameType gameType)
        {
            var db = new MailGamesContext();
            var board = GameLogic.GetBoard(db, id, gameType);
            var currentPlayer = GameLogic.EnsurePlayersTurn(board);
            board.WinnerState = currentPlayer == GamePlayer.FirstPlayer ? WinnerState.FirstPlayerResigned : WinnerState.SecondPlayerResigned;
            db.SaveChanges();

            SendOpponentMail(db, board, "You won the game, congratulations!", "Opponent resigned");

            return RedirectToAction("Game", gameType.ToString(), new { id });
        }

        public ActionResult User(int id)
        {
            var model = new MailGamesContext().Players.Where(p => p.Id == id).Select(p => new UserHomeViewModel
            {
                Guid = p.Guid,
                Name = PlayerManager.GetPlayerName(p)
            }).Single();
            return View(model);
        }
    }

    public class TicTacToeQueries
    {
        public static IQueryable<TicTacToeBoard> Boards(MailGamesContext db)
        {
            return
                db.TicTacToeBoards.Where(
                    b => b.FirstPlayer.Id == WebSecurity.CurrentUserId || b.SecondPlayer.Id == WebSecurity.CurrentUserId);
        }
    }
}
