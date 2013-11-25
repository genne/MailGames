using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess;
using MailGames.Context;

namespace MailGames
{
    public class ChessConversion
    {
        internal static ChessState GetCurrentState(ChessBoard chessBoard)
        {
            var state = ChessLogic.CreateInitialChessState();
            foreach (var move in chessBoard.ChessMoves)
            {
                ChessLogic.ApplyMove(state, move.From, move.To, move.PawnConversion.Select(p => (PieceType?)p.ConvertTo).FirstOrDefault());
            }
            if (chessBoard.ChessMoves.Any())
            {
                var move = chessBoard.ChessMoves.Last();
            }
            return state;
        }


    }
}