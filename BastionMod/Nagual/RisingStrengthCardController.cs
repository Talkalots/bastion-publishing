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
    public class RisingStrengthCardController : CardController
    {
        public RisingStrengthCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of your turn, you may put a card from your hand under this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, ChooseAndMoveCardResponse, TriggerType.MoveCard);
        }

        private IEnumerator ChooseAndMoveCardResponse(PhaseChangeAction pca)
        {
            // "... you may put a card from your hand under this card."
            if (base.HeroTurnTaker.HasCardsInHand)
            {
                SelectCardDecision decision = new SelectCardDecision(base.GameController, base.DecisionMaker, SelectionType.MoveUnderThisCard, base.HeroTurnTaker.Hand.Cards, isOptional: true, cardSource: GetCardSource());
                IEnumerator moveCoroutine = base.GameController.SelectCardAndDoAction(decision, (SelectCardDecision scd) => base.GameController.MoveCard(base.TurnTakerController, scd.SelectedCard, base.Card.UnderLocation, cardSource: GetCardSource()));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCoroutine);
                }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Nagual deals himself and each non-hero target X melee damage, where X = the number of cards under this card."
            IEnumerator selfDamageCoroutine = DealDamage(base.CharacterCard, base.CharacterCard, base.Card.UnderLocation.Cards.Count(), DamageType.Melee, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selfDamageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selfDamageCoroutine);
            }
            IEnumerator enemyDamageCoroutine = DealDamage(base.CharacterCard, (Card c) => c.IsTarget && !c.IsHero, (Card c) => base.Card.UnderLocation.Cards.Count(), DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(enemyDamageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(enemyDamageCoroutine);
            }
            // "Put all cards under this card into your hand."
            IEnumerator handCoroutine = base.GameController.BulkMoveCards(base.TurnTakerController, base.Card.UnderLocation.Cards, base.HeroTurnTaker.Hand, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(handCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(handCoroutine);
            }
            // "Destroy this card."
            IEnumerator destructCoroutine = base.GameController.DestroyCard(base.DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destructCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destructCoroutine);
            }
        }
    }
}
