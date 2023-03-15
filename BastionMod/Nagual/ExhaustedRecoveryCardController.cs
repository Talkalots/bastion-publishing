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
    public class ExhaustedRecoveryCardController : CardController
    {
        public ExhaustedRecoveryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Draw 2 cards."
            IEnumerator drawCoroutine = DrawCards(base.DecisionMaker, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            // "If {NagualCharacter} has fewer than 10 HP, he regains 3 HP."
            if (base.CharacterCard.IsTarget && base.CharacterCard.HitPoints.HasValueLessThan(10))
            {
                IEnumerator healCoroutine = base.GameController.GainHP(base.CharacterCard, 3, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(healCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(healCoroutine);
                }
            }
        }
    }
}
