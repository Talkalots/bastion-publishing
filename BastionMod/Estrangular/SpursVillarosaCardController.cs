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
    public class SpursVillarosaCardController : HumanSnakeUtilityCardController
    {
        public SpursVillarosaCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero target with highest HP
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the end of the villain turn, this card deals the hero target with the highest HP 2 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToHighestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Melee), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the start of each hero turn, this card deals [i]Estrangular[/i] 1 melee damage."
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, (PhaseChangeAction pca) => DealDamage(base.Card, base.CharacterCard, 1, DamageType.Melee, cardSource: GetCardSource()), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateSnake && base.CharacterCard.IsFlipped);
        }
    }
}
