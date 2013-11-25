using System;

namespace MailGames.Context
{
    public class TicTacToeMove
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public DateTime DateTime { get; set; }
    }
}