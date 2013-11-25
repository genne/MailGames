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
    }
}
