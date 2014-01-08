using System;
using System.Collections;
using System.Collections.Generic;

namespace GameBase
{
    public class GameBaseLogic
    {
        public static GamePlayer? GetWinner(WinnerState winnerState)
        {
            switch (winnerState)
            {
                case WinnerState.FirstPlayer:
                    return GamePlayer.FirstPlayer;
                case WinnerState.SecondPlayer:
                    return GamePlayer.SecondPlayer;
                case WinnerState.Tie:
                    return null;
                case WinnerState.FirstPlayerResigned:
                    return GamePlayer.SecondPlayer;
                    break;
                case WinnerState.SecondPlayerResigned:
                    return GamePlayer.FirstPlayer;
                    break;
                case WinnerState.FirstPlayerPassive:
                    return GamePlayer.SecondPlayer;
                    break;
                case WinnerState.SecondPlayerPassive:
                    return GamePlayer.FirstPlayer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("winnerState");
            }
        }

        public static GamePlayer GetNextPlayer(GamePlayer player)
        {
            return player == GamePlayer.FirstPlayer ? GamePlayer.SecondPlayer : GamePlayer.FirstPlayer;
        }

        public static IEnumerable<GamePlayer> GetAllPlayers()
        {
            yield return GamePlayer.FirstPlayer;
            yield return GamePlayer.SecondPlayer;
        }
    }
}