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
    public class MesmerisingPresenceCardController : HumanSnakeUtilityCardController
    {
        public MesmerisingPresenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero with fewest cards in play
            SpecialStringMaker.ShowHeroWithFewestCards(false);
            // If it's a hero turn and {snake} effects are active and the active hero was considered to have the fewest cards in play this turn and therefore can't use more than 1 power, show a note to that effect
            SpecialStringMaker.ShowSpecialString(() => base.Game.ActiveTurnTaker.Name + " was the hero with the fewest cards in play at the start of their turn, and cannot use more than 1 power this turn.").Condition = () => ActivateSnake && base.Game.ActiveTurnTaker != null && base.Game.ActiveTurnTaker.IsHero && HasBeenSetToTrueThisTurn(ActiveHeroHasFewestCardsInPlay);
        }

        protected readonly string ActiveHeroHasFewestCardsInPlay = "ActiveHeroHasFewestCardsInPlay";

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} Redirect all damage that would be dealt to villain targets to [i]Rico Homem[/i]."
            AddRedirectDamageTrigger((DealDamageAction dda) => ActivateHuman && IsVillainTarget(dda.Target) && dda.Target != base.CharacterCard && !base.CharacterCard.IsFlipped, () => base.CharacterCard);
            // "{snake} The hero with the fewest cards in play can only use 1 power during their turn."
            // After consult with designer, implemented as:
            // "At the start of each player's turn, if that player has the fewest cards in play, their hero cannot use more than 1 power this turn as long as this card is in play and {snake} effects are active."
            // Start of each hero turn: if {snake} effects are active, determine who the hero with the fewest cards in play is, save result
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, SnakeCheckCardsInPlayResponse, TriggerType.Other, additionalCriteria: (PhaseChangeAction pca) => ActivateSnake);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(ActiveHeroHasFewestCardsInPlay), TriggerType.Hidden);
            // If the active hero has the fewest cards in play and {snake} effects are active and the active hero has used as many powers as they have hero character cards this turn, then the active hero can't use powers
            CannotUsePowers((TurnTakerController ttc) => HasBeenSetToTrueThisTurn(ActiveHeroHasFewestCardsInPlay) && ttc.IsPlayer && ttc.TurnTaker == base.Game.ActiveTurnTaker && ActivateSnake && GetNumberOfPowersUsedThisTurn(ttc.ToHero()) >= ttc.CharacterCardControllers.Count(), (HeroCharacterCardController hcc) => GetNumberOfPowersUsedThisTurn(hcc) > 0);
            // If this causes a player to skip their Use Power phase, show a message to that effect
            AddTrigger((PhaseChangeAction pca) => base.GameController.ActiveTurnPhase != null && base.GameController.ActiveTurnPhase.IsUsePower && pca.FromPhase.IsUsePower && pca.FromPhase.TurnTaker.IsPlayer && base.GameController.ActiveTurnPhase.TurnTaker == pca.FromPhase.TurnTaker && base.GameController.GetPhaseActionCountForTurnPhase(pca.FromPhase) > 0 && ActivateSnake && HasBeenSetToTrueThisTurn(ActiveHeroHasFewestCardsInPlay) && GetNumberOfPowersUsedThisTurn(FindHeroTurnTakerController(pca.FromPhase.TurnTaker.ToHero())) > 0, ShowMessageResponse, TriggerType.ShowMessage, TriggerTiming.Before);
        }

        public override IEnumerator Play()
        {
            // If this card enters play on a hero turn and {snake} effects are active, immediately check whether the active hero has the fewest cards in play
            if (ActivateSnake && base.Game.ActiveTurnTaker != null && base.Game.ActiveTurnTaker.IsHero)
            {
                IEnumerator checkCoroutine = SnakeCheckCardsInPlayResponse(null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(checkCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(checkCoroutine);
                }
            }
            yield break;
        }

        private IEnumerator SnakeCheckCardsInPlayResponse(PhaseChangeAction pca)
        {
            List<TurnTaker> storedResults = new List<TurnTaker>();
            IEnumerator findCoroutine = FindHeroWithFewestCardsInPlay(storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            if (storedResults.Count > 0)
            {
                TurnTaker fewestCards = storedResults.First();
                bool match = fewestCards == base.Game.ActiveTurnTaker;
                SetCardProperty(ActiveHeroHasFewestCardsInPlay, match);
            }
        }

        private IEnumerator ShowMessageResponse(PhaseChangeAction pca)
        {
            yield return base.GameController.SendMessageAction(base.Card.Title + " prevents " + pca.FromPhase.TurnTaker + " from using any more powers this turn. Skipping the rest of their Use Power phase.", Priority.High, GetCardSource(), showCardSource: true);
        }
    }
}
