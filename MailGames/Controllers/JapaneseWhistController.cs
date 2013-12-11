using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameBase;
using JapaneseWhist;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Logic;
using MailGames.Models;

namespace MailGames.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class JapaneseWhistController : GameControllerBase
    {
        //
        // GET: /JapaneseWhist/

        public ActionResult Game(Guid id)
        {
            var board = new MailGamesContext().JapaneseWhistBoards.Find(id);
            var state = JapaneseWhistConversion.GetState(board);
            var player = GameLogic.GetLoggedInPlayer(board);
            var opponent = GameBaseLogic.GetNextPlayer(player);
            var selectableCards = state.CurrentPlayer == player && state.GetCurrentTrumf() != null ? JapaneseWhistLogic.GetPlayableCardIndices(state).ToArray() : new PlayerDeckIndex[0];
            var model = new JapaneseWhistGameViewModel(board)
            {
                CanPlayFromHand = JapaneseWhistLogic.CanPlayFromHand(state),
                SelectableHandCards = selectableCards.Where(c => c.PlayerDeck == PlayerDeck.Hand).Select(c => c.Index).ToArray(),
                SelectableVisibleCards = selectableCards.Where(c => c.PlayerDeck == PlayerDeck.Visible).Select(c => c.Index).ToArray(),
                OpponentHandCards = state.GetPlayerDeck(opponent, PlayerDeck.Hand),
                OpponentHiddenCards = state.GetPlayerDeck(opponent, PlayerDeck.Hidden),
                OpponentVisibleCards = state.GetPlayerDeck(opponent, PlayerDeck.Visible),
                PlayerHandCards = state.GetPlayerDeck(player, PlayerDeck.Hand),
                PlayerHiddenCards = state.GetPlayerDeck(player, PlayerDeck.Hidden),
                PlayerVisibleCards = state.GetPlayerDeck(player, PlayerDeck.Visible),
                CurrentStick = state.CurrentStick,
                PlayerSticks = state.GetPlayerDeck(player, PlayerDeck.Sticks),
                OpponentSticks = state.GetPlayerDeck(opponent, PlayerDeck.Sticks),
                PlayerTotalScore = state.GetPoints(player),
                OpponentTotalScore = state.GetPoints(opponent),
                PlayerLastStick = state.GetPlayerDeck(player, PlayerDeck.LastStick),
                OpponentLastStick = state.GetPlayerDeck(opponent, PlayerDeck.LastStick),
                SelectTrumf = state.GetCurrentTrumf() == null,
                CurrentTrumf = state.GetCurrentTrumf()
            };
            return View(model);
        }

        public ActionResult Select(Guid id, PlayerDeck deck, int index)
        {
            var db = new MailGamesContext();
            var board = db.JapaneseWhistBoards.Find(id);
            JapaneseWhistLogic.Select(JapaneseWhistConversion.GetState(board), deck, index); // Verify action before saving
            board.Moves.Add(new JapaneseWhistMove
            {
                CardIndex = index,
                PlayerDeck = deck,
                DateTime = DateTime.Now
            });
            db.SaveChanges();

            SendOpponentMail(db, board);

            return RedirectToAction("Game", new {id});
        }

        public ActionResult SelectTrumf(Guid id, CardColor trumf)
        {
            var db = new MailGamesContext();
            var board = db.JapaneseWhistBoards.Find(id);
            JapaneseWhistLogic.SelectTrumf(JapaneseWhistConversion.GetState(board), trumf); // Validate
            board.Moves.Add(new JapaneseWhistMove
            {
                Trumf = trumf,
                DateTime = DateTime.Now
            });
            db.SaveChanges();

            return RedirectToAction("Game", new {id});
        }
    }
}
