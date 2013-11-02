using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;
using MailGames.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class PawnConversionTests
    {
        [TestMethod]
        public void TestPawnConversion()
        {
            var state = new ChessState();
            var @from = new Position{ Row = 1, Col = 0}.ToInt();
            var to = new Position{ Row = 0, Col = 0}.ToInt();
            state.SetCell(from, new Piece{ PieceColor = PieceColor.White, PieceType = PieceType.Pawn});
            const PieceType pawnConversion = PieceType.Rook;
            ChessLogic.ApplyMove(state, from, to, pawnConversion);

            Assert.AreEqual(pawnConversion, state.GetCell(to).PieceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPawnConversionWhenInvalid()
        {
            var state = new ChessState();
            var @from = new Position { Row = 1, Col = 0 }.ToInt();
            var to = new Position { Row = 0, Col = 0 }.ToInt();
            state.SetCell(from, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.Pawn });
            ChessLogic.ApplyMove(state, from, to, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPawnConversionWhenInvalid2()
        {
            var state = new ChessState();
            var @from = new Position { Row = 1, Col = 0 }.ToInt();
            var to = new Position { Row = 0, Col = 0 }.ToInt();
            state.SetCell(from, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.Rook });
            const PieceType pawnConversion = PieceType.Rook;
            ChessLogic.ApplyMove(state, from, to, pawnConversion);
        }
    }
}
