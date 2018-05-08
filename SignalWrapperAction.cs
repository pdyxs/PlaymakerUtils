using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class SignalWrapper : ActorsActionDo<IFSMStateDescriber>
    {
        [HideInInspector]
        public string enumType;

        public SignalWrapper()
        {   
        }
        
        public SignalWrapper(Type stateEnum)
        {
            enumType = stateEnum.ToString();
        }
        
        protected override void DoAction()
        {
            foreach (var actor in actors)
            {
                if (actor.stateType.ToString() == enumType)
                {
                    var wrap = actor as FSMWrapper;
                    wrap.StateEntered(wrap.fsm.ActiveStateName);
                }
            }
        }
    }
}