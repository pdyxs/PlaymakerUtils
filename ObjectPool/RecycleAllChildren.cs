using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Object Pool")]
    [Tooltip("Recycles a GameObject")]
    public class RecycleAllChildren : ActionDo
    {
        [RequiredField]
        [Tooltip("The Gameobject to Recycle")]
        public FsmOwnerDefault gameObject;

        protected override void DoAction()
        {
            var transform = Fsm.GetOwnerDefaultTarget(gameObject).transform;
            foreach (Transform child in transform) {
                child.Recycle();
            }
        }
    }
}