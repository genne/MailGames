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
            var games = FilterPlayerBoards(allBoards).Select(b => new
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
            var player = PlayerManager.GetCurrent(db);
            return View(new IndexHomeViewModel
            {
                UserName = PlayerManager.GetPlayerName(player),
                YourTurnGames = games.Where(g => !g.Finished && g.YourTurn).Select(g => g.Game).OrderBy(g => g.LastActive),
                OpponentTurnGames = games.Where(g => !g.Finished && !g.YourTurn).Select(g => g.Game).OrderBy(g => g.LastActive),
                FinishedGames = games.Where(g => g.Finished).Select(g => g.Game).OrderByDescending(g => g.LastActive),
                UserId = player.Id
            });
        }

        private static IEnumerable<IGameBoard> FilterPlayerBoards(IEnumerable<IGameBoard> allBoards)
        {
            return allBoards.Where(b => b.FirstPlayer.Id == WebSecurity.CurrentUserId || b.SecondPlayer.Id == WebSecurity.CurrentUserId);
        }

        public void UpdateRankings()
        {
            var db = new MailGamesContext();
            db.Database.ExecuteSqlCommand("delete from playergamerankings");
            foreach (var board in GameLogic.GetAllBoards(db))
            {
                GameLogic.UpdateRankings(board);
            }
            db.SaveChanges();
        }

        public void UpdateStates()
        {
            var db = new MailGamesContext();
            foreach (var board in GameLogic.GetAllBoards(db).Where(board => board.WinnerState == null))
            {
                GameLogic.UpdateWinnerState(board);
            }
            db.SaveChanges();
        }

        public ActionResult StartGame()
        {
            var model = new StartGameHomeViewModel();
            model.PlayedOpponents =
                FilterPlayerBoards(GameLogic.GetAllBoards(new MailGamesContext()))
                    .SelectMany(b => new[] {b.FirstPlayer, b.SecondPlayer})
                    .Where(p => p.FullName != null && p.Id != WebSecurity.CurrentUserId)
                    .Distinct().Select(p => new StartGameHomeViewModel.Friend
                    {
                        Id = p.Id,
                        Name = p.FullName,
                        FriendType = StartGameHomeViewModel.FriendType.Opponent
                    }).ToArray();
            var facebookFriends = FacebookApi.Friends();
            if (facebookFriends != null)
            {
                model.PlayedOpponents =
                    model.PlayedOpponents.Concat(facebookFriends.Select(f => new StartGameHomeViewModel.Friend
                    {
                        Id = long.Parse(f.Id),
                        Name = f.Name,
                        FriendType = StartGameHomeViewModel.FriendType.Facebook
                    }).OrderBy(f => f.Name));
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult StartGame(string opponentsmail, string playedOpponent, GameType gameType)
        {
            StartGameHomeViewModel.FriendType friendType;
            long friendId;
            Player opponent;
            var db = new MailGamesContext();
            string name;
            if (ParseFriendId(playedOpponent, out friendType, out friendId, out name))
            {
                if (friendType == StartGameHomeViewModel.FriendType.Facebook)
                {
                    opponent = PlayerManager.FindOrCreateFBPlayer(db, friendId);
                    opponent.FullName = name;
                }
                else
                {
                    opponent = db.Players.Find(friendId);
                }
            }
            else
            {
                opponent = PlayerManager.FindOrCreatePlayer(db, opponentsmail);
            }

            var board = GameLogic.CreateGameBoard(gameType, db);
            board.Id = Guid.NewGuid();
            board.FirstPlayer = PlayerManager.GetCurrent(db);
            board.SecondPlayer = opponent;
            if (new Random().Next(2) == 1)
            {
                var firstPlayer = board.FirstPlayer;
                var secondPlayer = board.SecondPlayer;
                Utils.Swap(ref firstPlayer, ref secondPlayer);
                board.FirstPlayer = firstPlayer;
                board.SecondPlayer = secondPlayer;

                SendOpponentMail(db, board);
            }
            db.SaveChanges();
            return RedirectToAction("Game", GameLogic.GetController(board), new { id = board.Id });
        }

        private bool ParseFriendId(string playedOpponent, out StartGameHomeViewModel.FriendType friendType, out long friendId, out string name)
        {
            var split = playedOpponent.Split('_');
            if (split.Length != 2)
            {
                friendType = StartGameHomeViewModel.FriendType.Opponent;
                friendId = 0;
                name = null;
                return false;
            }
            friendId = long.Parse(split[0]);
            var split2 = split[1].Split('|');
            friendType = (StartGameHomeViewModel.FriendType)Enum.Parse(typeof(StartGameHomeViewModel.FriendType), split2[0]);
            name = split2[1];
            return true;
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
            var activity = GameLogic.GetActivity(board);
            switch (activity)
            {
                case Activity.PassiveLostGame:
                {
                    var currentPlayer = GameLogic.GetCurrentPlayer(board);
                    board.WinnerState = currentPlayer == GamePlayer.FirstPlayer
                                            ? WinnerState.FirstPlayerPassive
                                            : WinnerState.SecondPlayerPassive;

                    GameLogic.UpdateRankings(board);

                    SendOpponentMail(db, board, "You lost the game because of inactivity", "Game lost :(");
                }
                    break;
                case Activity.GameNeverStarted:
                    GameLogic.RemoveBoard(db, board);
                    break;
                default:
                    SendOpponentMail(db, board, "I'm waiting for you to make a move...", "Still your turn!");
                    board.LastReminded = DateTime.Now;
                    break;
            }
            db.SaveChanges();

            return activity == Activity.GameNeverStarted 
                ? RedirectToAction("Index")
                : RedirectToAction("Game", GameLogic.GetController(gametype), new{ id });
        }

        public ActionResult Surrender(Guid id, GameType gameType)
        {
            var db = new MailGamesContext();
            var board = GameLogic.GetBoard(db, id, gameType);
            var currentPlayer = GameLogic.EnsurePlayersTurn(board);
            board.WinnerState = currentPlayer == GamePlayer.FirstPlayer ? WinnerState.FirstPlayerResigned : WinnerState.SecondPlayerResigned;

            GameLogic.UpdateRankings(board);

            db.SaveChanges();

            SendOpponentMail(db, board, "You won the game, congratulations!", "Opponent resigned");

            return RedirectToAction("Game", GameLogic.GetController(gameType), new { id });
        }

        public ActionResult User(int id)
        {
            var db = new MailGamesContext();
            var boards = GameLogic.GetAllBoards(db).Where(b => b.WinnerState.HasValue && (b.FirstPlayer.Id == id || b.SecondPlayer.Id == id)).ToArray();
            var player1States = new[]{ WinnerState.FirstPlayer, WinnerState.SecondPlayerPassive, WinnerState.SecondPlayerResigned };
            var player2States = new[]{WinnerState.SecondPlayer, WinnerState.FirstPlayerPassive, WinnerState.FirstPlayerResigned};
            var model = db.Players.Where(p => p.Id == id).ToArray().Select(p => new UserHomeViewModel
            {
                Guid = p.Guid,
                Name = PlayerManager.GetPlayerName(p),
                GameRankings = p.Rankings.Select(r => new UserHomeViewModel.GameRanking
                {
                    GameType = r.GameType,
                    Ranking = r.Ranking,
                    NumLost = boards.Count(g => GameLogic.GetGameType(g) == r.GameType && (g.FirstPlayer.Id == id ? player2States.Contains(g.WinnerState.Value) : player1States.Contains(g.WinnerState.Value))),
                    NumWon = boards.Count(g => GameLogic.GetGameType(g) == r.GameType && (g.FirstPlayer.Id == id ? player1States.Contains(g.WinnerState.Value) : player2States.Contains(g.WinnerState.Value))),
                    NumTie = boards.Count(g => GameLogic.GetGameType(g) == r.GameType && g.WinnerState == WinnerState.Tie),
                })
            }).Single();
            return View(model);
        }

        public ActionResult GameRankings(GameType gametype)
        {
            var model = new HomeGameRankingsViewModel
            {
                GameType = gametype,
                TopUsers = new MailGamesContext().Players.Select(p => new HomeGameRankingsViewModel.TopUser
                {
                    Id = p.Id,
                    Name = p.FullName,
                    Ranking = (float?)p.Rankings.FirstOrDefault(r => r.GameType == gametype).Ranking ?? 0
                }).Where(p => p.Ranking != 0 && p.Name != null).OrderByDescending(p => p.Ranking)
            };
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
