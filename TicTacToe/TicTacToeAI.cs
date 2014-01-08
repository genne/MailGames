using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase;

namespace TicTacToe
{
    public class TicTacToeAI : ABoardAI<TicTacToeState,Position>
    {
        public TicTacToeAI(int depth) : base(depth)
        {
        }

        protected override IEnumerable<Position> GetAllMoves(TicTacToeState state)
        {
            return TicTacToeLogic.AllInterrestingPositions(state).Where(p => state.Get(p) == null);
        }

        protected override GamePlayer GetCurrentPlayer(TicTacToeState state)
        {
            return state.CurrentPlayer;
        }

        protected override WinnerState? GetWinnerState(TicTacToeState state)
        {
            return TicTacToeLogic.GetWinner(state);
        }

        protected override float GetCurrentPlayerPoints(TicTacToeState state)
        {
            var points = 0;
            foreach (var line in TicTacToeLogic.AllInterrestingLines(state))
            {
                if (line.Count == 0) continue;

                var linePoints = (int) Math.Pow(10, line.Count);
                if (line.CurrentPlayer == state.CurrentPlayer) points += linePoints;
                else points -= linePoints;
            }
            return points;
        }

        protected override TicTacToeState GetNewState(TicTacToeState state, Position move)
        {
            var newState = new TicTacToeState(state);
            newState.Play(move.X, move.Y);
            return newState;
        }
    }
}
