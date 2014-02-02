using GameBase;
using MailGames.Context;
using Othello;

namespace MailGames.Logic
{
    public class OthelloConversion
    {
        public static OthelloState GetCurrentState(OthelloBoard othelloBoard)
        {
            var state = new OthelloState();
            foreach (var move in othelloBoard.Moves)
            {
                OthelloLogic.Play(state, Position.FromInt(move.Position));
            }
            return state;
        }
    }
}