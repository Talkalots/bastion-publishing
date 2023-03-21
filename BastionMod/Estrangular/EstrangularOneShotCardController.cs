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
    public abstract class EstrangularOneShotCardController : HumanSnakeUtilityCardController
    {
        public EstrangularOneShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public abstract IEnumerator IconlessText();

        public abstract IEnumerator HumanText();

        public abstract IEnumerator SnakeText();

        public bool HasIconless = false;
        public bool HasHuman = true;
        public bool HasSnake = true;

        public override IEnumerator Play()
        {
            // If iconless text is present, activate it
            if (HasIconless)
            {
                IEnumerator iconlessCoroutine = IconlessText();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(iconlessCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(iconlessCoroutine);
                }
            }
            // If {human} text is present and active, activate it
            if (HasHuman && ActivateHuman)
            {
                IEnumerator humanCoroutine = HumanText();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(humanCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(humanCoroutine);
                }
            }
            // If {snake} text is present and active, activate it
            if (HasSnake && ActivateSnake)
            {
                IEnumerator snakeCoroutine = SnakeText();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(snakeCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(snakeCoroutine);
                }
            }
        }
    }
}
