﻿// cratesmith 2018, pdyxs 2018
using System;
using System.Collections;
#if UNITY_EDITOR
using System.Linq;
#endif
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;


// A base class for FSM wrappers which bind an Enum in C# to Events in Playmaker.
// In editor the playmaker FSM (or FSMTemplate) will always contain non-global event stings for each item in the enum.

// Example usage :
//
//public class FSMWrapperExample : FSMWrapper<ExampleFSMEvents>
//{    
//}

//public enum ExampleFSMEvents 
//{
//  TacoTimeStart,
//  TacoTimeEnd
//}
//
//[RequireComponent(typeof(FSMWrapperExample))]
//public class Example : MonoBehaviour
//{
//  void Start()
//  {
//      var wrapper = gameObject.GetComponent<FSMWrapperExample>();
//      wrapper.SendEvent(ExampleFSMEvents.TacoTimeStart);
//      wrapper.SendEvent(ExampleFSMEvents.TacoTimeEnd);
//  }
//}
//

[RequireComponent(typeof(PlayMakerFSM))]
public abstract class FSMWrapper : MonoBehaviour
{
    private PlayMakerFSM m_fsm;
    public PlayMakerFSM fsm
    {
        get
        {
            if (m_fsm == null)
            {
                m_fsm = GetComponent<PlayMakerFSM>();
            }
            return m_fsm;
        }
    }

    public virtual void OnStateEntered(string state) { }

#if UNITY_EDITOR
    protected Fsm targetFsm
    {
        get
        {
            var root = UnityEditor.PrefabUtility.FindPrefabRoot(gameObject);
            if (root != null && root != gameObject) {
                FSMWrapper pfsmwrapper = root.GetComponent<FSMWrapper>();
                if (pfsmwrapper != null)
                {
                    return pfsmwrapper.targetFsm;
                }
            }

            return fsm.UsesTemplate ? fsm.FsmTemplate.fsm : fsm.Fsm;
        }
    }
#endif
}

public abstract class FSMWrapper<TEventEnum> :
    FSMWrapper
#if UNITY_EDITOR
    ,ISerializationCallbackReceiver
#endif
    where TEventEnum:IConvertible
{

    public virtual void SendEvent(TEventEnum eventType)
    {
        if (!typeof(TEventEnum).IsEnum) 
        {
            throw new ArgumentException("FSMWrapper T must be an enumerated type");
        }
        fsm.SendEvent(System.Enum.GetName(typeof(TEventEnum), eventType));
    }

#if UNITY_EDITOR
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        UnityEditor.EditorApplication.delayCall += ApplyFSM;
    }

    protected virtual void ApplyFSM() {
        if (Application.isPlaying || this == null || gameObject == null)
        {
            return;
        }

        if (fsm == null)
        {
            gameObject.AddComponent<PlayMakerFSM>();
        }

        if (targetFsm == null)
        {
            return;
        }

        ApplyEvents();
    }

    void ApplyEvents()
    {
        if (!typeof(TEventEnum).IsEnum) 
        {
            throw new ArgumentException("FSMWrapper TEventEnum must be an enumerated type");
        }

        var eventNames = Enum.GetNames(typeof(TEventEnum));
        var missingEventNames = eventNames.Where(x => targetFsm.Events.All(y => y.Name != x));
        targetFsm.Events = targetFsm.Events.Concat(missingEventNames.Select(x=>new FsmEvent(x)))
            .ToArray();
    }
#endif
}

public abstract class FSMWrapper<TEventEnum, TStateEnum> :
    FSMWrapper<TEventEnum>
    where TEventEnum : IConvertible
    where TStateEnum : IConvertible
{

    public TStateEnum currentState
    {
        get; private set;
    }

    public override void OnStateEntered(string state)
    {
        currentState = (TStateEnum)Enum.Parse(typeof(TStateEnum), state);
        OnStateEntered(currentState);
    }

    public abstract void OnStateEntered(TStateEnum state);

    public override void SendEvent(TEventEnum eventType)
    {
        if (!EventsFrom(currentState).Contains(eventType))
        {
            Debug.LogError("Event " + eventType + " should not be fired" +
                           " from current state " + currentState);
        }

        base.SendEvent(eventType);
    }

    protected abstract TEventEnum[] EventsFrom(TStateEnum state);

    protected abstract FSMVariableWrapper[] GetWrappedVariables();

    protected virtual void Awake()
    {
        foreach (var wrapper in GetWrappedVariables()) {
            wrapper.Initialise(fsm);
        }
    }

#if UNITY_EDITOR
    protected override void ApplyFSM()
    {
        base.ApplyFSM();

        if (Application.isPlaying || this == null || gameObject == null)
        {
            return;
        }

        if (targetFsm == null)
        {
            return;
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ApplyStates());
        }
    }

    IEnumerator ApplyStates()
    {
        if (!typeof(TStateEnum).IsEnum)
        {
            throw new ArgumentException("FSMWrapper TStateEnum must be an enumerated type");
        }

        if (!targetFsm.Initialized) {
            targetFsm.Init(this);
        }

        var stateNames = Enum.GetNames(typeof(TStateEnum));

        var missingStateNames = stateNames.Where(x => targetFsm.States.All(y => y.Name != x));
        targetFsm.States = targetFsm.States.Concat(missingStateNames.Select(x =>
        {
            var state = new FsmState(targetFsm);
            state.Name = x;
            return state;
        })).ToArray();

        foreach (TStateEnum s in Enum.GetValues(typeof(TStateEnum)))
        {
            var sn = Enum.GetName(typeof(TStateEnum), s);
            FsmState state = targetFsm.GetState(sn);
            state.ColorIndex = 5;
            if (Array.Find(state.Actions, (a) => a is SignalWrapperAction) == null)
            {
                var newAction = new SignalWrapperAction();
                state.Actions = state.Actions.Concat(
                    new FsmStateAction[] { newAction }
                ).ToArray();
            }

            var transitions = EventsFrom(s);
            var transitionNames = transitions.Select(
                (x) => Enum.GetName(typeof(TEventEnum), x));
            var missingTransitions = transitionNames.Where(
                x => state.Transitions.All(y => y.EventName != x));
            state.Transitions = state.Transitions.Concat(
                missingTransitions.Select(x => {
                    var t = new FsmTransition();
                    t.FsmEvent = targetFsm.GetEvent(x);
                    return t;
                })).ToArray();

            foreach (var t in state.Transitions) {
                if (transitionNames.Contains(t.EventName)) {
                    t.ColorIndex = 5;
                    t.LinkStyle = FsmTransition.CustomLinkStyle.Circuit;
                }
            }
        }

        foreach (FSMVariableWrapper wrapper in GetWrappedVariables()) {
            if (wrapper.name != "" && !targetFsm.Variables.Contains(wrapper.name)) {
                wrapper.AddTo(targetFsm.Variables);
            }
            wrapper.Initialise(fsm);
        }

        targetFsm.UpdateStateChanges();
        targetFsm.ForcePreprocess();
        UnityEditor.EditorUtility.SetDirty(fsm);
        yield return new WaitForEndOfFrame();
    }
#endif
}
