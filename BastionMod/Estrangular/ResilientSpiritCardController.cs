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
    public class ResilientSpiritCardController : HumanSnakeUtilityCardController
    {
        public ResilientSpiritCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show whether redirection has been used this turn
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstRicoDamageThisTurn, base.Card.Title + " can no longer redirect damage from [i]Rico Homem[/i] this turn.", base.Card.Title + " has not redirected damage from [i]Rico Homem[/i] this turn.").Condition = () => base.Card.IsInPlayAndHasGameText;
            // Show non-villain target with lowest HP
            SpecialStringMaker.ShowNonVillainTargetWithLowestHP();
            // Show whether retaliation has been used this turn
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstMonsterDamageThisTurn, base.Card.Title + " has already responded to damage dealt to [i]Estrangular[/i] this turn.", base.Card.Title + " has not responded to damage dealt to [i]Estrangular[/i] this turn.").Condition = () => base.Card.IsInPlayAndHasGameText;
        }

        protected readonly string FirstRicoDamageThisTurn = "FirstRicoDamageThisTurn";
        protected readonly string FirstMonsterDamageThisTurn = "FirstMonsterDamageThisTurn";

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} The first time [i]Rico Homem[/i] would be dealt damage each turn, redirect that damage to the non-villain target with the lowest HP."
            AddTrigger((DealDamageAction dda) => !HasBeenSetToTrueThisTurn(FirstRicoDamageThisTurn) && dda.Target == base.CharacterCard && !base.CharacterCard.IsFlipped, IfHumanRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstRicoDamageThisTurn), TriggerType.Hidden);
            // "{snake} The first time [i]Estrangular[/i] would be dealt damage each turn, he deals the source of that damage {H - 2} melee damage."
            AddTrigger((DealDamageAction dda) => !HasBeenSetToTrueThisTurn(FirstMonsterDamageThisTurn) && !dda.IsPretend && dda.Amount > 0 && dda.Target == base.CharacterCard && base.CharacterCard.IsFlipped, IfSnakeRetaliateResponse, TriggerType.DealDamage, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstMonsterDamageThisTurn), TriggerType.Hidden);
        }

        private IEnumerator IfHumanRedirectResponse(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction(FirstRicoDamageThisTurn);
            if (ActivateHuman)
            {
                // "... redirect that damage to the non-villain target with the lowest HP."
                IEnumerator redirectCoroutine = RedirectDamage(dda, TargetType.LowestHP, (Card c) => !IsVillainTarget(c));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(redirectCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(redirectCoroutine);
                }
            }
            yield break;
        }

        private IEnumerator IfSnakeRetaliateResponse(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction(FirstMonsterDamageThisTurn);
            if (ActivateSnake)
            {
                if (dda.DamageSource != null && dda.DamageSource.IsTarget)
                {
                    // "... he deals the source of that damage {H - 2} melee damage."
                    IEnumerator meleeCoroutine = DealDamage(base.CharacterCard, dda.DamageSource.Card, H - 2, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource());
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
            yield break;
        }
    }
}
