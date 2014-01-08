using System;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBaseTests
{
    [TestClass]
    public class GameBaseTests
    {
        [TestMethod]
        public void TestPositionToFromInt()
        {
            var pos = new Position(5, 6);
            Assert.AreEqual(pos, Position.FromInt(pos.ToInt()));
        }

        [TestMethod]
        public void TestPositionToFromIntNegative()
        {
            var pos = new Position(-1, -1);
            Assert.AreEqual(pos, Position.FromInt(pos.ToInt(1, -1), 1, -1));
        }

        [TestMethod]
        public void TestRankingWhenTie()
        {
            Assert.AreEqual(0, RankingLogic.GetRankingUpdateWhenTie(1500, 1500));
            Assert.IsTrue(RankingLogic.GetRankingUpdateWhenTie(1600, 1500) < 0);
            Assert.IsTrue(RankingLogic.GetRankingUpdateWhenTie(1500, 1600) > 0);
            Assert.AreEqual(-RankingLogic.GetRankingUpdateWhenTie(1600, 1500), RankingLogic.GetRankingUpdateWhenTie(1500, 1600));

            Assert.AreEqual(25, RankingLogic.GetRankingUpdateWhenTie(1500, 1600));
        }

        [TestMethod]
        public void TestWinningRankingWhenSameInitialRanking()
        {
            var diff = GetWinningLoosingRankings(1500, 1500);

            Assert.IsTrue(diff < 0);
            Assert.IsTrue(diff > -5);
        }

        [TestMethod]
        public void TestWinningRankingWhenHigherInitialRanking()
        {
            var diff = GetWinningLoosingRankings(1600, 1500);
            Assert.IsTrue(diff < -5);
        }

        [TestMethod]
        public void TestWinningRankingWhenLowerInitialRanking()
        {
            var diff = GetWinningLoosingRankings(1500, 1600);
            Assert.IsTrue(diff > 3);
        }

        [TestMethod]
        public void TestRankingWinning()
        {
            float prevDiff = 0;
            for (int i = -1000; i < 1000; i++)
            {
                var diff = RankingLogic.GetRankingUpdateWhenWinning(1500 - i, 1500);
                Assert.IsTrue(diff > prevDiff);
                prevDiff = diff;
            }
        }

        private static float GetWinningLoosingRankings(float r0, float r1)
        {
            var initialR0 = r0;
            var diff1 = RankingLogic.GetRankingUpdateWhenWinning(r0, r1);
            r0 += diff1;
            r1 -= diff1;
            var diff2 = RankingLogic.GetRankingUpdateWhenWinning(r1, r0);
            r0 -= diff2;
            r1 += diff2;

            return r0 - initialR0;
        }
    }
}
