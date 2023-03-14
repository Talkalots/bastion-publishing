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
    public class ScourTheLandCardController : CardController
    {
        public ScourTheLandCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Destroy an environment card."
            IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
            // "{NagualCharacter} deals 1 target 2 melee damage."
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
    }
}
