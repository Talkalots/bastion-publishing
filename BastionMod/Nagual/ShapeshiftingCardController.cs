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
    public class ShapeshiftingCardController : NagualHandCheckCardController
    {
        public ShapeshiftingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            if (base.HeroTurnTaker.Hand.IsEmpty)
            {
                // "If you have no cards in hand, draw 3 cards."
                IEnumerator drawCoroutine = DrawCards(base.HeroTurnTakerController, 3);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCoroutine);
                }
            }
            else
            {
                // "Otherwise, discard any number of cards."
                List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.HeroTurnTakerController, null, requiredDecisions: 0, storedResults: discardResults, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
                int numOngoingCards = discardResults.Count();
                // "Reveal cards from your deck until X Ongoing cards are revealed, where X = the number of cards you discarded. Put one of them into play. Shuffle the other revealed cards back into your deck."
                IEnumerator revealCoroutine = RevealCards_SelectSome_MoveThem_ReturnTheRest(base.DecisionMaker, base.TurnTakerController, base.TurnTaker.Deck, (Card c) => c.IsOngoing, numOngoingCards, 1, false, true, false, "Ongoing cards");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(revealCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(revealCoroutine);
                }
            }
        }
    }
}
