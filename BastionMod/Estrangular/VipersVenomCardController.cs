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
    public class VipersVenomCardController : HumanSnakeUtilityCardController
    {
        public VipersVenomCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show whether damage has been dealt by a villain target yet this turn
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstVillainDamageThisTurn, "A villain target has already dealt damage this turn since " + base.Card.Title + " entered play.", "No villain targets have dealt damage this turn since " + base.Card.Title + " entered play.").Condition = () => base.Card.IsInPlayAndHasGameText;
        }

        protected readonly string FirstVillainDamageThisTurn = "FirstVillainDamageThisTurn";

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} The first time any villain target deals damage each turn, increase that damage by 2 and change its type to toxic."
            AddTrigger((DealDamageAction dda) => !HasBeenSetToTrueThisTurn(FirstVillainDamageThisTurn) && dda.DamageSource != null && dda.DamageSource.IsVillainTarget, IfHumanIncreaseModifyResponse, new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.ChangeDamageType }, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstVillainDamageThisTurn), TriggerType.Hidden);
            // "{snake} Change all damage dealt by [i]Estrangular[/i] to Toxic. Increase damage dealt by [i]Estrangular[/i] by 1."
            AddChangeDamageTypeTrigger((DealDamageAction dda) => ActivateSnake && dda.DamageSource != null && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped, DamageType.Toxic);
            AddIncreaseDamageTrigger((DealDamageAction dda) => ActivateSnake && dda.DamageSource != null && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped, (DealDamageAction dda) => 1);
        }

        private IEnumerator IfHumanIncreaseModifyResponse(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction(FirstVillainDamageThisTurn, gameAction: dda);
            if (ActivateHuman)
            {
                // "{human} ... increase that damage by 2..."
                IEnumerator increaseCoroutine = base.GameController.IncreaseDamage(dda, (DealDamageAction d) => 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(increaseCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(increaseCoroutine);
                }
                // "... and change its type to toxic."
                IEnumerator changeCoroutine = base.GameController.ChangeDamageType(dda, DamageType.Toxic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(changeCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(changeCoroutine);
                }
            }
            yield break;
        }
    }
}
