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
    public class LariatEspinosaCardController : HumanSnakeUtilityCardController
    {
        public LariatEspinosaCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show hero target with second lowest HP
            SpecialStringMaker.ShowHeroTargetWithLowestHP(2);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the start of the villain turn, this card deals the hero target with the second lowest HP 2 melee damage."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToLowestHP(base.Card, 2, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Melee), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} Reduce damage dealt by [i]Estrangular[/i] by 1."
            AddReduceDamageTrigger((DealDamageAction dda) => ActivateSnake && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped, (DealDamageAction dda) => 1);
        }
    }
}
