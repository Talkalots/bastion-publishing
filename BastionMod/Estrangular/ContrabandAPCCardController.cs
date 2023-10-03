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
    public class ContrabandAPCCardController : HumanSnakeUtilityCardController
    {
        public ContrabandAPCCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show H-2 non-Villain targets with highest HP
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(1, H - 2);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Increase damage dealt by [i]Estrangular[/i] to the {ContrabandAPC} by 2."
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.IsCard && dda.DamageSource.IsSameCard(base.CharacterCard) && base.CharacterCard.IsFlipped && dda.Target == base.Card, (DealDamageAction dda) => 2);
            // "{human} At the end of the villain turn, {ContrabandAPC} deals the {H - 2} non-Villain targets with the highest HP 3 projectile damage each."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HumanProjectileResponse, TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the end of the villain turn, {ContrabandAPC} deals [i]Estrangular[/i] 4 projectile damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, SnakeProjectileResponse, TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateSnake);
        }

        private IEnumerator HumanProjectileResponse(PhaseChangeAction pca)
        {
            // "... {ContrabandAPC} deals the {H - 2} non-Villain targets with the highest HP 3 projectile damage each."
            IEnumerator projectileCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => !IsVillainTarget(c), (Card c) => 3, DamageType.Projectile, numberOfTargets: HumanTargetsCount);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(projectileCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(projectileCoroutine);
            }
        }

        private int HumanTargetsCount()
        {
            if (ActivateHuman)
            {
                return H - 2;
            }
            else
            {
                return 0;
            }
        }

        private IEnumerator SnakeProjectileResponse(PhaseChangeAction pca)
        {
            // "... {ContrabandAPC} deals [i]Estrangular[/i] 4 projectile damage."
            IEnumerator projectileCoroutine = DealDamage(base.Card, base.CharacterCard, 4, DamageType.Projectile, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(projectileCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(projectileCoroutine);
            }
        }
    }
}
