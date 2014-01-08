using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameBase;

namespace Chess
{
    public static class ChessLogic
    {
        private static readonly ChessPointsSettings ChessPointsSettings = new ChessPointsSettings();
        private const int MaxProgress = 100;

        public static IEnumerable<int> GetAvailableTargets(ChessState state, int sourceCell)
        {
            var piece = state.GetCell(sourceCell);
            var pos = Position.FromInt(sourceCell);
            var allTargets = GetAllMoves(piece, pos, state);
            return allTargets.Where(t => !IsCheck(state, pos, t, state.CurrentPlayer)).Select(t => pos.Move(t).ToInt()).ToArray();
        }

        private static bool IsCheck(ChessState state, Position pos, Move move, GamePlayer color)
        {
            var targetState = new ChessState(state);
            ApplyMove(targetState, pos.ToInt(), pos.Move(move).ToInt(), IsPawnConversion(state, pos) ? PieceType.Queen : (PieceType?) null);
            return IsCheck(targetState, color);
        }

        private static bool IsPawnConversion(ChessState state, Position pos)
        {
            var pawnConversionRow = state.CurrentPlayer == GamePlayer.FirstPlayer ? 1 : 6;
            return pos.Y == pawnConversionRow && state.GetCell(pos).PieceType == PieceType.Pawn;
        }

        public static bool IsCheck(ChessState state, GamePlayer color)
        {
            return IsAttacked(state, PieceType.King, color);
        }

        private static IEnumerable<Move> GetAllMoves(Piece piece, Position pos, ChessState state, bool attackOnly = false)
        {
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    return GetPawnMoves(piece, pos, state, attackOnly);

                case PieceType.Knight:
                    return GetKnightMoves(piece, pos, state);

                case PieceType.Bishop:
                    return GetBishopMoves(piece, pos, state);

                case PieceType.Rook:
                    return GetRookMoves(piece, pos, state);

                case PieceType.Queen:
                    return GetQueenMoves(piece, pos, state);

                case PieceType.King:
                    return GetKingMoves(piece, pos, state, attackOnly);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<Move> GetKingMoves(Piece piece, Position pos, ChessState state, bool attackOnly)
        {
            var moves = new List<Move>();
            foreach (var move in AllDirections.Where(move => GetTargetState(piece, pos, state, move) != TargetState.SelfOrOutside))
            {
                moves.Add(move);
            }

            // Rockad
            if (attackOnly) return moves;

            var kingPos = FindPiece(state, PieceType.King, state.CurrentPlayer);
            if (IsCheck(state, state.CurrentPlayer) || HasMoved(state, kingPos)) return moves;

            foreach (var rookPos in FindPieces(state, PieceType.Rook, state.CurrentPlayer))
            {
                if (HasMoved(state, rookPos)) continue;

                var cellsBetween = GetCellsBetween(kingPos, rookPos, false);
                var rookDir = rookPos.X > kingPos.X ? 1 : -1;
                var kingMovement = new Move {DeltaCol = 2*rookDir};
                var kingTargetPos = kingPos.Move(kingMovement);
                var kingMovementCells = GetCellsBetween(kingPos, kingTargetPos, true).ToArray();
                var isCellsBetweenAlreadyOccupied = cellsBetween.Any(c => state.GetCell(c) != null);
                var isKingMovementCellsAttacked =
                    kingMovementCells.Any(
                        kingMovementCell => IsAttacked(state, kingMovementCell, state.CurrentPlayer));
                if (!isCellsBetweenAlreadyOccupied
                    && !isKingMovementCellsAttacked)
                    moves.Add(kingMovement);
            }

            return moves;
        }

        private static IEnumerable<Move> GetQueenMoves(Piece piece, Position pos, ChessState state)
        {
            return AllMovesInDirections(piece, pos, state, AllDirections);
        }

        private static IEnumerable<Move> GetRookMoves(Piece piece, Position pos, ChessState state)
        {
            var rookDirections = new[]
            {
                new Move {DeltaCol = -1}, 
                new Move {DeltaCol = 1}, 
                new Move {DeltaRow = -1}, 
                new Move {DeltaRow = 1}
            };
            return AllMovesInDirections(piece, pos, state, rookDirections);
        }

        private static IEnumerable<Move> GetBishopMoves(Piece piece, Position pos, ChessState state)
        {
            var bishopDirections = new[]
            {
                new Move {DeltaCol = -1, DeltaRow = -1}, 
                new Move {DeltaCol = 1, DeltaRow = -1},
                new Move {DeltaCol = -1, DeltaRow = 1}, 
                new Move {DeltaCol = 1, DeltaRow = 1}
            };
            return AllMovesInDirections(piece, pos, state, bishopDirections);
        }

        private static IEnumerable<Move> GetKnightMoves(Piece piece, Position pos, ChessState state)
        {
            var moves = new List<Move>();
            foreach (var dx in new[] {-1, 1})
            {
                foreach (var dy in new[] {-2, 2})
                {
                    moves.Add(new Move {DeltaCol = dx, DeltaRow = dy});
                    moves.Add(new Move {DeltaCol = dy, DeltaRow = dx});
                }
            }
            return moves.Where(move => GetTargetState(piece, pos, state, move) != TargetState.SelfOrOutside);
        }

        private static IEnumerable<Move> GetPawnMoves(Piece piece, Position pos, ChessState state, bool attackOnly)
        {
            var gamePlayer = piece.GamePlayer;
            var pawnStartRow = gamePlayer == GamePlayer.SecondPlayer ? 1 : 6;
            var pawnDir = GetPawnDir(gamePlayer);
            if (!attackOnly && pos.Y == pawnStartRow 
                && state.GetCell(pos.Add(new Position(0, pawnDir))) == null
                && state.GetCell(pos.Add(new Position(0, pawnDir*2))) == null
                ) yield return new Move {DeltaRow = 2*pawnDir};

            foreach (var directionalMove in new[] {-1, 1}.Select(d => new Move {DeltaRow = pawnDir, DeltaCol = d}))
            {
                var targetPos = pos.Move(directionalMove);
                if (IsOutside(targetPos)) continue;
                var directionalMoveTarget = targetPos.ToInt();
                var targetPiece = state.GetCell(directionalMoveTarget);
                if (targetPiece != null &&
                    targetPiece.GamePlayer != gamePlayer)
                    yield return directionalMove;
                else if (IsEnPassant(state, targetPos))
                    yield return directionalMove;
            }

            if (attackOnly) yield break;

            var forwardMove = new Move {DeltaRow = 1*pawnDir};
            if (state.GetCell(pos.Move(forwardMove)) == null) yield return forwardMove;
        }

        private static int GetPawnDir(GamePlayer gamePlayer)
        {
            return gamePlayer == GamePlayer.SecondPlayer ? 1 : -1;
        }

        private static bool IsEnPassant(ChessState state, Position targetPos)
        {
            var lastMove = state.Moves.LastOrDefault();
            if (lastMove == null) return false;
            var yDir = GetPawnDir(lastMove.Piece.GamePlayer);
            return lastMove.Piece.PieceType == PieceType.Pawn
                   && lastMove.From.Equals(new Position(targetPos.X, targetPos.Y - yDir))
                   && lastMove.To.Equals(new Position(targetPos.X, targetPos.Y + yDir));
        }

        private static IEnumerable<Move> AllDirections
        {
            get
            {
                return from i in new[] {-1, 0, 1} 
                       from j in new[] {-1, 0, 1} 
                       where i != 0 || j != 0
                       select new Move {DeltaCol = i, DeltaRow = j};
            }
        }

        private static bool IsOutside(Position position)
        {
            return position.X < 0 || position.Y < 0 || position.X > 7 || position.Y > 7;
        }

        private static IEnumerable<Position> GetCellsBetween(Position from, Position to, bool includeTo)
        {
            if  (from.Y != to.Y) throw new NotImplementedException();

            var fromCol = @from.X;
            var toCol = to.X;
            if (fromCol > toCol)
            {
                var temp = fromCol;
                fromCol = toCol;
                toCol = temp;
                if (includeTo) fromCol -= 1;
            }
            else if (includeTo) toCol += 1;
            for (int i = fromCol + 1; i < toCol; i++)
            {
                yield return new Position(i, from.Y);
            }
        }

        private static bool HasMoved(ChessState state, Position rook)
        {
            return state.HasMoved(rook);
        }

        private static IEnumerable<Position> FindPieces(ChessState state, PieceType pieceType, GamePlayer gamePlayer)
        {
            return state.GetCells().Where(c => c.Value.GamePlayer == gamePlayer && c.Value.PieceType == pieceType).Select(p => Position.FromInt(p.Key));
        }

        private static IEnumerable<KeyValuePair<int, Piece>> FindPieces(ChessState state, GamePlayer gamePlayer)
        {
            return state.GetCells().Where(c => c.Value.GamePlayer == gamePlayer);
        }

        private static Position FindPiece(ChessState state, PieceType pieceType, GamePlayer gamePlayer)
        {
            return FindPieces(state, pieceType, gamePlayer).Single();
        }

        private static bool IsAttacked(ChessState state, PieceType pieceType, GamePlayer gamePlayer)
        {
            return IsAttacked(state, FindPiece(state, pieceType, gamePlayer), gamePlayer);
        }

        private static bool IsAttacked(ChessState state, Position position, GamePlayer gamePlayer)
        {
            var allOpponentPieces = state.GetCells().Where(c => c.Value.GamePlayer != gamePlayer);
            return
                allOpponentPieces.Any(
                    p =>
                    GetAllMoves(p.Value, Position.FromInt(p.Key), state, attackOnly: true)
                        .Any(m => Position.FromInt(p.Key).Move(m).Equals(position)));
        }

        private static IEnumerable<Move> AllMovesInDirections(Piece piece, Position pos, ChessState state, IEnumerable<Move> allDirections)
        {
            foreach (var move in allDirections)
            {
                var curMove = move;
                while (true)
                {
                    var targetState = GetTargetState(piece, pos, state, curMove);
                    if (targetState != TargetState.SelfOrOutside)
                        yield return curMove;
                    if (targetState != TargetState.Blank)
                        break;
                    curMove = curMove.Forward();
                }
            }
        }

        private static TargetState GetTargetState(Piece piece, Position pos, ChessState state, Move move)
        {
            var targetPos = pos.Move(move);
            if (IsOutside(targetPos)) return TargetState.SelfOrOutside;
            var targetCell = state.GetCell(targetPos);
            if (targetCell == null) return TargetState.Blank;
            if (targetCell.GamePlayer != piece.GamePlayer) return TargetState.Enemy;
            return TargetState.SelfOrOutside;
        }

        private enum TargetState
        {
            SelfOrOutside,
            Blank,
            Enemy
        }

        public static ChessState CreateInitialChessState()
        {
            var board = new ChessState();
            for (int i = 0; i < 4; i++)
            {
                AddReflected(board, i, 1, PieceType.Pawn);
            }
            AddReflected(board, 0, 0, PieceType.Rook);
            AddReflected(board, 1, 0, PieceType.Knight);
            AddReflected(board, 2, 0, PieceType.Bishop);
            SetCell(board, 3, GetPlayerRow(0, GamePlayer.SecondPlayer), new Piece(GamePlayer.SecondPlayer, PieceType.Queen));
            SetCell(board, 4, GetPlayerRow(0, GamePlayer.SecondPlayer), new Piece(GamePlayer.SecondPlayer, PieceType.King ));
            SetCell(board, 3, GetPlayerRow(0, GamePlayer.FirstPlayer), new Piece(GamePlayer.FirstPlayer,  PieceType.Queen));
            SetCell(board, 4, GetPlayerRow(0, GamePlayer.FirstPlayer), new Piece(GamePlayer.FirstPlayer, PieceType.King));
            return board;
        }

        private static void AddReflected(ChessState state, int col, int row, PieceType pieceType)
        {
            foreach (var player in GameBaseLogic.GetAllPlayers())
            {
                SetCell(state, col, GetPlayerRow(row, player),
                        new Piece(player, pieceType));
                SetCell(state, 7 - col, GetPlayerRow(row, player), new Piece(player, pieceType));
            }
        }

        private static int GetPlayerRow(int row, GamePlayer player)
        {
            return player == GamePlayer.SecondPlayer ? row : 7 - row;
        }

        private static void SetCell(ChessState state, int col, int row, Piece piece)
        {
            state.SetCell(new Position(col, row).ToInt(), piece);
        }

        public static void ApplyMove(ChessState state, int @from, int to, PieceType? pawnConversion)
        {
            ValidateMove(state, @from, to, pawnConversion);
            state.CurrentPlayer = GetNextColor(state.CurrentPlayer);

            ApplyMoveWithoutColorSwap(state, @from, to, pawnConversion);

            bool wasKingMoved = state.GetCell(to).PieceType == PieceType.King;
            if (wasKingMoved)
            {
                var fromPos = Position.FromInt(to);
                int deltaCol = fromPos.X - Position.FromInt(from).X;
                bool isRockad = Math.Abs(deltaCol) > 1;
                if (isRockad)
                {
                    int rookPos = new Position(deltaCol > 0 ? 7 : 0, fromPos.Y).ToInt();
                    int rookTargetPos = new Position(deltaCol > 0 ? 5 : 3, fromPos.Y).ToInt();
                    ApplyMoveWithoutColorSwap(state, rookPos, rookTargetPos, null);
                }
            }
        }

        public static void ValidateMove(ChessState state, int @from, int to, PieceType? convertPawnTo, bool validateTarget = false)
        {
            var fromCell = state.GetCell(@from);
            if (fromCell.GamePlayer != state.CurrentPlayer) throw new ArgumentException("Invalid move", "from");
            var pawnConversionRow = fromCell.GamePlayer == GamePlayer.FirstPlayer ? 1 : 6;
            if ((fromCell.PieceType == PieceType.Pawn && Position.FromInt(from).Y == pawnConversionRow) != convertPawnTo.HasValue) throw new ArgumentException("Invalid pawn conversion", "convertPawnTo");
            if (validateTarget && !GetAvailableTargets(state, from).Contains(to)) throw new ArgumentException("Invalid move", "to");
        }

        private static void ApplyMoveWithoutColorSwap(ChessState state, int @from, int to, PieceType? pawnConversion)
        {
            var piece = state.GetCell(@from);
            if (pawnConversion.HasValue)
            {
                piece = new Piece(piece.GamePlayer, pawnConversion.Value);
            }
            var capturedPiece = state.GetCell(to);
            if (capturedPiece != null && capturedPiece.PieceType == PieceType.King) throw new InvalidOperationException("Can't capture king, game should end. From=" + from + " to=" + to + " FromType=" + piece.PieceType + " CurrentPlayer=" + state.CurrentPlayer);
            state.SetCell(to, piece);
            state.SetCell(@from, null);
            state.AddMove(piece, from, to, capturedPiece);
            state.MarkAsMoved(to);
        }

        public static GamePlayer GetNextColor(GamePlayer currentColor)
        {
            return currentColor == GamePlayer.FirstPlayer ? GamePlayer.SecondPlayer : GamePlayer.FirstPlayer;
        }

        public static WinnerState? GetWinnerState(ChessState state, out bool isCheck)
        {
            isCheck = IsCheck(state, state.CurrentPlayer);
            var canMove = state.GetCells().Any(c =>
            {
                int sourceCell = c.Key;
                var piece = state.GetCell(sourceCell);
                var pos = Position.FromInt(sourceCell);
                var allTargets = GetAllMoves(piece, pos, state);
                return c.Value.GamePlayer == state.CurrentPlayer && allTargets.Where(t => !IsCheck(state, pos, t, state.CurrentPlayer)).Select(t => pos.Move(t).ToInt()).Any();
            });
            if (canMove) return null;
            return isCheck ? GetWinnerStateForPlayer(GetNextColor(state.CurrentPlayer)) : WinnerState.Tie;
        }

        private static WinnerState GetWinnerStateForPlayer(GamePlayer currentPlayer)
        {
            switch (currentPlayer)
            {
                case GamePlayer.FirstPlayer:
                    return WinnerState.FirstPlayer;
                case GamePlayer.SecondPlayer:
                    return WinnerState.SecondPlayer;
                default:
                    throw new ArgumentOutOfRangeException("currentPlayer");
            }
        }

        public static float GetProgress(ChessState state, GamePlayer player)
        {
            var points1 = GetPoints(state, player, ChessPointsSettings);
            var points2 = GetPoints(state, GameBaseLogic.GetNextPlayer(player), ChessPointsSettings);
            if (points1 == 0) return 0;
            if (points2 == 0) return MaxProgress;
            return Math.Max(0, Math.Min(MaxProgress, (MaxProgress / 2) + (points1 - points2) * (MaxProgress / 2) / GetPieceTypePoints(PieceType.Queen)));
        }

        public static float GetPoints(ChessState state, GamePlayer gamePlayer, ChessPointsSettings pointsSettings)
        {
            return FindPieces(state, gamePlayer).Sum(p => GetPiecePoints(p.Key, p.Value, pointsSettings));
        }

        private static float GetPiecePoints(int position, Piece piece, ChessPointsSettings pointsSettings)
        {
            float points = GetPieceTypePoints(piece.PieceType);
            if (piece.PieceType == PieceType.Pawn)
            {
                points += pointsSettings.PawnProgressionPoints*GetPawnProgression(position, piece.GamePlayer);
            }
            return points;
        }

        private static int GetPawnProgression(int position, GamePlayer gamePlayer)
        {
            var row = Position.FromInt(position).Y;
            var startRow = GetPawnStartRow(gamePlayer);
            return Math.Abs(row - startRow);
        }

        private static int GetPawnStartRow(GamePlayer gamePlayer)
        {
            return GetPlayerRow(1, gamePlayer);
        }

        private static int GetPieceTypePoints(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return 1;
                case PieceType.Knight:
                    return 3;
                case PieceType.Bishop:
                    return 3;
                case PieceType.Rook:
                    return 5;
                case PieceType.Queen:
                    return 9;
                case PieceType.King:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException("pieceType");
            }
        }

        public static string FormatChessPosition(Position position)
        {
            return FormatColPosition(position.X) + FormatRowPosition(position.Y);
        }

        public static string FormatRowPosition(int y)
        {
            return (8 - y).ToString(CultureInfo.InvariantCulture);
        }

        public static string FormatColPosition(int x)
        {
            return ((char)('A' + x)).ToString(CultureInfo.InvariantCulture);
        }


        public static Position ParseChessPosition(string cell)
        {
            cell = cell.ToUpper();
            var col = cell[0];
            var row = cell[1];
            return new Position(ParseColPosition(col), ParseRowPosition(row));
        }

        private static int ParseRowPosition(char row)
        {
            return 8 - int.Parse(row.ToString(CultureInfo.InvariantCulture));
        }

        private static int ParseColPosition(char col)
        {
            return col - 'A';
        }

        public static void ApplyMove(ChessState state, string @from, string to, PieceType? pawnConversion)
        {
            ApplyMove(state, ParseChessPosition(from).ToInt(), ParseChessPosition(to).ToInt(), pawnConversion);
        }

        public static void ValidateMove(ChessState state, string @from, string to, PieceType? convertPawnTo, bool validateTarget)
        {
            ValidateMove(state, ParseChessPosition(from).ToInt(), ParseChessPosition(to).ToInt(), convertPawnTo, validateTarget);
        }
    }
}