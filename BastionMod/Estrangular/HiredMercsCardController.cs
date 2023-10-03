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
    public class HiredMercsCardController : HumanSnakeUtilityCardController
    {
        public HiredMercsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show H-2 players with most cards in hand?? No SpecialStringMaker method for this...
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the end of the villain turn, this card deals the {H - 2} heroes with the most cards in hand 2 projectile damage each."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HumanProjectileResponse, TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the end of the villain turn, each player may draw a card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, SnakeDrawResponse, TriggerType.DrawCard, (PhaseChangeAction pca) => ActivateSnake);
        }

        private IEnumerator HumanProjectileResponse(PhaseChangeAction pca)
        {
            // "... this card deals the {H - 2} heroes with the most cards in hand 2 projectile damage each."
            // Select H-2 players with most cards in hand, save to storedResultsTurnTaker
            List<TurnTaker> storedResultsTurnTaker = new List<TurnTaker>();
            IEnumerator findPlayerCoroutine = FindHeroWithMostCardsInHand(storedResultsTurnTaker, 1, H - 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findPlayerCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findPlayerCoroutine);
            }
            // For each selected player, find a hero character to take damage, save to storedResultsCharacter
            List<Card> storedResultsCharacter = new List<Card>();
            foreach(TurnTaker player in storedResultsTurnTaker)
            {
                IEnumerator findCharacterCoroutine = FindCharacterCardToTakeDamage(player, storedResultsCharacter, base.Card, 2, DamageType.Projectile);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(findCharacterCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(findCharacterCoroutine);
                }
            }
            // Deal damage to selected characters
            IEnumerator projectileCoroutine = DealDamage(base.Card, (Card c) => storedResultsCharacter.Contains(c), (Card c) => 2, DamageType.Projectile, dynamicNumberOfTargets: HumanTargetsCount);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(projectileCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(projectileCoroutine);
            }
        }

        private int HumanTargetsCount()
        {
            if (ActivateHuman)
            {
                return 999;
            }
            else
            {
                return 0;
            }
        }

        private IEnumerator SnakeDrawResponse(PhaseChangeAction pca)
        {
            // "... each player may draw a card."
            IEnumerator drawCoroutine = EachPlayerDrawsACard(optional: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            // ...
        }
    }
}
