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
    public class ReplenishedByRainCardController : NagualHandCheckCardController
    {
        public ReplenishedByRainCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of your turn, if you have no cards in hand, {NagualCharacter} regains 1 HP. Otherwise, you may discard a card and have {NagualCharacter} deal a target 2 radiant damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HealOrDiscardForDamageResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.DiscardCard, TriggerType.DealDamage });
        }

        private IEnumerator HealOrDiscardForDamageResponse(PhaseChangeAction pca)
        {
            // "... if you have no cards in hand, {NagualCharacter} regains 1 HP."
            if (!base.HeroTurnTaker.HasCardsInHand)
            {
                IEnumerator healCoroutine = base.GameController.GainHP(base.CharacterCard, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(healCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(healCoroutine);
                }
            }
            else
            {
                // "Otherwise, you may discard a card..."
                List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.DecisionMaker, 1, true, 1, discardResults, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
                if (DidDiscardCards(discardResults))
                {
                    // "... to have {NagualCharacter} deal a target 2 radiant damage."
                    IEnumerator radiantCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Radiant, 1, false, 1, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(radiantCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(radiantCoroutine);
                    }
                }
            }
        }
    }
}
