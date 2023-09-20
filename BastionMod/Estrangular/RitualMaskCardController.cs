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
    public class RitualMaskCardController : CardController
    {
        public RitualMaskCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "This card is immune to damage dealt by villain targets."
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target == base.Card && dda.DamageSource != null && dda.DamageSource.IsCard && IsVillainTarget(dda.DamageSource.Card));
            // "Damage dealt by [i]Estrangular[/i] is irreducible."
            AddMakeDamageIrreducibleTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped);
            // "At the end of the villain turn, each player may discard a card. Each hero whose player did not discard a card this way deals themself 1 psychic damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DiscardOrDealSelfDamageResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DealDamage });
        }

        private IEnumerator DiscardOrDealSelfDamageResponse(PhaseChangeAction pca)
        {
            // "... each player may discard a card."
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            IEnumerator discardCoroutine = base.GameController.EachPlayerDiscardsCards(0, 1, discardResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCoroutine);
            }
            // "Each hero whose player did not discard a card this way deals themself 1 psychic damage."
            List<HeroTurnTakerController> didNotDiscardPlayers = FindActiveHeroTurnTakerControllers().Except(from d in discardResults where d.WasCardDiscarded select d.HeroTurnTakerController).ToList();
            List<Card> didNotDiscardHeroes = FindCardsWhere((Card c) => IsHeroCharacterCard(c) && c.IsTarget && c.Owner.IsHero && didNotDiscardPlayers.Contains(base.GameController.FindTurnTakerController(c.Owner).ToHero())).ToList();
            IEnumerator psychicCoroutine = base.GameController.DealDamageToSelf(DecisionMaker, (Card c) => didNotDiscardHeroes.Contains(c), (Card c) => 1, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(psychicCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(psychicCoroutine);
            }
        }
    }
}
