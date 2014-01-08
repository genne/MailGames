using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class ChessAITests
    {
        [TestMethod]
        public void TestPawnAI()
        {
            var state = new ChessState();
            state.SetCell(0, 7, GamePlayer.FirstPlayer, PieceType.King);
            state.SetCell(1, 6, GamePlayer.FirstPlayer, PieceType.Pawn);

            state.SetCell(0, 0, GamePlayer.SecondPlayer, PieceType.King);
            state.SetCell(0, 3, GamePlayer.SecondPlayer, PieceType.Pawn);

            var moves = new ChessAI().GetBestMoves(state).ToArray();
            CollectionAssert.DoesNotContain(moves, new ChessAI.Move{ From = new Position(1, 6).ToInt(), To = new Position(1, 4).ToInt()});
        }

        [TestMethod]
        public void TestGameOver()
        {
            var state = new ChessState();
            state.SetCell(0, 0, GamePlayer.FirstPlayer, PieceType.King);

            state.SetCell(2, 2, GamePlayer.SecondPlayer, PieceType.King);
            state.SetCell(1, 4, GamePlayer.SecondPlayer, PieceType.Queen);

            state.CurrentPlayer = GamePlayer.SecondPlayer;

            var moves = new ChessAI().GetBestMovesWithPoints(state, new ABoardAI<ChessState, ChessAI.Move>.DepthCounter(2)).ToArray();
            Assert.AreEqual(float.MaxValue, moves.Max(m => m.Points));
        }

        [TestMethod]
        public void TestPawnProgressionPoints()
        {
            var winner = RunAIGames(10, new ChessAI(new ChessPointsSettings{ PawnProgressionPoints = 0.1f}, depth: 1), new ChessAI(depth: 1));
            Assert.AreEqual(GamePlayer.FirstPlayer, winner);
        }

        private GamePlayer RunAIGames(int numGames, ChessAI firstPlayerAI, ChessAI secondPlayerAI)
        {
            var playerPoints = new[] {0, 0};
            for (int i = 0; i < numGames; i++)
            {
                Debug.WriteLine("New game");
                Debug.WriteLine("--------------");
                var winner = RunAIGame(firstPlayerAI, secondPlayerAI);
                if (winner.HasValue)
                    playerPoints[(int) winner.Value]++;
                Debug.WriteLine("Winner: " + winner);
            }
            return playerPoints[0] > playerPoints[1] ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
        }

        private static GamePlayer? RunAIGame(ChessAI firstPlayerAI, ChessAI secondPlayerAI)
        {
            var state = ChessLogic.CreateInitialChessState();
            for (int i = 0; i < 1000; i++)
            {
                bool isCheck;
                var winnerState = ChessLogic.GetWinnerState(state, out isCheck);
                if (winnerState.HasValue) return GameBaseLogic.GetWinner(winnerState.Value);
                var currentPlayerAI = state.CurrentPlayer == GamePlayer.FirstPlayer ? firstPlayerAI : secondPlayerAI;
                var bestMove = currentPlayerAI.GetRandomBestMove(state);
                Debug.WriteLine(ChessLogic.GetProgress(state, GamePlayer.FirstPlayer) + ": " + bestMove);
                ChessLogic.ApplyMove(state, bestMove.From, bestMove.To, bestMove.ConvertPawnTo);
            }
            return null;
        }
    }
}
