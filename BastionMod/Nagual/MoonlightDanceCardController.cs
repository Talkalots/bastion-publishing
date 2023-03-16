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
    public class MoonlightDanceCardController : NagualHandCheckCardController
    {
        public MoonlightDanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "If you have no cards in hand, increase melee damage dealt by {NagualCharacter} by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsSameCard(base.CharacterCard) && dda.DamageType == DamageType.Melee && !base.HeroTurnTaker.HasCardsInHand, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numRevealed = GetPowerNumeral(0, 2);
            // "Reveal the top 2 cards of your deck. Put any Ongoing cards revealed this way into play. Shuffle the other revealed cards back into your deck."
            IEnumerator revealCoroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsOngoing, "Ongoing"), null, numRevealed, revealedCardDisplay: RevealedCardDisplay.Message);
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
