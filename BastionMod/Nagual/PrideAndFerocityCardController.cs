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
            List<DealDamageAction> damageResults = new List<DealDamageAction>();
            IEnumerator radiantCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Radiant, 1, false, 1, storedResultsDamage: damageResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(radiantCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(radiantCoroutine);
            }
            // "If a hero was dealt damage this way, that hero's player may draw 2 cards."
            List<TurnTaker> relevantPlayers = (from dda in damageResults where dda != null && dda.DidDealDamage && dda.Target.IsHeroCharacterCard && dda.Target.Owner.IsHero select dda.Target.Owner).Distinct().ToList();
            IEnumerator drawCoroutine = base.GameController.SelectHeroToDrawCards(base.DecisionMaker, 2, optionalSelectHero: true, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => relevantPlayers.Contains(tt)), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
        }
    }
}
