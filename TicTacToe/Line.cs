using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    public class Line
    {
        protected bool Equals(Line other)
        {
            return Equals(Position, other.Position) && Equals(Direction, other.Direction);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Position != null ? Position.GetHashCode() : 0)*397) ^ (Direction != null ? Direction.GetHashCode() : 0);
            }
        }

        public Position Position { get; set; }
        public Position Direction { get; set; }

        public Line(Position position, Position direction)
        {
            Position = position;
            Direction = direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Line) obj);
        }

        public override string ToString()
        {
            return Position.ToString() + ">" + Direction.ToString();
        }
    }
}