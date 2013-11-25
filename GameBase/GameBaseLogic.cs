namespace GameBase
{
    public class GameBaseLogic
    {
        public static GamePlayer GetNextPlayer(GamePlayer player)
        {
            return player == GamePlayer.FirstPlayer ? GamePlayer.SecondPlayer : GamePlayer.FirstPlayer;
        }
    }
}