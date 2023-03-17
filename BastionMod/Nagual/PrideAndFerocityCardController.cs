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
    public class PrideAndFerocityCardController : CardController
    {
        public PrideAndFerocityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "{NagualCharacter} deals a target 2 radiant damage."
            IEnumerator radiantCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Radiant, 1, false, 1, addStatusEffect: OnHeroDamageResponse, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(radiantCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(radiantCoroutine);
            }
        }

        private IEnumerator OnHeroDamageResponse(DealDamageAction dda)
        {
            // "If a hero was dealt damage this way, that hero's player may draw 2 cards."
            if (dda.Target.IsHeroCharacterCard && dda.Target.Owner.IsHero)
            {
                IEnumerator drawCoroutine = DrawCards(FindHeroTurnTakerController(dda.Target.Owner.ToHero()), 2, optional: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCoroutine);
                }
            }
            yield break;
        }
    }
}
