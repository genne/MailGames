using System;
using System.Collections.Generic;
using GameBase;
using MailGames.Context;

namespace MailGames.Logic
{
    interface IGameBoardVisitor
    {
        GamePlayer GetCurrentPlayer();
        IEnumerable<DateTime> GetActivityDates();
        IGameBoard CreateBoard(MailGamesContext db);
        IGameBoard FindBoard(MailGamesContext db, Guid id);
        string GetName();
        string GetController();
        WinnerState? GetWinnerState();
        string GetWikipediaId();
    }
}