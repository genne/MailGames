using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using MailGames.Context;

namespace Chess
{
    public static class ChessLogic
    {
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
            var pawnStartRow = piece.GamePlayer == GamePlayer.SecondPlayer ? 1 : 6;
            var pawnDir = piece.GamePlayer == GamePlayer.SecondPlayer ? 1 : -1;
            if (!attackOnly && pos.Y == pawnStartRow 
                && state.GetCell(pos.Add(new Position(0, pawnDir))) == null
                && state.GetCell(pos.Add(new Position(0, pawnDir*2))) == null
                ) yield return new Move {DeltaRow = 2*pawnDir};

            foreach (var directionalMove in new[] {-1, 1}.Select(d => new Move {DeltaRow = pawnDir, DeltaCol = d}))
            {
                var position = pos.Move(directionalMove);
                if (IsOutside(position)) continue;
                var directionalMoveTarget = position.ToInt();
                if (state.GetCell(directionalMoveTarget) != null &&
                    state.GetCell(directionalMoveTarget).GamePlayer != piece.GamePlayer)
                    yield return directionalMove;
            }

            if (attackOnly) yield break;

            var forwardMove = new Move {DeltaRow = 1*pawnDir};
            if (state.GetCell(pos.Move(forwardMove)) == null) yield return forwardMove;
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
            SetCell(board, 3, 0, new Piece { GamePlayer = GamePlayer.SecondPlayer, PieceType = PieceType.Queen });
            SetCell(board, 4, 0, new Piece { GamePlayer = GamePlayer.SecondPlayer, PieceType = PieceType.King });
            SetCell(board, 3, 7, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.Queen });
            SetCell(board, 4, 7, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.King });
            return board;
        }

        private static void AddReflected(ChessState state, int col, int row, PieceType pieceType)
        {
            SetCell(state, col, row, new Piece { GamePlayer = GamePlayer.SecondPlayer, PieceType = pieceType });
            SetCell(state, col, 7 - row, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = pieceType });
            SetCell(state, 7 - col, row,  new Piece { GamePlayer = GamePlayer.SecondPlayer, PieceType = pieceType });
            SetCell(state, 7 - col, 7 - row, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = pieceType });
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

        public static void ValidateMove(ChessState state, int @from, int to, PieceType? convertPawnTo)
        {
            var fromCell = state.GetCell(@from);
            if (fromCell.GamePlayer != state.CurrentPlayer) throw new ArgumentException("Invalid move", "from");
            var pawnConversionRow = fromCell.GamePlayer == GamePlayer.FirstPlayer ? 1 : 6;
            if ((fromCell.PieceType == PieceType.Pawn && Position.FromInt(from).Y == pawnConversionRow) != convertPawnTo.HasValue) throw new ArgumentException("Invalid pawn conversion", "convertPawnTo");
            //if (!new ChessLogic().GetAvailableTargets(state, from).Contains(to)) throw new ArgumentException("Invalid move", "to");
        }

        private static void ApplyMoveWithoutColorSwap(ChessState state, int @from, int to, PieceType? pawnConversion)
        {
            var piece = state.GetCell(@from);
            if (pawnConversion.HasValue)
            {
                piece.PieceType = pawnConversion.Value;
            }
            var capturedPiece = state.GetCell(to);
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
            var points1 = GetPoints(state, player);
            var points2 = GetPoints(state, GameBaseLogic.GetNextPlayer(player));
            if (points1 == 0) return 0;
            if (points2 == 0) return MaxProgress;
            return Math.Max(0, Math.Min(MaxProgress, (MaxProgress / 2) + (points1 - points2) * (MaxProgress / 2) / GetPieceTypePoints(PieceType.Queen)));
        }

        public static int GetPoints(ChessState state, GamePlayer gamePlayer)
        {
            return FindPieces(state, gamePlayer).Sum(p => GetPieceTypePoints(p.Value.PieceType));
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
    }
}