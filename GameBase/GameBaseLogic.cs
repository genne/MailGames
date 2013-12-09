using System.Collections;
using System.Collections.Generic;

namespace GameBase
{
    public class GameBaseLogic
    {
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