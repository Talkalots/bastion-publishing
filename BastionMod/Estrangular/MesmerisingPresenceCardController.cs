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
            // "At the start of each player's turn, if that player has the fewest cards in play, that player cannot use more than 1 power this turn as long as this card is in play and {snake} effects are active."
            // Start of each hero turn: if {snake} effects are active, determine who the hero with the fewest cards in play is, save result
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, SnakeCheckCardsInPlayResponse, TriggerType.Other, additionalCriteria: (PhaseChangeAction pca) => ActivateSnake);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(ActiveHeroHasFewestCardsInPlay), TriggerType.Hidden);
            // If the active hero has the fewest cards in play and {snake} effects are active and the active hero has used as many powers as they have hero character cards this turn, then the active hero can't use powers
            CannotUsePowers((TurnTakerController ttc) => HasBeenSetToTrueThisTurn(ActiveHeroHasFewestCardsInPlay) && ttc.IsPlayer && ttc.TurnTaker == base.Game.ActiveTurnTaker && ActivateSnake && GetNumberOfPowersUsedThisTurn(ttc.ToHero()) > 0, (HeroCharacterCardController hcc) => HasBeenSetToTrueThisTurn(ActiveHeroHasFewestCardsInPlay) && hcc.TurnTaker.IsPlayer && hcc.TurnTaker == base.Game.ActiveTurnTaker && ActivateSnake && GetNumberOfPowersUsedThisTurn(base.GameController.FindHeroTurnTakerController(hcc.TurnTaker.ToHero())) > 0);
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
            // FindHeroWithFewestCardsInPlay automatically asks the players to resolve a tie, regardless of whether that tie includes the TurnTaker we care about
            // What we actually need to do is find out whether the active player can count as the player with the fewest cards in play
            bool match = false;
            Func<TurnTaker, int> cardsInPlay = (TurnTaker tt) => tt.GetCardsWhere((Card c) => c.IsInPlay).Count();
            IEnumerable<TurnTaker> sortedHeroes = FindTurnTakersWhere((TurnTaker tt) => tt.IsPlayer && !tt.IsIncapacitatedOrOutOfGame).OrderBy(cardsInPlay);
            int minNumberOfCards = cardsInPlay(sortedHeroes.ElementAt(0));
            //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: base.Game.ActiveTurnTaker: " + base.Game.ActiveTurnTaker.Name);
            //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: minNumberOfCards: " + minNumberOfCards);
            //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: cardsInPlay(" + base.Game.ActiveTurnTaker.Name + "): " + cardsInPlay(base.Game.ActiveTurnTaker));
            if (cardsInPlay(base.Game.ActiveTurnTaker) == minNumberOfCards)
            {
                //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: active player's number of cards matches minNumberOfCards");
                IEnumerable<TurnTaker> minHeroes = FindTurnTakersWhere((TurnTaker tt) => tt.IsPlayer && cardsInPlay(tt) == minNumberOfCards);
                //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: minHeroes.Count(): " + minHeroes.Count());
                if (minHeroes.Count() > 1)
                {
                    //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: minHeroes.Count() > 1");
                    // The active player is one of multiple players with the fewest cards in play
                    // Ask the players which one counts as the fewest
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
                        match = fewestCards == base.Game.ActiveTurnTaker;
                    }
                }
                else if (minHeroes.Count() == 1)
                {
                    //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: minHeroes.Count() == 1");
                    // The active TurnTaker is the only player with the fewest cards in play
                    match = true;
                }
            }
            //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: match: " + match);
            SetCardProperty(ActiveHeroHasFewestCardsInPlay, match);
            if (match)
            {
                //Log.Debug("MesmerisingPresence.SnakeCheckCardsInPlayResponse: " + base.Game.ActiveTurnTaker.Name + " is the hero with the fewest cards in play");
                IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Game.ActiveTurnTaker.Name + " is the hero with the fewest cards in play, and can only use 1 power during their turn.", Priority.High, GetCardSource(), showCardSource: true);
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

        private IEnumerator ShowMessageResponse(PhaseChangeAction pca)
        {
            yield return base.GameController.SendMessageAction(base.Card.Title + " prevents " + pca.FromPhase.TurnTaker.Name + " from using any more powers this turn. Skipping the rest of their Use Power phase.", Priority.High, GetCardSource(), showCardSource: true);
        }
    }
}
