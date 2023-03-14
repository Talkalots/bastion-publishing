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
    public class ProwlUnseenCardController : CardController
    {
        public ProwlUnseenCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Reveal the top two cards of the villain deck. Put 1 of them on top of the deck and the other on the bottom."
            List<SelectLocationDecision> locationResults = new List<SelectLocationDecision>();
            IEnumerator findCoroutine = FindVillainDeck(base.DecisionMaker, SelectionType.RevealCardsFromDeck, locationResults, (Location l) => true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Location deck = GetSelectedLocation(locationResults);
            List<Card> revealedCards = new List<Card>();
            if (deck != null)
            {
                IEnumerator manipCoroutine = RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(base.DecisionMaker, base.TurnTakerController, deck, 2, 1, 1, revealedCards);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(manipCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(manipCoroutine);
                }
                List<Location> sources = new List<Location>();
                sources.Add(deck.OwnerTurnTaker.Revealed);
                IEnumerator cleanupCoroutine = CleanupCardsAtLocations(sources, deck, cardsInList: revealedCards);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(cleanupCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(cleanupCoroutine);
                }
            }
            // "You may draw a card or play a card."
            IEnumerator drawPlayCoroutine = DrawACardOrPlayACard(base.DecisionMaker, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawPlayCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawPlayCoroutine);
            }
        }
    }
}
