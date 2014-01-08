using System.Collections.Generic;
using System.Linq;
using GameBase;

namespace Othello
{
    public class OthelloAI : ABoardAI<OthelloState, Position>
    {
        public OthelloAI(int depth) : base(depth)
        {
        }

        protected override IEnumerable<Position> GetAllMoves(OthelloState state)
        {
            return OthelloLogic.GetValidTargets(state);
        }

        protected override GamePlayer GetCurrentPlayer(OthelloState state)
        {
            return state.CurrentPlayer;
        }

        protected override WinnerState? GetWinnerState(OthelloState state)
        {
            return OthelloLogic.GetWinner(state);
        }

        protected override float GetCurrentPlayerPoints(OthelloState state)
        {
            var points = 0;
            foreach (var player in OthelloLogic.GetAllPositions().Select(state.Get).Where(player => player.HasValue))
            {
                if (player.Value == state.CurrentPlayer)
                    points++;
                else
                    points--;
            }
            return points;
        }

        protected override OthelloState GetNewState(OthelloState state, Position move)
        {
            var newState = new OthelloState(state);
            OthelloLogic.Play(newState, move);
            return newState;
        }
    }
}