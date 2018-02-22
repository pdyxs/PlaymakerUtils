using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class SignalWrapperAction : ActorActionDo<FSMWrapper>
    {
        protected override void DoAction()
        {
            actor.OnStateEntered(actor.fsm.ActiveStateName);
        }
    }
}