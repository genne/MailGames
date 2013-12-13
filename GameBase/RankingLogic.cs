using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase
{
    public static class RankingLogic
    {
        public static float DefaultRanking = 1500;

        public static float GetRankingUpdateWhenWinning(float winningPlayerRanking, float loosingPlayerRanking)
        {
            var diff = loosingPlayerRanking / winningPlayerRanking;
            return diff * 50;
        }

        public static float GetRankingUpdateWhenTie(float playerRanking, float otherPlayerRanking)
        {
            var diff = otherPlayerRanking - playerRanking;
            return diff / 4;
        }
    }
}
