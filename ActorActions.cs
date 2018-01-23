/**
 * Originally developed by Cratesmith
 * github.com/cratesmith
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    public abstract class ActionQuery : ActionDo
	{
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		
		public override void Reset()
		{
			base.Reset();
			trueEvent = null;
			falseEvent = null;
		}
		
		protected abstract bool IsTrue();

		protected override void DoAction()
		{
			Fsm.Event(IsTrue() ? trueEvent : falseEvent);
		}
	}

    public abstract class ActionDo : FsmStateAction
	{
        public FsmBool everyFrame;

		public override void Reset()
		{
			base.Reset();
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
		    if (!Application.isPlaying) return;
			if(!everyFrame.Value)
			{
				Check();
				Finish();
			}
		}
		
		public override void OnUpdate ()
		{
			base.OnUpdate ();
			Check();
		}
		
		protected abstract void DoAction();
		
		void Check()
		{
			DoAction();
		}
	}

    public abstract class ActorAction<T> : FsmStateAction where T:Component
	{
		[RequiredFieldAttribute]
		[Tooltip("The actor's root gameobject")]
        public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			base.Reset ();
			gameObject = null;
		}
	
		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			actor = go ? go.GetComponent<T>():null;
			
			if(!actor)
			{
				Debug.LogError("ActorAction was created without actor of type: "+typeof(T).Name);
			}

			base.OnEnter();
		}

		public T actor { get; private set; }
	}

    public abstract class ActorActionDo<T> : ActorAction<T> where T:Component
	{
		public FsmBool everyFrame;
		
		public override void Reset()
		{
			base.Reset();
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			if(!everyFrame.Value)
			{
				Check();
				Finish();
			}
		}
		
		public override void OnUpdate ()
		{
			base.OnUpdate ();
			Check();
		}
		
		protected abstract void DoAction();
		
		void Check()
		{
			DoAction();
		}
	}

    public abstract class ActorActionDoQuery<T> : ActorActionDo<T> where T:Component
	{
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		
		public override void Reset()
		{
			base.Reset();
			trueEvent = null;
			falseEvent = null;
		}
		
		protected abstract bool IsTrue();

		protected override void DoAction()
		{
			Fsm.Event(IsTrue() ? trueEvent : falseEvent);
		}
	}
}