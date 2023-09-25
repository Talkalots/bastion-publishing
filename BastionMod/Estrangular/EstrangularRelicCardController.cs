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
    public abstract class EstrangularRelicCardController : CardController
    {
        public EstrangularRelicCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show whether Estrangular is in play
            SpecialStringMaker.ShowIfElseSpecialString(() => base.CharacterCard.IsFlipped, () => "[i]Estrangular[/i] is in play.", () => "[i]Estrangular[/i] is not in play.").Condition = () => base.CharacterCard.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            // "This card is immune to damage dealt by villain targets."
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target == base.Card && dda.DamageSource != null && dda.DamageSource.IsCard && IsVillainTarget(dda.DamageSource.Card));
            base.AddTriggers();
        }
    }
}
