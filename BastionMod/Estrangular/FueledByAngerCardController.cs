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
    public class FueledByAngerCardController : HumanSnakeUtilityCardController
    {
        public FueledByAngerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} Increase damage dealt by villain targets by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => ActivateHuman && dda.DamageSource.IsCard && IsVillainTarget(dda.DamageSource.Card), (DealDamageAction dda) => 1);
            // "{snake} Increase damage dealt by [i]Estrangular[/i] by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => ActivateSnake && dda.DamageSource.IsCard && dda.DamageSource.Card == base.CharacterCard && base.CharacterCard.IsFlipped, (DealDamageAction dda) => 1);
        }
    }
}
