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
    public class EstrangularTurnTakerController : TurnTakerController
    {
        public EstrangularTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            // "Put the villain character card into play [i]Jealous Cattle Baron[/i] side up."
            // "Search the villain deck for {ContrabandAPC} and put it into play, ..."
            Card apc = base.TurnTaker.GetCardByIdentifier("ContrabandAPC");
            IEnumerator apcCoroutine = base.GameController.PlayCard(this, apc, true, responsibleTurnTaker: base.TurnTaker, cardSource: new CardSource(base.CharacterCardController));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(apcCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(apcCoroutine);
            }
            // "... then reveal cards from the top of the villain deck until {H - 2} Minions are revealed, put them into play, and shuffle the other revealed cards back into the deck."
            IEnumerator minionCoroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.IsMinion, "Minion"), H - 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(minionCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(minionCoroutine);
            }
        }
    }
}
