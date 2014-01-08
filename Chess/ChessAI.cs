using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase;

namespace Chess
{
    public class ChessAI : ABoardAI<ChessState, ChessAI.Move>
    {
        private const int DefaultDepth = 2;
        private readonly ChessPointsSettings _pointsSettings;

        public class Move
        {
            public int From { get; set; }
            public int To { get; set; }
            public PieceType? ConvertPawnTo { get; set; }

            public override string ToString()
            {
                return Position.FromInt(From) + " -> " + Position.FromInt(To);
            }
        }

        public ChessAI(ChessPointsSettings pointsSettings = null, int? depth = null) : base(depth ?? DefaultDepth)
        {
            _pointsSettings = pointsSettings ?? new ChessPointsSettings();
        }

        protected override IEnumerable<Move> GetAllMoves(ChessState state)
        {
            return
                GetPiecePositions(state).SelectMany(
                    from =>
                    ChessLogic.GetAvailableTargets(state, from.Key).Select(to => new Move
                    {
                        From = from.Key, To = to,
                        ConvertPawnTo = IsPawnConversion(@from, to) ? PieceType.Queen : (PieceType?) null
                    })).ToArray();
        }

        protected override GamePlayer GetCurrentPlayer(ChessState state)
        {
            return state.CurrentPlayer;
        }

        protected override WinnerState? GetWinnerState(ChessState state)
        {
            bool isCheck;
            return ChessLogic.GetWinnerState(state, out isCheck);
        }

        protected override float GetCurrentPlayerPoints(ChessState state)
        {
            return ChessLogic.GetPoints(state, state.CurrentPlayer, _pointsSettings) - ChessLogic.GetPoints(state, GameBaseLogic.GetNextPlayer(state.CurrentPlayer), _pointsSettings);
        }

        protected override ChessState GetNewState(ChessState state, Move move)
        {
            var newState = new ChessState(state);
            ChessLogic.ApplyMove(newState, move.From, move.To, move.ConvertPawnTo);
            return newState;
        }

        private static bool IsPawnConversion(KeyValuePair<int, Piece> @from, int to)
        {
            return @from.Value.PieceType == PieceType.Pawn && (Position.FromInt(to).Y == 0 || Position.FromInt(to).Y == 7);
        }

        private static IEnumerable<KeyValuePair<int, Piece>> GetPiecePositions(ChessState state)
        {
            return state.GetCells().Where(c => c.Value.GamePlayer == state.CurrentPlayer);
        }
    }
}
