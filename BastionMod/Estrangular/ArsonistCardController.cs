using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bastion.Estrangular
{
    public class ArsonistCardController : HumanSnakeUtilityCardController
    {
        public ArsonistCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show H-1 non-villain targets with highest HP
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(1, H - 1);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the start of the villain turn, [i]Arsonist[/i] deals the {H - 1} non-villain targets with the highest HP 2 fire damage each."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HumanFireResponse, TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the end of the villain turn, [i]Arsonist[/i] deals [i]Estrangular[/i] 4 fire damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, SnakeFireResponse, TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateSnake);
        }

        private IEnumerator HumanFireResponse(PhaseChangeAction pca)
        {
            // "... [i]Arsonist[/i] deals the {H - 1} non-villain targets with the highest HP 2 fire damage each."
            IEnumerator fireCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => !IsVillainTarget(c), (Card c) => 2, DamageType.Fire, numberOfTargets: () => H - 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(fireCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(fireCoroutine);
            }
        }

        private IEnumerator SnakeFireResponse(PhaseChangeAction pca)
        {
            // "... [i]Arsonist[/i] deals [i]Estrangular[/i] 4 fire damage."
            IEnumerator fireCoroutine = DealDamage(base.Card, base.CharacterCard, 4, DamageType.Fire, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(fireCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(fireCoroutine);
            }
        }
    }
}
