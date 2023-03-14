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
    public class PounceCardController : CardController
    {
        public PounceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "{NagualCharacter} deals a target 2 melee damage. Reduce damage dealt by that target by 1 until the start of your next turn."
            IEnumerator meleeCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, 1, false, 1, addStatusEffect: (DealDamageAction dda) => ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, 1), selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(meleeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(meleeCoroutine);
            }
        }
    }
}
