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
    public class DisgustingLeerCardController : EstrangularOneShotCardController
    {
        public DisgustingLeerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero with most cards in play
            SpecialStringMaker.ShowHeroWithMostCards(false);
            // Show hero with most cards in hand
            SpecialStringMaker.ShowHeroWithMostCards(true);
        }

        public override IEnumerator IconlessText()
        {
            yield break;
        }

        public override IEnumerator HumanText()
        {
            // "The hero with the most cards in play cannot use powers until the start of the next villain turn."
            List<TurnTaker> results = new List<TurnTaker>();
            IEnumerator findCoroutine = FindHeroWithMostCardsInPlay(results, evenIfCannotDealDamage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            if (results.Count() > 0)
            {
                TurnTaker mostCardsInPlay = results.First();
                CannotUsePowersStatusEffect debuff = new CannotUsePowersStatusEffect();
                debuff.TurnTakerCriteria.IsSpecificTurnTaker = mostCardsInPlay;
                debuff.UntilStartOfNextTurn(base.TurnTaker);
                IEnumerator statusCoroutine = AddStatusEffect(debuff);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(statusCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(statusCoroutine);
                }
            }
        }

        public override IEnumerator SnakeText()
        {
            // "The hero with the most cards in hand cannot play cards until the start of the next villain turn."
            List<TurnTaker> results = new List<TurnTaker>();
            IEnumerator findCoroutine = FindHeroWithMostCardsInHand(results, evenIfCannotDealDamage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            if (results.Count() >  0)
            {
                TurnTaker mostCardsInHand = results.First();
                CannotPlayCardsStatusEffect debuff = new CannotPlayCardsStatusEffect();
                debuff.TurnTakerCriteria.IsSpecificTurnTaker = mostCardsInHand;
                debuff.UntilStartOfNextTurn(base.TurnTaker);
                IEnumerator statusCoroutine = AddStatusEffect(debuff);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(statusCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(statusCoroutine);
                }
            }
        }
    }
}
