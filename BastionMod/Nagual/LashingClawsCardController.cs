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
    public class LashingClawsCardController : NagualHandCheckCardController
    {
        public LashingClawsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstDamageThisTurn, base.CharacterCard.Title + " has already been dealt damage this turn since " + base.Card.Title + " entered play.", base.CharacterCard.Title + " has not been dealt damage this turn since " + base.Card.Title + " entered play.");
        }

        private const string FirstDamageThisTurn = "FirstDamageDealtToNagualThisTurn";

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "The first time {NagualCharacter} is dealt damage each turn, if you have no cards in hand, {NagualCharacter} deals 1 target 2 melee damage."
            AddTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && dda.DidDealDamage && !HasBeenSetToTrueThisTurn(FirstDamageThisTurn), DamageIfEmptyHandResponse, TriggerType.DealDamage, TriggerTiming.After);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstDamageThisTurn), TriggerType.Hidden);
            // "At the end of your turn, you may discard a card. If you do, one player draws a card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DiscardToGiveDrawResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DrawCard });
        }

        private IEnumerator DamageIfEmptyHandResponse(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction(FirstDamageThisTurn);
            // "... if you have no cards in hand, {NagualCharacter} deals 1 target 2 melee damage."
            String message = base.Card.Title + " reacts!";
            if (base.HeroTurnTaker.HasCardsInHand)
            {
                message = base.Card.Title + " reacts, but " + base.TurnTaker.Name + " has at least one card in hand, so he doesn't deal damage.";
            }
            IEnumerator messageCoroutine = base.GameController.SendMessageAction(message, Priority.High, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(messageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(messageCoroutine);
            }
            if (!base.HeroTurnTaker.HasCardsInHand)
            {
                IEnumerator meleeCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(meleeCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(meleeCoroutine);
                }
            }
        }

        private IEnumerator DiscardToGiveDrawResponse(PhaseChangeAction pca)
        {
            // "... you may discard a card."
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            IEnumerator discardCoroutine = base.GameController.SelectAndDiscardCard(base.DecisionMaker, optional: true, storedResults: discardResults, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCoroutine);
            }
            // "If you do, one player draws a card."
            if (DidDiscardCards(discardResults))
            {
                IEnumerator drawCoroutine = base.GameController.SelectHeroToDrawCard(base.DecisionMaker, optionalSelectHero: false, optionalDrawCard: false, cardSource: GetCardSource());
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
}
