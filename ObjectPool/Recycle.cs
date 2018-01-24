using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Object Pool")]
    [Tooltip("Recycles a GameObject")]
    public class Recycle : ActionDo
    {
        [RequiredField]
        [Tooltip("The Gameobject to Recycle")]
        public FsmOwnerDefault gameObject;

        protected override void DoAction()
        {
            Fsm.GetOwnerDefaultTarget(gameObject).Recycle();
        }
    }
}