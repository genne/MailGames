using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    internal class Line
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _dx;
        private readonly int _dy;
        private readonly int _length;

        public Line(int x, int y, int dx, int dy, int length)
        {
            _x = x;
            _y = y;
            _dx = dx;
            _dy = dy;
            _length = length;
        }

        public IEnumerable<Position> Cells { get
        {
            for (var i = 0; i < _length; i++)
            {
                yield return new Position(_x + _dx * i, _y + _dy * i);
            }
        }}
    }
}