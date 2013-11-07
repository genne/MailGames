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
    [InitializeSimpleMembership]
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var db = new MailGamesContext();
            var chessGames = db.ChessBoards.Select(b => new
            {
                Board = b,
                Opponent = (b.FirstPlayer.Id == WebSecurity.CurrentUserId ? b.SecondPlayer : b.FirstPlayer)
            }).Select(b => new
            {
                Finished = b.Board.Winner.HasValue,
                YourTurn = b.Board.ChessMoves.Count()%2 == (b.Board.FirstPlayer.Id == WebSecurity.CurrentUserId ? 0 : 1),
                Game = new IndexHomeViewModel.Game
                {
                    GameType = GameType.Chess,
                    Id = b.Board.Id,
                    OpponentName = b.Opponent.UserName ?? b.Opponent.Mail,
                    LastActive = b.Board.ChessMoves.OrderByDescending(m => m.Id).FirstOrDefault().DateTime
                }
            });
            var tttGames = db.TicTacToeBoards.Select(b => new
            {
                Board = b,
                Opponent = (b.FirstPlayer.Id == WebSecurity.CurrentUserId ? b.SecondPlayer : b.FirstPlayer)
            }).Select(b => new
            {
                Finished = b.Board.Winner != TicTacToeWinner.None,
                YourTurn = b.Board.Moves.Count() % 2 == (b.Board.FirstPlayer.Id == WebSecurity.CurrentUserId ? 0 : 1),
                Game = new IndexHomeViewModel.Game
                {
                    GameType = GameType.TicTacToe,
                    Id = b.Board.Id,
                    OpponentName = b.Opponent.UserName ?? b.Opponent.Mail,
                    LastActive = b.Board.Moves.OrderByDescending(m => m.Id).FirstOrDefault().DateTime
                }
            });
            var games = chessGames.Concat(tttGames).OrderBy(g => g.Game.LastActive);
            return View(new IndexHomeViewModel
            {
                UserName = WebSecurity.CurrentUserName,
                YourTurnGames = games.Where(g => !g.Finished && g.YourTurn).Select(g => g.Game),
                OpponentTurnGames = games.Where(g => !g.Finished && !g.YourTurn).Select(g => g.Game),
                FinishedGames = games.Where(g => g.Finished).Select(g => g.Game)
            });
        }

        public ActionResult StartGame()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult StartGame(string opponentsmail, GameType gameType)
        {
            var db = new MailGamesContext();
            var board = CreateGameBoard(gameType, db);
            board.Id = Guid.NewGuid();
            string yourmail = PlayerManager.GetCurrent(db).Mail;
            board.FirstPlayer = PlayerManager.FindOrCreatePlayer(yourmail, db);
            board.SecondPlayer = PlayerManager.FindOrCreatePlayer(opponentsmail, db);
            db.SaveChanges();
            string controller = gameType.ToString();
            return RedirectToAction("Game", controller, new { id = board.Id });
        }

        private IGameBoard CreateGameBoard(GameType gameType, MailGamesContext db)
        {
            IGameBoard gameBoard;
            switch (gameType)
            {
                case GameType.Chess:
                    gameBoard = db.ChessBoards.Create();
                    db.ChessBoards.Add((ChessBoard)gameBoard);
                    break;
                case GameType.TicTacToe:
                    gameBoard = db.TicTacToeBoards.Create();
                    db.TicTacToeBoards.Add((TicTacToeBoard)gameBoard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gameType");
            }
            return gameBoard;
        }
    }
}
