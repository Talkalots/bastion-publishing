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
    public class ToroRoccoCardController : HumanSnakeUtilityCardController
    {
        public ToroRoccoCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show non-villain target with highest HP
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP();
            // Show villain target with highest HP
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} At the end of the villain turn, this card deals the non-villain target with the highest HP {H - 1} melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToHighestHP(base.Card, 1, (Card c) => !IsVillainTarget(c), (Card c) => H - 1, DamageType.Melee), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateHuman);
            // "{snake} At the start of the villain turn, this card deals the villain target with the highest HP {H - 1} melee damage."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToHighestHP(base.Card, 1, (Card c) => IsVillainTarget(c), (Card c) => H - 1, DamageType.Melee), TriggerType.DealDamage, (PhaseChangeAction pca) => ActivateSnake);
        }
    }
}
