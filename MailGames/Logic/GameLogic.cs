using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GameBase;
using MailGames.Context;
using MailGames.Controllers;
using MailGames.Models;
using TicTacToe;
using WebMatrix.WebData;

namespace MailGames.Logic
{
    public class GameLogic
    {
        private static readonly TimeSpan PassivityDuration = TimeSpan.FromDays(1);

        public static IGameBoard GetBoard(MailGamesContext db, Guid id, GameType gametype)
        {
            return GetVisitor(gametype).FindBoard(db, id);
        }

        public static GamePlayer EnsurePlayersTurn(IGameBoard boardObj)
        {
            if (boardObj.WinnerState.HasValue) throw new ValidationException("Game over");
            var currentPlayer = GetCurrentPlayer(boardObj);
            var loggedInPlayer = GetLoggedInPlayer(boardObj);
            if (currentPlayer != loggedInPlayer) throw new ValidationException("Not your turn");
            return currentPlayer;
        }

        private static GamePlayer? GetWinner(WinnerState winnerState)
        {
            switch (winnerState)
            {
                case WinnerState.FirstPlayer:
                    return GamePlayer.FirstPlayer;
                case WinnerState.SecondPlayer:
                    return GamePlayer.SecondPlayer;
                case WinnerState.Tie:
                    return null;
                case WinnerState.FirstPlayerResigned:
                    return GamePlayer.SecondPlayer;
                    break;
                case WinnerState.SecondPlayerResigned:
                    return GamePlayer.FirstPlayer;
                    break;
                case WinnerState.FirstPlayerPassive:
                    return GamePlayer.SecondPlayer;
                    break;
                case WinnerState.SecondPlayerPassive:
                    return GamePlayer.FirstPlayer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("winnerState");
            }
        }

        public static GameState GetGameState(IGameBoard board)
        {
            var winnerState = board.WinnerState;
            var loggedInPlayer = GetLoggedInPlayer(board);
            var currentPlayer = GetCurrentPlayer(board);
            if (winnerState.HasValue)
            {
                var winner = GetWinner(winnerState.Value);
                if (winner == null) return GameState.Tie;
                return winner == loggedInPlayer ? GameState.PlayerWon : GameState.OpponentWon;
            }
            return currentPlayer == loggedInPlayer ? GameState.YourTurn : GameState.OpponentsTurn;
        }

        public static Activity GetActivity(IEnumerable<DateTime> allMoves, IGameBoard board)
        {
            DateTime? lastMove = null;
            if (allMoves.Any())
                lastMove = allMoves.Max();
            return GetActivity(lastMove, board);
        }

        public static GamePlayer GetLoggedInPlayer(IGameBoard boardObj)
        {
            return boardObj.FirstPlayer.Id == WebSecurity.CurrentUserId ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
        }

        public static IGameBoard CreateGameBoard(GameType gameType, MailGamesContext db)
        {
            return GetVisitor(gameType).CreateBoard(db);
        }

        private static IGameBoardVisitor GetVisitor(GameType gameType)
        {
            return GetVisitor(null, gameType);
        }

        private static IGameBoardVisitor GetVisitor(IGameBoard board)
        {
            return GetVisitor(board, GetGameType(board));
        }

        private static IGameBoardVisitor GetVisitor(IGameBoard board, GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Chess:
                    return new ChessBoardVisitor(board as ChessBoard);
                case GameType.TicTacToe:
                    return new TicTacToeVisitor(board as TicTacToeBoard, TicTacToeVariant.Original);
                case GameType.CrissCross:
                    return new TicTacToeVisitor(board as TicTacToeBoard, TicTacToeVariant.Lufferschess);
                case GameType.Othello:
                    return new OthelloVisitor(board as OthelloBoard);
                default:
                    throw new ArgumentException("board");
            }
        }

        public static GamePlayer GetCurrentPlayer(IGameBoard board)
        {
            return GetVisitor(board).GetCurrentPlayer();
        }

        public static GamePlayer? GetNextPlayer(GamePlayer currentPlayer)
        {
            return currentPlayer == GamePlayer.FirstPlayer ? GamePlayer.SecondPlayer : GamePlayer.FirstPlayer;
        }

        public static DateTime? GetLastActive(IGameBoard board)
        {
            return GetVisitor(board).GetActivityDates().OrderByDescending(d => d).OfType<DateTime?>().FirstOrDefault();
        }

        public static IEnumerable<IGameBoard> GetAllBoards(MailGamesContext db)
        {
            return db.ChessBoards.ToArray()
                .Concat(db.TicTacToeBoards.ToArray().OfType<IGameBoard>())
                .Concat(db.OthelloBoards.ToArray().OfType<IGameBoard>())
                .ToArray();
        }

        public static Activity GetActivity(IGameBoard gameBoard)
        {
            return GetActivity(GetLastActive(gameBoard), gameBoard);
        }

        private static Activity GetActivity(DateTime? lastMove, IGameBoard board)
        {
            if (!lastMove.HasValue) return Activity.Active;
            var lastReminded = board.LastReminded;
            var lastActiveDate = lastReminded.HasValue && lastReminded.Value > lastMove ? lastReminded.Value : lastMove;
            var passive = DateTime.Now - lastActiveDate > PassivityDuration;
            if (!passive) return Activity.Active;
            return board.LastReminded.HasValue ? Activity.PassiveLostGame : Activity.Passive;

        }

        public static string GetName(GameType type)
        {
            return GetVisitor(type).GetName();
        }

        public static GameType GetGameType(IGameBoard boardObj)
        {
            var type = boardObj.GetType();
            if (IsType<ChessBoard>(type)) return GameType.Chess;
            if (IsType<TicTacToeBoard>(type)) 
                return (boardObj as TicTacToeBoard).Variant == TicTacToeVariant.Lufferschess ? GameType.CrissCross : GameType.TicTacToe;
            if (IsType<OthelloBoard>(type))
                return GameType.Othello;
            throw new ArgumentException("Unknown type: " + type);
        }

        private static bool IsType<T>(Type type)
        {
            return type == typeof(T) || type.BaseType == typeof(T);
        }

        public static IEnumerable<GameType> GetGameTypes()
        {
            return Enum.GetValues(typeof(GameType)).OfType<GameType>();
        }

        public static string GetController(IGameBoard gameBoard)
        {
            return GetVisitor(gameBoard).GetController();
        }

        public static string GetController(GameType gameBoard)
        {
            return GetVisitor(gameBoard).GetController();
        }

        public static WinnerState? GetWinnerState(IGameBoard board)
        {
            return GetVisitor(board).GetWinnerState();
        }

        public static string GetWikipediaId(GameType gameType)
        {
            return GetVisitor(gameType).GetWikipediaId();
        }
    }
}