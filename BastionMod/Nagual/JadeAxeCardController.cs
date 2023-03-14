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
    public class JadeAxeCardController : CardController
    {
        public JadeAxeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numTargets = GetPowerNumeral(0, 1);
            int meleeAmt = GetPowerNumeral(1, 1);
            // "Draw a card or discard a card."
            List<Function> options = new List<Function>();
            Function draw = new Function(base.DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(base.HeroTurnTaker), CanDrawCards(base.DecisionMaker), base.TurnTaker.NameRespectingVariant + " has no cards in hand and must draw a card.", "Draw a card");
            options.Add(draw);
            Function discard = new Function(base.DecisionMaker, "Discard a card", SelectionType.DiscardCard, () => SelectAndDiscardCards(base.DecisionMaker, 1, false, 1, responsibleTurnTaker: base.TurnTaker), base.HeroTurnTaker != null && base.HeroTurnTaker.HasCardsInHand, base.TurnTaker.NameRespectingVariant + " cannot draw cards and must discard a card.", "Discard a card");
            options.Add(discard);
            if (options.Count > 0)
            {
                SelectFunctionDecision choice = new SelectFunctionDecision(base.GameController, base.DecisionMaker, options, false, noSelectableFunctionMessage: base.TurnTaker.NameRespectingVariant + " cannot draw or discard cards.", cardSource: GetCardSource());
                IEnumerator chooseCoroutine = base.GameController.SelectAndPerformFunction(choice);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseCoroutine);
                }
            }
            // "{NagualCharacter} deals 1 target 1 melee damage."
            IEnumerator meleeCoroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), meleeAmt, DamageType.Melee, numTargets, false, numTargets, cardSource: GetCardSource());
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
}
