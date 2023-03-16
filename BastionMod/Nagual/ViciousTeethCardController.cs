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
    public class ViciousTeethCardController : NagualHandCheckCardController
    {
        public ViciousTeethCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of your turn, if you have no cards in hand, {NagualCharacter} deals a target 2 melee damage. Otherwise, you may discard up to 3 cards."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DamageOrDiscardResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DiscardCard });
            // ...
        }

        private IEnumerator DamageOrDiscardResponse(PhaseChangeAction pca)
        {
            // "... if you have no cards in hand, {NagualCharacter} deals a target 2 melee damage."
            if (!base.HeroTurnTaker.HasCardsInHand)
            {
                IEnumerator meleeCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(meleeCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(meleeCoroutine);
                }
            }
            else
            {
                // "Otherwise, you may discard up to 3 cards."
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.DecisionMaker, 3, requiredDecisions: 0, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
            }
        }
    }
}
