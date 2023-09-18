using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bastion.Estrangular
{
    internal class EstrangularCharacterCardController : VillainCharacterCardController
    {
        public EstrangularCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ActivatesEffects);
            // Special strings...
            // Front...
            // Show number of Minions in the villain trash
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain(MinionKeyword), "", singular: "Minion", plural: "Minions"), showInEffectsList: () => !base.Card.IsFlipped).Condition = () => !base.Card.IsFlipped;
            // Show location of Ancient Serpent Armband
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c.Identifier == ArmbandIdentifier, "Ancient Serpent Armband", useCardsSuffix: false)).Condition = () => !base.Card.IsFlipped;
            // Back...
            // Show whether Ancient Serpent Armband is in play
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(ArmbandIdentifier).Condition = () => base.Card.IsFlipped;
            // Show hero target with highest HP
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsFlipped;
        }

        public const string HUMAN = "{human}";
        public const string SNAKE = "{snake}";
        public const string MinionKeyword = "minion";
        public const string ArmbandIdentifier = "AncientSerpentArmband";

        public override bool? AskIfActivatesEffect(TurnTakerController turnTakerController, string effectKey)
        {
            bool? result = null;
            if (turnTakerController == base.TurnTakerController)
            {
                if (!base.Card.IsFlipped)
                {
                    // Front side: "Activate the {human} on villain cards."
                    if (effectKey == HUMAN)
                    {
                        result = true;
                    }
                }
                else
                {
                    // Back side: "Activate the {snake} on villain cards."
                    if (effectKey == SNAKE)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if (!base.Card.IsFlipped)
            {
                // Front side:
                // "If [i]Rico Homem[/i] is reduced to 0 or fewer HP or otherwise destroyed, the heroes lose the game."
                AddSideTrigger(AddTrigger((DealDamageAction dda) => dda.DidDealDamage && dda.Amount > 0 && dda.Target == base.Card && dda.TargetHitPointsAfterBeingDealtDamage <= 0, HeroesLoseResponse, TriggerType.GameOver, TriggerTiming.After));
                AddSideTrigger(AddTrigger((SetHPAction sha) => sha.HpGainer == base.Card && base.Card.HitPoints <= 0, HeroesLoseResponse, TriggerType.GameOver, TriggerTiming.After));
                AddSideTrigger(AddTrigger((DestroyCardAction dca) => dca.CardToDestroy.Card == base.Card, HeroesLoseResponse, TriggerType.GameOver, TriggerTiming.Before));
                // Heroes also lose if he's removed from game
                AddSideTrigger(AddTrigger((MoveCardAction mca) => mca.CardToMove == base.Card && mca.Destination.Name == LocationName.OutOfGame, HeroesLoseResponse, TriggerType.GameOver, TriggerTiming.Before));
                // "At the start of the villain turn, if there are {H} or more Minions in the villain trash, search the villain deck and trash for {AncientSerpentArmband} and put it into play."
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FetchArmbandIfApplicableResponse, TriggerType.PlayCard));
                // "At the end of the villain turn, each villain target regains 1 HP, then play the top card of the villain deck."
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HealPlayResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.PlayCard }));
                if (base.IsGameAdvanced)
                {
                    // Front side, Advanced:
                    // "At the start of the villain turn, play the top card of the villain deck."
                    base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard));
                }
            }
            else
            {
                // Back side:
                // "Whenever [i]Estrangular[/i] destroys a target, he regains 3 HP."
                AddSideTrigger(AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget && dca.CardSource != null && (dca.CardSource.Card == base.Card || dca.CardSource.Card.ResponsibleTarget == base.Card), (DestroyCardAction dca) => base.GameController.GainHP(base.Card, 3, cardSource: GetCardSource()), TriggerType.GainHP, TriggerTiming.After));
                // "At the start of the villain turn, if {AncientSerpentArmband} is not in play, shuffle the villain trash into the villain deck, then flip this card."
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && !FindCard(ArmbandIdentifier).IsInPlayAndHasGameText, ReshuffleFlipResponse, new TriggerType[] { TriggerType.ShuffleTrashIntoDeck, TriggerType.FlipCard }));
                // "At the end of the villain turn, [i]Estrangular[/i] deals each other target 1 melee damage, then deals the hero target with the highest HP 2 melee damage and 2 infernal damage."
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, MassDamageResponse, TriggerType.DealDamage));
                // If destroyed or removed from game, the heroes win
                AddSideTrigger(AddTrigger((DealDamageAction dda) => dda.DidDealDamage && dda.Amount > 0 && dda.Target == base.Card && dda.TargetHitPointsAfterBeingDealtDamage <= 0, DefeatedResponse, TriggerType.GameOver, TriggerTiming.After));
                AddSideTrigger(AddTrigger((SetHPAction sha) => sha.HpGainer == base.Card && base.Card.HitPoints <= 0, DefeatedResponse, TriggerType.GameOver, TriggerTiming.After));
                AddSideTrigger(AddTrigger((DestroyCardAction dca) => dca.CardToDestroy.Card == base.Card, DefeatedResponse, TriggerType.GameOver, TriggerTiming.Before));
                AddSideTrigger(AddTrigger((MoveCardAction mca) => mca.CardToMove == base.Card && mca.Destination.Name == LocationName.OutOfGame, DefeatedResponse, TriggerType.GameOver, TriggerTiming.Before));
                if (base.IsGameAdvanced)
                {
                    // Back side, Advanced:
                    // "Reduce damage dealt to [i]Estrangular[/i] by 1."
                    base.AddSideTrigger(AddReduceDamageTrigger((Card c) => c == base.Card, 1));
                }
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator baseCoroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(baseCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(baseCoroutine);
            }
            // Back side: "When the villain character flips to this side, he regains {H * 5} HP."
            if (base.Card.IsFlipped)
            {
                IEnumerator healCoroutine = base.GameController.GainHP(base.Card, H * 5, cardSource: GetCardSource());
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

        private IEnumerator HeroesLoseResponse(GameAction ga)
        {
            // "... the heroes lose the game."
            yield return base.GameController.GameOver(EndingResult.AlternateDefeat, "Rico Homem was reduced to 0 or fewer HP or otherwise destroyed or removed from the game. The heroes lose!", cardSource: GetCardSource());
        }

        private IEnumerator FetchArmbandIfApplicableResponse(PhaseChangeAction pca)
        {
            // "... if there are {H} or more Minions in the villain trash, search the villain deck and trash for {AncientSerpentArmband} and put it into play."
            int count = base.TurnTaker.Trash.Cards.Where((Card c) => c.DoKeywordsContain(MinionKeyword)).Count();
            if (count >= H)
            {
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

        private IEnumerator HealPlayResponse(PhaseChangeAction pca)
        {
            // "... each villain target regains 1 HP, ..."
            IEnumerator healCoroutine = base.GameController.GainHP(base.DecisionMaker, (Card c) => IsVillainTarget(c), 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(healCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(healCoroutine);
            }
            // "... then play the top card of the villain deck."
            IEnumerator playCoroutine = PlayTheTopCardOfTheVillainDeckResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
        }

        private IEnumerator ReshuffleFlipResponse(PhaseChangeAction pca)
        {
            // "... shuffle the villain trash into the villain deck, ..."
            IEnumerator shuffleCoroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            // "... then flip this card."
            IEnumerator flipCoroutine = base.GameController.FlipCard(this, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(flipCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(flipCoroutine);
            }
        }

        private IEnumerator MassDamageResponse(PhaseChangeAction pca)
        {
            // "... [i]Estrangular[/i] deals each other target 1 melee damage, ..."
            IEnumerator allCoroutine = DealDamage(base.Card, (Card c) => c != base.Card, 1, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(allCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(allCoroutine);
            }
            // "... then deals the hero target with the highest HP 2 melee damage and 2 infernal damage."
            List<DealDamageAction> instances = new List<DealDamageAction>();
            instances.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Melee));
            instances.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Infernal));
            IEnumerator highestCoroutine = DealMultipleInstancesOfDamageToHighestLowestHP(instances, (Card c) => IsHeroTarget(c), HighestLowestHP.HighestHP);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(highestCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(highestCoroutine);
            }
        }
    }
}
