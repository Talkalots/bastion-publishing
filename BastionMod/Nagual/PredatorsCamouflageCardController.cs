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
    public class PredatorsCamouflageCardController : NagualHandCheckCardController
    {
        public PredatorsCamouflageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Whenever {NagualCharacter} would be dealt damage, you may discard a card. If you do, reduce that damage by 1."
            AddTrigger((DealDamageAction dda) => dda.Amount > 0 && dda.Target == base.CharacterCard && base.HeroTurnTaker.HasCardsInHand, DiscardToReduceResponse, new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.DiscardCard, TriggerType.ReduceDamageLimited }, TriggerTiming.Before);
            // "If you have no cards in hand, reduce damage dealt to {NagualCharacter} by 1."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && !base.HeroTurnTaker.HasCardsInHand, (DealDamageAction dda) => 1);
        }

        private IEnumerator DiscardToReduceResponse(DealDamageAction dda)
        {
            // "... you may discard a card. If you do, reduce that damage by 1."
            if (base.HeroTurnTaker.HasCardsInHand)
            {
                List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.DecisionMaker, 1, true, 1, discardResults, gameAction: dda, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
                if (DidDiscardCards(discardResults))
                {
                    IEnumerator reduceCoroutine = base.GameController.ReduceDamage(dda, 1, null, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(reduceCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(reduceCoroutine);
                    }
                }
            }
        }
    }
}
