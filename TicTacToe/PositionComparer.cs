using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    internal class PositionComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position x, Position y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Position obj)
        {
            return obj.GetHashCode();
        }
    }
}