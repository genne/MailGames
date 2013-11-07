using System;
using System.Collections.Generic;
using TicTacToe;

namespace MailGames.Context
{
    public class TicTacToeBoard : IGameBoard
    {
        public Guid Id { get; set; }

        public virtual ICollection<TicTacToeMove> Moves { get; set; }

        public virtual Player FirstPlayer { get; set; }
        public virtual Player SecondPlayer { get; set; }

        public virtual TicTacToeWinner Winner { get; set; }
    }

    public class TicTacToeMove
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public DateTime DateTime { get; set; }
    }
}