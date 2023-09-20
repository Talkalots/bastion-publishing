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
    public class PoachersCardController : HumanSnakeUtilityCardController
    {
        public PoachersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show non-villain target with lowest HP
            SpecialStringMaker.ShowNonVillainTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the end of the villain turn, this card deals the non-villain target with the lowest HP 2 projectile damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToLowestHP(base.Card, 1, (Card c) => !IsVillainTarget(c), (Card c) => 2, DamageType.Projectile), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the end of the villain turn, this card deals [i]Estrangular[/i] 2 projectile damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.Card, base.CharacterCard, 2, DamageType.Projectile, cardSource: GetCardSource()), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateSnake && base.CharacterCard.IsFlipped);
        }
    }
}
