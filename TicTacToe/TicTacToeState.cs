using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class TicTacToeState
    {
        private Dictionary<Position, TicTacToeColor> _colors = new Dictionary<Position, TicTacToeColor>(new PositionComparer());

        public TicTacToeColor? Get(Position position)
        {
            TicTacToeColor ticTacToeColor;
            if (_colors.TryGetValue(position, out ticTacToeColor))
                return ticTacToeColor;
            return null;
        }

        public void Play(int x, int y)
        {
            var position = new Position(x, y);
            TicTacToeColor value;
            if (_colors.TryGetValue(position, out value)) throw new InvalidOperationException("That position is already played");
            _colors[position] = CurrentPlayer;
        }

        public TicTacToeColor CurrentPlayer { get { return _colors.Count%2 == 0 ? TicTacToeColor.X : TicTacToeColor.O; } }

        public bool IsFull()
        {
            return _colors.Count == 3*3;
        }
    }
}