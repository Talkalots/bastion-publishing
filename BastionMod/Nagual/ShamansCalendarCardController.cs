using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bastion.Nagual
{
    public class ShamansCalendarCardController : CardController
    {
        public ShamansCalendarCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        private const string HasDrawnCardsThisPhase = "NagualHasDrawnCardsThisPhase";

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "You may skip your draw phase. If you do, reveal the top card of your deck, then either put it into play or discard it."
            // To make this run smoothly even with the "automatically draw cards when safe to do so" option enabled:
            // 1) when Nagual draws a card, if it's his draw phase and HasDrawnCardsThisPhase is false, set HasDrawnCardsThisPhase to true
            AddTrigger((DrawCardAction dca) => dca.HeroTurnTaker == base.HeroTurnTaker && base.GameController.ActiveTurnTaker == base.TurnTaker && base.GameController.ActiveTurnPhase.IsDrawCard && !GetCardPropertyJournalEntryBoolean(HasDrawnCardsThisPhase).GetValueOrDefault(), SetTrueResponse, TriggerType.Other, TriggerTiming.After);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(HasDrawnCardsThisPhase), TriggerType.Hidden);
            // 2) at the end of Nagual's draw phase...
            // ... if HasDrawnCardsThisPhase is false, reveal the top card of his deck and put it into play or discard it
            // ... set HasDrawnCardsThisPhase to false
            AddTrigger((PhaseChangeAction pca) => pca.FromPhase.IsDrawCard && pca.FromPhase.TurnTaker == base.TurnTaker, RevealDiscardOrPlayResetResponse, new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.DiscardCard, TriggerType.Other }, TriggerTiming.After);
        }

        private IEnumerator SetTrueResponse(DrawCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(HasDrawnCardsThisPhase);
            yield break;
        }

        private IEnumerator RevealDiscardOrPlayResetResponse(PhaseChangeAction pca)
        {
            if (!(GetCardPropertyJournalEntryBoolean(HasDrawnCardsThisPhase).GetValueOrDefault()))
            {
                // "... reveal the top card of your deck, then either put it into play or discard it."
                IEnumerator revealCoroutine = RevealCard_PlayItOrDiscardIt(base.TurnTakerController, base.TurnTaker.Deck, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, isDiscard: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(revealCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(revealCoroutine);
                }
            }
            SetCardProperty(HasDrawnCardsThisPhase, false);
            yield break;
        }
    }
}
