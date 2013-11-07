using System;

namespace TicTacToe
{
    public class Position
    {
        protected bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            if (x < 0 || x > 2) throw new ArgumentException("x");
            if (y < 0 || y > 2) throw new ArgumentException("y");
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position) obj);
        }
    }
}