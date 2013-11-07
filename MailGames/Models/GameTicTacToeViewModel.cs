using System;
using GameBase;
using TicTacToe;

namespace MailGames.Models
{
    public class GameTicTacToeViewModel : GameViewModel
    {
        public TicTacToeColor?[,] Colors { get; set; }

        public Guid Id { get; set; }
    }

    public class GameViewModel
    {
        public GameType GameType { get; set; }
        public GameState State { get; set; }
        public bool GameOver
        {
            get { return State == GameState.OpponentWon || State == GameState.PlayerWon || State == GameState.Tie; }
        }
    }
}