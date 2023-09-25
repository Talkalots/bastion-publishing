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
    public class TreacherousAmbushCardController : EstrangularOneShotCardController
    {
        public TreacherousAmbushCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero target with lowest HP
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
            // Show 2 hero targets with lowest HP
            SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 2);
        }

        public override IEnumerator IconlessText()
        {
            yield break;
        }

        public override IEnumerator HumanText()
        {
            // "[i]Rico Homem[/i] deals the hero target with the lowest HP 3 projectile damage."
            IEnumerator projectileCoroutine = DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => 3, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(projectileCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(projectileCoroutine);
            }
            // "Each villain target regains {H - 1} HP."
            IEnumerator healCoroutine = base.GameController.GainHP(DecisionMaker, (Card c) => IsVillainTarget(c), H - 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(healCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(healCoroutine);
            }
        }

        public override IEnumerator SnakeText()
        {
            if (base.CharacterCard.IsFlipped)
            {
                // "[i]Estrangular[/i] deals the 2 hero targets with the lowest HP {H - 2} toxic damage each."
                IEnumerator toxicCoroutine = DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H - 2, DamageType.Toxic, numberOfTargets: 2);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(toxicCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(toxicCoroutine);
                }
            }
            else
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction("[i]Estrangular[/i] is not in play.", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
            }
            // "Play the top card of the villain deck."
            IEnumerator playCoroutine = base.GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
        }
    }
}
