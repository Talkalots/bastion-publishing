﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bastion.Estrangular
{
    public class RapidEscalationCardController : EstrangularOneShotCardController
    {
        public RapidEscalationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero target with highest HP
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator IconlessText()
        {
            yield break;
        }

        public override IEnumerator HumanText()
        {
            // "Shuffle the villain trash into the villain deck."
            IEnumerator reshuffleCoroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(reshuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(reshuffleCoroutine);
            }
            if (ActivateHuman)
            {
                // "Play the top 2 cards of the villain deck."
                IEnumerator playCoroutine = base.GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCoroutine);
                }
                if (ActivateHuman)
                {
                    IEnumerator playNextCoroutine = base.GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(playNextCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(playNextCoroutine);
                    }
                }
            }
        }

        public override IEnumerator SnakeText()
        {
            if (base.CharacterCard.IsFlipped)
            {
                // "[i]Estrangular[/i] deals the hero target with the highest HP {H} melee damage..."
                IEnumerator meleeCoroutine = DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(meleeCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(meleeCoroutine);
                }
                // "... and regains {H * 2} HP."
                IEnumerator healCoroutine = base.GameController.GainHP(base.CharacterCard, H * 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(healCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(healCoroutine);
                }
            }
            else
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction("[i]Estrangular[/i] is not in play.", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
            }
            // "Shuffle the villain trash into the villain deck."
            IEnumerator reshuffleCoroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(reshuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(reshuffleCoroutine);
            }
        }
    }
}
