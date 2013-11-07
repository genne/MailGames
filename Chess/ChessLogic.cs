using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using MailGames.Chess;

namespace Chess
{
    public static class ChessLogic
    {
        public static IEnumerable<int> GetAvailableTargets(ChessState state, int sourceCell)
        {
            var piece = state.GetCell(sourceCell);
            var pos = Position.FromInt(sourceCell);
            var allTargets = GetAllMoves(piece, pos, state);
            return allTargets.Where(t => !IsCheck(state, pos, t, state.CurrentColor)).Select(t => pos.Move(t).ToInt());
        }

        private static bool IsCheck(ChessState state, Position pos, Move move, PieceColor color)
        {
            var targetState = new ChessState(state);
            ApplyMove(targetState, pos.ToInt(), pos.Move(move).ToInt(), IsPawnConversion(state, pos) ? PieceType.Queen : (PieceType?) null);
            return IsCheck(targetState, color);
        }

        private static bool IsPawnConversion(ChessState state, Position pos)
        {
            var pawnConversionRow = state.CurrentColor == PieceColor.White ? 1 : 6;
            return pos.Row == pawnConversionRow && state.GetCell(pos).PieceType == PieceType.Pawn;
        }

        private static bool IsCheck(ChessState state, PieceColor color)
        {
            return IsAttacked(state, PieceType.King, color);
        }

        private static IEnumerable<Move> GetAllMoves(Piece piece, Position pos, ChessState state, bool attackOnly = false)
        {
            var allDirections = (from i in new[] {-1, 0, 1} from j in new[] {-1, 0, 1} select new Move {DeltaCol = i, DeltaRow = j});
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    var pawnStartRow = piece.PieceColor == PieceColor.Black ? 1 : 6;
                    var pawnDir = piece.PieceColor == PieceColor.Black ? 1 : -1;
                    if (!attackOnly && pos.Row == pawnStartRow) yield return new Move { DeltaRow = 2 * pawnDir };

                    foreach (var directionalMove in new[]{ -1, 1 }.Select(d => new Move{ DeltaRow = pawnDir, DeltaCol = d }))
                    {
                        var position = pos.Move(directionalMove);
                        if (position.IsOutside()) continue;
                        int directionalMoveTarget = position.ToInt();
                        if (state.GetCell(directionalMoveTarget) != null && state.GetCell(directionalMoveTarget).PieceColor != piece.PieceColor)
                            yield return directionalMove;
                    }
                    if (!attackOnly)
                    {
                        var forwardMove = new Move {DeltaRow = 1*pawnDir};
                        if (state.GetCell(pos.Move(forwardMove)) == null) yield return forwardMove;
                    }
                    break;

                case PieceType.Knight:
                    var moves = new List<Move>();
                    foreach(var dx in new[]{ -1, 1})
                    {
                        foreach (var dy in new[] {-2, 2})
                        {
                            moves.Add(new Move { DeltaCol = dx, DeltaRow = dy });
                            moves.Add(new Move { DeltaCol = dy, DeltaRow = dx });
                        }
                    }
                    foreach (var move in moves)
                    {
                        if (GetTargetState(piece, pos, state, move) != TargetState.SelfOrOutside)
                            yield return move;
                    }
                    break;

                case PieceType.Bishop:
                    var bishopDirections = new[]{ new Move{ DeltaCol = -1, DeltaRow = -1}, new Move{ DeltaCol = 1, DeltaRow = -1}, new Move{ DeltaCol = -1, DeltaRow = 1}, new Move{ DeltaCol = 1, DeltaRow = 1}};
                    foreach (var move1 in AllMovesInDirections(piece, pos, state, bishopDirections)) yield return move1;
                    break;
                case PieceType.Rook:
                    var rookDirections = new[]{ new Move{ DeltaCol = -1}, new Move{ DeltaCol = 1}, new Move{ DeltaRow = -1}, new Move{ DeltaRow = 1}};
                    foreach (var move1 in AllMovesInDirections(piece, pos, state, rookDirections)) yield return move1;
                    break;
                case PieceType.Queen:
                    foreach (var move1 in AllMovesInDirections(piece, pos, state, allDirections)) yield return move1;
                    break;
                case PieceType.King:
                    foreach (var move in allDirections)
                    {
                        if (GetTargetState(piece, pos, state, move) != TargetState.SelfOrOutside)
                            yield return move;
                    }

                    // Rockad
                    if (!attackOnly)
                    {
                        var kingPos = FindPiece(state, PieceType.King, state.CurrentColor);
                        if (!IsCheck(state, state.CurrentColor) && !HasMoved(state, kingPos))
                        {
                            foreach (var rookPos in FindPieces(state, PieceType.Rook, state.CurrentColor))
                            {
                                var cellsBetween = GetCellsBetween(kingPos, rookPos, false);
                                var rookDir = rookPos.Col > kingPos.Col ? 1 : -1;
                                var kingMovement = new Move {DeltaCol = 2*rookDir};
                                var kingTargetPos = kingPos.Move(kingMovement);
                                var kingMovementCells = GetCellsBetween(kingPos, kingTargetPos, true);
                                var hasRookMoved = HasMoved(state, rookPos);
                                var isCellsBetweenAlreadyOccupied = cellsBetween.Any(c => state.GetCell(c) != null);
                                var isKingMovementCellsAttacked =
                                    kingMovementCells.Any(
                                        kingMovementCell => IsAttacked(state, kingMovementCell, state.CurrentColor));
                                if (!hasRookMoved
                                    && !isCellsBetweenAlreadyOccupied
                                    && !isKingMovementCellsAttacked)
                                    yield return kingMovement;
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<Position> GetCellsBetween(Position from, Position to, bool includeTo)
        {
            if  (from.Row != to.Row) throw new NotImplementedException();

            var fromCol = @from.Col;
            var toCol = to.Col;
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
                yield return new Position{ Col = i, Row = from.Row };
            }
        }

        private static bool HasMoved(ChessState state, Position rook)
        {
            return state.HasMoved(rook);
        }

        private static IEnumerable<Position> FindPieces(ChessState state, PieceType pieceType, PieceColor pieceColor)
        {
            return state.GetCells().Where(c => c.Value.PieceColor == pieceColor && c.Value.PieceType == pieceType).Select(p => Position.FromInt(p.Key));
        }

        private static Position FindPiece(ChessState state, PieceType pieceType, PieceColor pieceColor)
        {
            return FindPieces(state, pieceType, pieceColor).Single();
        }

        private static bool IsAttacked(ChessState state, PieceType pieceType, PieceColor pieceColor)
        {
            return IsAttacked(state, FindPiece(state, pieceType, pieceColor), pieceColor);
        }

        private static bool IsAttacked(ChessState state, Position position, PieceColor pieceColor)
        {
            var allOpponentPieces = state.GetCells().Where(c => c.Value.PieceColor != pieceColor);
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
            if (targetPos.IsOutside()) return TargetState.SelfOrOutside;
            var targetCell = state.GetCell(targetPos);
            if (targetCell == null) return TargetState.Blank;
            if (targetCell.PieceColor != piece.PieceColor) return TargetState.Enemy;
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
            SetCell(board, 3, 0, new Piece { PieceColor = PieceColor.Black, PieceType = PieceType.Queen });
            SetCell(board, 4, 0, new Piece { PieceColor = PieceColor.Black, PieceType = PieceType.King });
            SetCell(board, 3, 7, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.Queen });
            SetCell(board, 4, 7, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.King });
            return board;
        }

        private static void AddReflected(ChessState state, int col, int row, PieceType pieceType)
        {
            SetCell(state, col, row, new Piece { PieceColor = PieceColor.Black, PieceType = pieceType });
            SetCell(state, col, 7 - row, new Piece { PieceColor = PieceColor.White, PieceType = pieceType });
            SetCell(state, 7 - col, row,  new Piece { PieceColor = PieceColor.Black, PieceType = pieceType });
            SetCell(state, 7 - col, 7 - row, new Piece { PieceColor = PieceColor.White, PieceType = pieceType });
        }

        private static void SetCell(ChessState state, int col, int row, Piece piece)
        {
            state.SetCell(new Position {Col = col, Row = row}.ToInt(), piece);
        }

        public static void ApplyMove(ChessState state, int @from, int to, PieceType? pawnConversion)
        {
            ValidateMove(state, @from, to, pawnConversion);
            state.CurrentColor = GetNextColor(state.CurrentColor);

            ApplyMoveWithoutColorSwap(state, @from, to, pawnConversion);

            bool wasKingMoved = state.GetCell(to).PieceType == PieceType.King;
            if (wasKingMoved)
            {
                var fromPos = Position.FromInt(to);
                int deltaCol = fromPos.Col - Position.FromInt(from).Col;
                bool isRockad = Math.Abs(deltaCol) > 1;
                if (isRockad)
                {
                    int rookPos = new Position { Col = deltaCol > 0 ? 7 : 0, Row = fromPos.Row }.ToInt();
                    int rookTargetPos = new Position { Col = deltaCol > 0 ? 5 : 3, Row = fromPos.Row }.ToInt();
                    ApplyMoveWithoutColorSwap(state, rookPos, rookTargetPos, null);
                }
            }
        }

        public static void ValidateMove(ChessState state, int @from, int to, PieceType? convertPawnTo)
        {
            var fromCell = state.GetCell(@from);
            if (fromCell.PieceColor != state.CurrentColor) throw new ArgumentException("Invalid move", "from");
            var pawnConversionRow = fromCell.PieceColor == PieceColor.White ? 1 : 6;
            if ((fromCell.PieceType == PieceType.Pawn && Position.FromInt(from).Row == pawnConversionRow) != convertPawnTo.HasValue) throw new ArgumentException("Invalid pawn conversion", "convertPawnTo");
            //if (!new ChessLogic().GetAvailableTargets(state, from).Contains(to)) throw new ArgumentException("Invalid move", "to");
        }

        private static void ApplyMoveWithoutColorSwap(ChessState state, int @from, int to, PieceType? pawnConversion)
        {
            var piece = state.GetCell(@from);
            if (pawnConversion.HasValue)
            {
                piece.PieceType = pawnConversion.Value;
            }
            state.SetCell(to, piece);
            state.SetCell(@from, null);
            state.AddMove(piece, from, to);
            state.MarkAsMoved(to);
        }

        public static PieceColor GetNextColor(PieceColor currentColor)
        {
            return currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        public static ChessRunningState GetRunningState(ChessState state)
        {
            var isCheck = IsCheck(state, state.CurrentColor);
            var canMove = state.GetCells().Any(c =>
            {
                int sourceCell = c.Key;
                var piece = state.GetCell(sourceCell);
                var pos = Position.FromInt(sourceCell);
                var allTargets = GetAllMoves(piece, pos, state);
                return c.Value.PieceColor == state.CurrentColor && allTargets.Where(t => !IsCheck(state, pos, t, state.CurrentColor)).Select(t => pos.Move(t).ToInt()).Any();
            });
            if (isCheck && canMove) return ChessRunningState.Check;
            if (isCheck && !canMove) return ChessRunningState.CheckMate;
            if (!canMove) return ChessRunningState.StaleMate;
            return ChessRunningState.Nothing;
        }

        public static GameState GetGameState(ChessRunningState state, PieceColor currentPlayer, PieceColor loggedInPlayer)
        {
            if (state == ChessRunningState.CheckMate)
            {
                if (currentPlayer == loggedInPlayer)
                    return GameState.PlayerWon;
                return GameState.OpponentWon;
            }
            if (state == ChessRunningState.StaleMate)
                return GameState.Tie;
            if (currentPlayer == loggedInPlayer) return GameState.YourTurn;
            return GameState.OpponentsTurn;
        }
    }

    public enum ChessRunningState
    {
        Nothing,
        Check,
        CheckMate,
        StaleMate
    }
}