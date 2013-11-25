using System;
using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    public class TicTacToeState
    {
        private Dictionary<Position, TicTacToeColor> _colors = new Dictionary<Position, TicTacToeColor>(new PositionComparer());

        public TicTacToeState(TicTacToeVariant variant)
        {
            switch (variant)
            {
                case TicTacToeVariant.Original:
                    Width = Height = 3;
                    NumInRow = 3;
                    break;
                case TicTacToeVariant.Lufferschess:
                    Width = Height = 15;
                    NumInRow = 5;
                    break;
            };
        }

        public TicTacToeColor? Get(Position position)
        {
            TicTacToeColor ticTacToeColor;
            if (_colors.TryGetValue(position, out ticTacToeColor))
                return ticTacToeColor;
            return null;
        }

        public void Play(int x, int y)
        {
            var position = Validate(x, y);
            _colors[position] = CurrentPlayer;
        }

        public Position Validate(int x, int y)
        {
            var position = new Position(x, y);
            TicTacToeColor value;
            if (_colors.TryGetValue(position, out value))
                throw new InvalidOperationException("That position is already played");
            return position;
        }

        public TicTacToeColor CurrentPlayer { get { return _colors.Count%2 == 0 ? TicTacToeColor.X : TicTacToeColor.O; } }

        public int Width { get; set; }
        public int Height { get; set; }
        public int NumInRow { get; set; }

        public bool IsFull()
        {
            return _colors.Count == Width * Height;
        }
    }
}