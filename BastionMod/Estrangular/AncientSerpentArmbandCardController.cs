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
    public class AncientSerpentArmbandCardController : CardController
    {
        public AncientSerpentArmbandCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Reduce all non-melee non-projectile damage dealt to [i]Estrangular[/i] by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && base.CharacterCard.IsFlipped && !(dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Projectile), (DealDamageAction dda) => 2);
            // "At the start of each player's turn, that player may discard their hand. If they discarded at least 1 card this way, destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, MayDiscardHandToDestructResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DestroySelf });
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, flip the villain character card to [i]Estrangular, Vision Serpent Incarnate.[/i]"
            if (!base.CharacterCard.IsFlipped)
            {
                IEnumerator flipCoroutine = base.GameController.FlipCard(base.CharacterCardController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(flipCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(flipCoroutine);
                }
            }
        }

        private IEnumerator MayDiscardHandToDestructResponse(PhaseChangeAction pca)
        {
            // "... that player may discard their hand."
            TurnTaker tt = pca.ToPhase.TurnTaker;
            if (!tt.IsIncapacitatedOrOutOfGame)
            {
                List<Card> relevant = new List<Card>();
                relevant.Add(base.Card);
                HeroTurnTakerController player = FindHeroTurnTakerController(tt.ToHero());
                YesNoDecision choice = new YesNoDecision(base.GameController, player, SelectionType.Custom, associatedCards: relevant, cardSource: GetCardSource());
                IEnumerator chooseCoroutine = base.GameController.MakeDecisionAction(choice);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseCoroutine);
                }
                if (choice != null && choice.Answer.HasValue && choice.Answer.Value)
                {
                    List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
                    IEnumerator discardCoroutine = base.GameController.DiscardHand(player, false, discardResults, base.TurnTaker, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(discardCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(discardCoroutine);
                    }
                    // "If they discarded at least 1 card this way, destroy this card."
                    if (discardResults.Any((DiscardCardAction dca) => dca != null && dca.WasCardDiscarded))
                    {
                        IEnumerator destructCoroutine = DestroyThisCardResponse(null);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(destructCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(destructCoroutine);
                        }
                    }
                    else
                    {
                        IEnumerator messageCoroutine = base.GameController.SendMessageAction("No cards were discarded, so " + base.Card.Title + " is not destroyed.", Priority.High, GetCardSource(), showCardSource: true);
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Do you want to discard your hand?", "choosing whether to discard their hand", "Vote for whether " + decision.HeroTurnTakerController.Name + " should discard their hand", "discard your hand");
        }
    }
}
