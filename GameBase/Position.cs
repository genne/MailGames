using System;
using System.Collections.Generic;

namespace GameBase
{
    public class Position
    {
        private const int DefaultMaxMultiplier = 10;

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

        public static Position FromInt(int i, int max = DefaultMaxMultiplier, int min = 0)
        {
            return new Position(i / max + min, i % max + min);
        }

        public int ToInt(int max = DefaultMaxMultiplier, int min = 0)
        {
            return (X - min) * max + (Y - min);
        }

        public Position Move(Move move)
        {
            return new Position(X + move.DeltaCol, Y + move.DeltaRow);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public Position Add(Position direction, int multiply = 1)
        {
            return new Position(X + direction.X * multiply, Y + direction.Y * multiply);
        }
    }
}