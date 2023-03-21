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
    public class HumanSnakeUtilityCardController : CardController
    {
        public HumanSnakeUtilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show which icon effects will be activated
            base.SpecialStringMaker.ShowIfElseSpecialString(() => ActivateHuman, () => HUMAN + " effects are active.", () => HUMAN + " effects are not active.");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => ActivateSnake, () => SNAKE + " effects are active.", () => SNAKE + " effects are not active.");
        }

        public const string HUMAN = "{human}";
        public const string SNAKE = "{snake}";

        public const string MinionKeyword = "minion";

        public bool ActivateHuman => CanActivateEffect(base.TurnTakerController, HUMAN);
        public bool ActivateSnake => CanActivateEffect(base.TurnTakerController, SNAKE);
    }
}
