using System;
using GameBase;
using MailGames.Context;
using MailGames.Controllers;
using MailGames.Logic;

namespace MailGames.Models
{
    public class GameViewModel
    {
        public GameType GameType { get; private set; }
        public GameState State { get; private set; }
        public bool GameOver
        {
            get { return State == GameState.OpponentWon || State == GameState.PlayerWon || State == GameState.Tie; }
        }

        public Guid Id { get; private set; }

        public string OpponentName { get; private set; }

        public Activity OpponentActivity { get; private set; }

        public int OpponentId { get; private set; }

        public GameViewModel(IGameBoard board)
        {
            GameType = GameLogic.GetGameType(board);
            State = GameLogic.GetGameState(board);
            Id = board.Id;
            OpponentName = PlayerManager.GetOpponentName(board);
            OpponentId = PlayerManager.GetOpponentId(board);
            OpponentActivity = GameLogic.GetActivity(board);
        }
    }
}