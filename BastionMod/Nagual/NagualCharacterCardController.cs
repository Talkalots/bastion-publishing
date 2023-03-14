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
    public class NagualCharacterCardController : HeroCharacterCardController
    {
        public NagualCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Discard three cards."
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            IEnumerator discardCoroutine = SelectAndDiscardCards(base.DecisionMaker, 3, requiredDecisions: 3, storedResults: discardResults, allowAutoDecide: base.HeroTurnTaker != null && base.HeroTurnTaker.Hand.NumberOfCards <= 3, responsibleTurnTaker: base.TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCoroutine);
            }
            // "If you do, you may put a card from your trash into play."
            if (DidDiscardCards(discardResults, 3))
            {
                IEnumerator putCoroutine = SearchForCards(base.DecisionMaker, false, true, 0, 1, new LinqCardCriteria((Card c) => true), true, false, false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(putCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(putCoroutine);
                }
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapCoroutine;
            switch (index)
            {
                case 0:
                    incapCoroutine = UseIncapOption1();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 1:
                    incapCoroutine = UseIncapOption2();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 2:
                    incapCoroutine = UseIncapOption3();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
            }
            yield break;
        }

        private IEnumerator UseIncapOption1()
        {
            // "One hero regains 2 HP."
            yield return base.GameController.SelectAndGainHP(base.DecisionMaker, 2, false, (Card c) => c.IsHeroCharacterCard, 1, 1, cardSource: GetCardSource());
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero may use a power."
            yield return base.GameController.SelectHeroToUsePower(base.DecisionMaker, cardSource: GetCardSource());
        }

        private IEnumerator UseIncapOption3()
        {
            // "Destroy an environment card."
            yield return base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, responsibleCard: base.Card, cardSource: GetCardSource());
        }
    }
}
