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
    public class CleftMaskCardController : NagualHandCheckCardController
    {
        public CleftMaskCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of your turn, {NagualCharacter} may deal himself 3 psychic damage. If no damage is taken this way, destroy this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageToSomeoneOrDestroyThisResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Draw 3 cards."
            IEnumerator drawCoroutine = DrawCards(base.HeroTurnTakerController, 3);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            // "You may discard any number of cards."
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            if (base.HeroTurnTaker.HasCardsInHand)
            {
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.DecisionMaker, null, requiredDecisions: 0, storedResults: discardResults, allowAutoDecide: true, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
            }
            else
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " has no cards in hand to discard.", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
            }
            // "Nagual deals a target X radiant damage, where X = the number of cards discarded this way."
            IEnumerator radiantCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), GetNumberOfCardsDiscarded(discardResults), DamageType.Radiant, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(radiantCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(radiantCoroutine);
            }
        }

        private IEnumerator DealDamageToSomeoneOrDestroyThisResponse(PhaseChangeAction pca)
        {
            // "{NagualCharacter} may deal himself 3 psychic damage."
            List<DealDamageAction> damageResults = new List<DealDamageAction>();
            IEnumerator damageCoroutine = base.GameController.DealDamage(base.DecisionMaker, base.CharacterCard, (Card c) => c == base.CharacterCard, 3, DamageType.Psychic, optional: true, storedResults: damageResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "If no damage is taken this way, destroy this card."
            if (!DidDealDamage(damageResults))
            {
                IEnumerator destructCoroutine = base.GameController.DestroyCard(base.DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destructCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destructCoroutine);
                }
            }
        }
    }
}
