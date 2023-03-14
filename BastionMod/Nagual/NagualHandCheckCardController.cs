using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bastion.Nagual
{
    public class NagualHandCheckCardController : CardController
    {
        public NagualHandCheckCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show number of cards in Nagual's hand
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.HeroTurnTaker.Hand);
        }
    }
}
