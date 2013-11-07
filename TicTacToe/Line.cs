using System.Collections.Generic;

namespace TicTacToe
{
    internal class Line
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _dx;
        private readonly int _dy;

        public Line(int x, int y, int dx, int dy)
        {
            _x = x;
            _y = y;
            _dx = dx;
            _dy = dy;
        }

        public IEnumerable<Position> Cells { get
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new Position(_x + _dx * i, _y + _dy * i);
            }
        }}
    }
}