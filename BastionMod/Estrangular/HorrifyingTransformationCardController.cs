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
    public class HorrifyingTransformationCardController : EstrangularOneShotCardController
    {
        public HorrifyingTransformationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show villain target with highest HP
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            // Show location of Ancient Serpent Armband
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c.Identifier == ArmbandIdentifier, "Ancient Serpent Armband", useCardsSuffix: false));
            // Show list of targets with 1 HP
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints.HasValueEqualTo(1), "with 1 HP", false, true, "target", "targets"));
        }

        public const string ArmbandIdentifier = "AncientSerpentArmband";

        public override IEnumerator IconlessText()
        {
            yield break;
        }

        public override IEnumerator HumanText()
        {
            // "The villain target with the highest HP deals each hero target 1 psychic damage."
            List<Card> highest = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsVillainTarget(c), highest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card target = highest.FirstOrDefault();
            if (target != null)
            {
                IEnumerator psychicCoroutine = DealDamage(target, (Card c) => c.IsHero, 1, DamageType.Psychic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(psychicCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(psychicCoroutine);
                }
            }
            if (ActivateHuman)
            {
                // "Search the villain deck and trash for {AncientSerpentArmband} and put it into play. If you searched the deck, shuffle it."
                IEnumerator fetchCoroutine = PlayCardFromLocations(new Location[] { base.TurnTaker.Deck, base.TurnTaker.Trash }, ArmbandIdentifier);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(fetchCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(fetchCoroutine);
                }
            }
        }

        public override IEnumerator SnakeText()
        {
            // "[i]Estrangular[/i] deals each target 1 melee damage."
            IEnumerator meleeCoroutine = DealDamage(base.CharacterCard, (Card c) => true, 1, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(meleeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(meleeCoroutine);
            }
            // "Destroy each target with 1 HP."
            IEnumerator destroyCoroutine = base.GameController.DestroyCards(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints.HasValueEqualTo(1), "with 1 HP", false, true, "target", "targets"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
        }
    }
}
