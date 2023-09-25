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
    public class SacrificialDaggerCardController : EstrangularRelicCardController
    {
        public SacrificialDaggerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show villain target with highest HP
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            // Show hero target with second highest HP
            SpecialStringMaker.ShowHeroTargetWithHighestHP(2);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Increase damage dealt by [i]Estrangular[/i] by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped, (DealDamageAction dda) => 1);
            // "At the end of the villain turn, the villain target with the highest HP deals the hero target with the second highest HP 2 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, MeleeDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator MeleeDamageResponse(PhaseChangeAction pca)
        {
            // "... the villain target with the highest HP..."
            List<Card> storedResults = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsVillainTarget(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card source = storedResults.FirstOrDefault();
            if (source != null)
            {
                // "... deals the hero target with the second highest HP 2 melee damage."
                IEnumerator damageCoroutine = DealDamageToHighestHP(source, 2, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
        }
    }
}
