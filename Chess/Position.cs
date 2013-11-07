using System.Globalization;

namespace MailGames.Chess
{
    public class Position
    {
        protected bool Equals(Position other)
        {
            return Col == other.Col && Row == other.Row;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Col * 397) ^ Row;
            }
        }

        public int Col { get; set; }
        public int Row { get; set; }

        public Position Move(Move directionalMove)
        {
            return new Position { Col = Col + directionalMove.DeltaCol, Row = Row + directionalMove.DeltaRow };
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position)obj);
        }

        public int ToInt()
        {
            return Col * 10 + Row;
        }

        public bool IsOutside()
        {
            return Col < 0 || Col > 7 || Row < 0 || Row > 7;
        }

        public static Position FromInt(int sourceCell)
        {
            return new Position { Col = sourceCell / 10, Row = sourceCell % 10 };
        }

        public override string ToString()
        {
            return "abcdefgh"[Col] + (Row + 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}