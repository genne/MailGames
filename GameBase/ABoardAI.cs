using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Profiling;

namespace GameBase
{
    public abstract class ABoardAI<TState, TMove>
    {
        private readonly int _depth;

        protected ABoardAI(int depth)
        {
            _depth = depth;
        }

        public IEnumerable<TMove> GetBestMoves(TState state)
        {
            var depthCounter = new DepthCounter(_depth);
            var orderByPoints = GetBestMovesWithPoints(state, depthCounter);
            if (!orderByPoints.Any())
            {
                return new TMove[0];
            }

            var bestPoint = orderByPoints.First().Points;
            var bestMoves = orderByPoints.TakeWhile(p => p.Points == bestPoint);
            return bestMoves.Select(m => m.Move);
        }

        public class DepthCounter
        {
            private DepthCounter _topCounter;
            public int Depth { get; private set; }
            public int TestCount { get; private set; }

            public DepthCounter(int depth, DepthCounter topCounter = null)
            {
                Depth = depth;
                _topCounter = topCounter ?? this;
            }

            public DepthCounter Decrease()
            {
                return new DepthCounter(Depth - 1, _topCounter);
            }

            public bool ShouldContinue()
            {
                _topCounter.TestCount++;
                return Depth > 0;
            }
        }

        public MovePoint[] GetBestMovesWithPoints(TState state, DepthCounter depth)
        {
            IEnumerable<TMove> allMoves;
            using (SummaryProfiler.Current.Step("GetAllMoves"))
            {
                allMoves = GetAllMoves(state);
            }
            using (SummaryProfiler.Current.Step("OrderByPoints"))
            {
                var orderByPoints = allMoves.Select(m => new MovePoint
                {
                    Move = m,
                    Points = GetPoints(state, m, depth)
                }).ToArray().OrderByDescending(m => m.Points).ToArray();
                return orderByPoints;
            }
        }

        public class MovePoint
        {
            public TMove Move { get; set; }
            public float Points { get; set; }
        }

        protected abstract IEnumerable<TMove> GetAllMoves(TState state);

        public TMove GetRandomBestMove(TState state)
        {
            var bestMoves = GetBestMoves(state).ToArray();
            return bestMoves[new Random().Next(bestMoves.Length)];
        }

        protected float GetBestPoints(TState state, DepthCounter depth)
        {
            using (SummaryProfiler.Current.Step("GetBestPoints"))
            {
                var bestMove = GetBestMovesWithPoints(state, depth).FirstOrDefault();
                if (bestMove == null) return float.MinValue; // Check mate?
                return bestMove.Points;
            }
        }

        protected virtual float GetPoints(TState state, TMove move, DepthCounter depth)
        {
            TState newState;
            using (SummaryProfiler.Current.Step("GetNewState"))
            {
                newState = GetNewState(state, move);
            }

            using (SummaryProfiler.Current.Step("WinnerState"))
            {
                var winnerState = GetWinnerState(newState);

                if (winnerState.HasValue)
                {
                    var winner = GameBaseLogic.GetWinner(winnerState.Value);
                    if (!winner.HasValue) return 0; // Equal
                    return winner.Value == GetCurrentPlayer(state) ? float.MaxValue : float.MinValue;
                }
            }

            if (depth.ShouldContinue())
            {
                var opponentBestPoints = GetBestPoints(newState, depth.Decrease());
                return -opponentBestPoints;
            }

            using (SummaryProfiler.Current.Step("GetCurrentPlayerPoints"))
            {
                return -GetCurrentPlayerPoints(newState); // Current player is opponent, negate points
            }
        }

        protected abstract GamePlayer GetCurrentPlayer(TState state);

        protected abstract WinnerState? GetWinnerState(TState state);

        protected abstract float GetCurrentPlayerPoints(TState state);

        protected abstract TState GetNewState(TState state, TMove move);
    }
}