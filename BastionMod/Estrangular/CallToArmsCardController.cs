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
    public class CallToArmsCardController : EstrangularOneShotCardController
    {
        public CallToArmsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            HasIconless = true;
            // Show list of Minions in the villain trash
            SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain(MinionKeyword), "", singular: "Minion", plural: "Minions"));
            // Show villain target with lowest HP
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
            // Show hero with highest HP
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
            // Show Minion with lowest HP
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain(MinionKeyword), "", singular: "Minion", plural: "Minions"));
        }

        public override IEnumerator IconlessText()
        {
            // "Put all Minions from the villain trash into play."
            // NOTE: check with John to make sure this works like Forced Deployment, making a list of cards to move and then only moving those cards and only once
            List<Card> minionsInTrash = base.TurnTaker.Trash.Cards.Where((Card c) => c.DoKeywordsContain(MinionKeyword)).ToList();
            IEnumerator bulkCoroutine = base.GameController.BulkMoveCards(base.TurnTakerController, minionsInTrash, base.TurnTaker.OffToTheSide, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(bulkCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(bulkCoroutine);
            }
            minionsInTrash.Reverse();
            foreach (Card minion in minionsInTrash)
            {
                IEnumerator putCoroutine = base.GameController.PlayCard(base.TurnTakerController, minion, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(putCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(putCoroutine);
                }
            }
            List<Location> toCheck = new List<Location>();
            toCheck.Add(base.TurnTaker.OffToTheSide);
            IEnumerator cleanupCoroutine = CleanupCardsAtLocations(toCheck, base.TurnTaker.Trash);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cleanupCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cleanupCoroutine);
            }
        }

        public override IEnumerator HumanText()
        {
            // "The villain target with the lowest HP deals the hero with the highest HP {H - 1} melee damage."
            List<Card> lowest = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => IsVillainTarget(c), lowest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card target = lowest.FirstOrDefault();
            if (target != null)
            {
                IEnumerator meleeCoroutine = DealDamageToHighestHP(target, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => H - 1, DamageType.Melee);
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

        public override IEnumerator SnakeText()
        {
            // "The Minion with the lowest HP deals [i]Estrangular[/i] 2 projectile damage."
            if (base.CharacterCard.IsFlipped)
            {
                List<Card> lowest = new List<Card>();
                IEnumerator findCoroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.DoKeywordsContain(MinionKeyword), lowest, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(findCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(findCoroutine);
                }
                Card target = lowest.FirstOrDefault();
                if (target != null)
                {
                    IEnumerator projectileCoroutine = DealDamage(target, base.CharacterCard, 2, DamageType.Projectile, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(projectileCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(projectileCoroutine);
                    }
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
        }
    }
}
