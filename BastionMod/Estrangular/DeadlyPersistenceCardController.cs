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
    public class DeadlyPersistenceCardController : HumanSnakeUtilityCardController
    {
        public DeadlyPersistenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "{human} Whenever a villain target is destroyed, [i]Rico Homem[/i] deals each hero target 1 projectile damage."
            AddTrigger((DestroyCardAction dca) => ActivateHuman && IsVillainTarget(dca.CardToDestroy.Card) && dca.WasCardDestroyed && !base.CharacterCard.IsFlipped, HumanRevengeResponse, TriggerType.DealDamage, TriggerTiming.After);
            // "{snake} Reduce projectile damage dealt to [i]Estrangular[/i] by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => ActivateSnake && dda.DamageType == DamageType.Projectile && dda.Target == base.CharacterCard && base.CharacterCard.IsFlipped, (DealDamageAction dda) => 2);
        }

        private IEnumerator HumanRevengeResponse(DestroyCardAction dca)
        {
            // "... [i]Rico Homem[/i] deals each hero target 1 projectile damage."
            IEnumerator projectileCoroutine = DealDamage(base.CharacterCard, (Card c) => IsHeroTarget(c), 1, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(projectileCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(projectileCoroutine);
            }
        }
    }
}
