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
    public class JaguarPeltCardController : NagualHandCheckCardController
    {
        public JaguarPeltCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int amt1 = GetPowerNumeral(0, 2);
            int amt2 = GetPowerNumeral(1, 2);
            // "{NagualCharacter} deals a target 2 melee damage."
            List<DealDamageAction> damageResults = new List<DealDamageAction>();
            IEnumerator firstCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), amt1, DamageType.Melee, 1, false, 1, storedResultsDamage: damageResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(firstCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(firstCoroutine);
            }
            // "If you have no cards in hand, {NagualCharacter} deals another target 2 melee damage."
            if (base.HeroTurnTaker.Hand.IsEmpty)
            {
                IEnumerator secondCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), amt2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !damageResults.Select((DealDamageAction dda) => dda.Target).Contains(c), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(secondCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(secondCoroutine);
                }
            }
        }
    }
}
