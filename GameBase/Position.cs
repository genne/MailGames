using System;
using System.Collections.Generic;

namespace GameBase
{
    public class Position
    {
        private const int IntXMultiplier = 10;

        public static IEnumerable<Position> AllDirections()
        {
            return new[]
            {
                new Position(1, 0),
                new Position(0, 1),
                new Position(1, 1),
                new Position(-1, 1),
                new Position(-1, 0),
                new Position(0, -1),
                new Position(-1, -1),
                new Position(1, -1),
            };
        }

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

        public Position Add(Position dir)
        {
            return new Position(X + dir.X, Y + dir.Y);
        }

        public static Position FromInt(int i)
        {
            return new Position(i / IntXMultiplier, i % IntXMultiplier);
        }

        public int ToInt()
        {
            return X*IntXMultiplier + Y;
        }

        public Position Move(Move move)
        {
            return new Position(X + move.DeltaCol, Y + move.DeltaRow);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }
    }
}