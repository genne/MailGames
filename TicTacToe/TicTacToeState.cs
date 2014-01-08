using System;
using System.Collections;
using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    public class TicTacToeState
    {
        private Dictionary<Position, GamePlayer> _colors = new Dictionary<Position, GamePlayer>(new EqualsComparer<Position>());
        private Dictionary<Line, InterrestingLine> _interrestingLines = new Dictionary<Line, InterrestingLine>(new EqualsComparer<Line>());

        public int Width { get; set; }
        public int Height { get; set; }
        public int NumInRow { get; set; }

        public GamePlayer CurrentPlayer { get { return _colors.Count % 2 == 0 ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer; } }

        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MaxY { get; private set; }


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
            MinX = MaxX = Width/2;
            MinY = MaxY = Height/2;
        }

        public TicTacToeState(TicTacToeState state)
        {
            _colors = new Dictionary<Position, GamePlayer>(state._colors);
            foreach (var interrestingLine in state._interrestingLines)
            {
                _interrestingLines[interrestingLine.Key] = new InterrestingLine(interrestingLine.Value);
            }

            Width = state.Width;
            Height = state.Height;
            NumInRow = state.NumInRow;
            MinX = state.MinX;
            MaxX = state.MaxX;
            MinY = state.MinY;
            MaxY = state.MaxY;
        }

        public GamePlayer? Get(Position position)
        {
            GamePlayer ticTacToeColor;
            if (_colors.TryGetValue(position, out ticTacToeColor))
                return ticTacToeColor;
            return null;
        }

        public void Play(int x, int y)
        {
            var position = new Position(x, y);
            Validate(position);
            var currentPlayer = CurrentPlayer;

            _colors[position] = currentPlayer;
            MinX = Math.Min(MinX, x);
            MaxX = Math.Max(MaxX, x);
            MinY = Math.Min(MinY, y);
            MaxY = Math.Max(MaxY, y);

            foreach (var interrestingLine in GetInterrestingLinesForPosition(position))
            {
                interrestingLine.Add(currentPlayer);
                if (interrestingLine.Count > NumInRow) throw new InvalidOperationException("Line count = " + interrestingLine.Count);
            }
        }

        private IEnumerable<InterrestingLine> GetInterrestingLinesForPosition(Position position)
        {
            IEnumerable<Position> allDirections = new []{ new Position(1, 0), new Position(0, 1), new Position(1, 1), new Position(-1, 1)};
            foreach (var direction in allDirections)
            {
                for (int i = -NumInRow + 1; i <= 0; i++)
                {
                    var currentPosition = position.Add(direction, i);
                    var line = new Line(currentPosition, direction);
                    if (IsInside(line))
                    {
                        yield return GetLine(line);
                    }
                }
            }
        }

        private bool IsInside(Line line)
        {
            return IsInside(line.Position) && IsInside(GetLineEndPosition(line));
        }

        private Position GetLineEndPosition(Line line)
        {
            return line.Position.Add(line.Direction, NumInRow - 1);
        }

        private bool IsInside(Position currentPosition)
        {
            return currentPosition.X >= 0 && currentPosition.X < Width && currentPosition.Y >= 0 &&
                   currentPosition.Y <= Height;
        }

        private InterrestingLine GetLine(Line key)
        {
            InterrestingLine line;
            if (!_interrestingLines.TryGetValue(key, out line))
            {
                line = new InterrestingLine();
                _interrestingLines.Add(key, line);
            }
            return line;
        }

        public void Validate(Position position)
        {
            GamePlayer value;
            if (_colors.TryGetValue(position, out value))
                throw new InvalidOperationException("That position is already played");
        }

        public bool IsFull()
        {
            return _colors.Count == Width * Height;
        }

        public IEnumerable<InterrestingLine> GetInterrestingLines()
        {
            return _interrestingLines.Values;
        }
    }
}