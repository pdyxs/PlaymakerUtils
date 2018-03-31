// cratesmith 2018, pdyxs 2018
using System;
using System.Collections;
#if UNITY_EDITOR
using System.Linq;
#endif
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.Events;


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
#if UNITY_EDITOR
    ,ISerializationCallbackReceiver
#endif
{
    [SerializeField]
    [DefaultOwnerObject]
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

    public virtual void StateEntered(string state) { }

    protected virtual void Awake()
    {
        if (this is IFSMWrappedVariableSpecifier)
        {
            (this as IFSMWrappedVariableSpecifier).OnAwake(this.fsm);
        }
    }

#if UNITY_EDITOR
    public Fsm targetFsm
    {
        get
        {
            if (fsm.UsesTemplate)
            {
                return fsm.FsmTemplate.fsm;
            }
            
            var root = (GameObject)UnityEditor.PrefabUtility.GetPrefabParent(gameObject);
            if (root != null && root != gameObject) {
                FSMWrapper pfsmwrapper = root.GetComponent<FSMWrapper>();
                if (pfsmwrapper != null)
                {
                    return pfsmwrapper.targetFsm;
                }
            }

            return fsm.Fsm;
        }
    }
    
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        UnityEditor.EditorApplication.delayCall += applyFSM;
    }

    private void applyFSM()
    {
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
        
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(doApplyFSM());
        } else {
            ApplyFSM();
            postApplyFSM();
        }
    }

    private IEnumerator doApplyFSM()
    {
        ApplyFSM();
        postApplyFSM();
        yield return new WaitForEndOfFrame();
    }

    protected virtual void ApplyFSM() {
        FSMWrappedVariableApplier.Apply(this);
    }

    private void postApplyFSM()
    {
        targetFsm.CheckIfDirty();
        //targetFsm.UpdateStateChanges();
        targetFsm.ForcePreprocess();
        UnityEditor.EditorUtility.SetDirty(fsm);
    }
#endif
}

public abstract class FSMWrapper<TEventEnum> :
    FSMWrapper
    where TEventEnum:IConvertible
{
    [FSMColour]
    public int eventColour = 4;
    
    protected virtual bool ShouldWaitToSendEvent(TEventEnum eventType, int attemptNumber)
    {
        return false;
    }
    
    protected virtual bool CanSendEvent(TEventEnum eventType)
    {
        return true;
    }

    public void SendEvent(TEventEnum eventType)
    {
        if (!typeof(TEventEnum).IsEnum) 
        {
            throw new ArgumentException("FSMWrapper T must be an enumerated type");
        }

        StartCoroutine(WaitToSendEvent(eventType));
    }

    private IEnumerator WaitToSendEvent(TEventEnum eventType)
    {
        int attemptNumber = 0;
        while (ShouldWaitToSendEvent(eventType, attemptNumber))
        {
            yield return null;
            attemptNumber++;
        }

        if (CanSendEvent(eventType))
        {
            fsm.SendEvent(System.Enum.GetName(typeof(TEventEnum), eventType));
        }
        else
        {
            //for now, we'll send the event either way
            fsm.SendEvent(System.Enum.GetName(typeof(TEventEnum), eventType));
        }
    }

    protected bool isGlobalEvent(TEventEnum eventType)
    {
        if (this is IFSMGlobalEventSpecifier<TEventEnum>)
        {
            return (this as IFSMGlobalEventSpecifier<TEventEnum>).isGlobalEvent(eventType);
        }

        return false;
    }

    protected bool doesStateSendEvent(TEventEnum eventType)
    {
        var state = Array.Find(fsm.FsmStates, (s) => s.Name == fsm.ActiveStateName);
        return Array.Find(state.Transitions, (a) => a.EventName == eventType.ToString()) != null;
    }

#if UNITY_EDITOR

    protected override void ApplyFSM() {
        base.ApplyFSM();

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

public interface IFSMStateDescriber
{
    int stateColour { get; }
}

public abstract class FSMWrapper<TEventEnum, TStateEnum> :
    FSMWrapper<TEventEnum>,
    IFSMStateDescriber 
    where TEventEnum : IConvertible
    where TStateEnum : IConvertible
{
    [FSMColour]
    [SerializeField]
    private int _stateColour = 5;

    public int stateColour { get { return _stateColour; } }
    
    [System.Serializable]
    public class TStateEvent : UnityEvent<TStateEnum> {}

    public TStateEnum currentState
    {
        get; private set;
    }

    public override void StateEntered(string state)
    {
        if (state == "") {
            return;
        }

        if (this is IFSMStateHandler<TStateEnum>)
        {
            try
            {
                currentState = (TStateEnum) Enum.Parse(typeof(TStateEnum), state);
                (this as IFSMStateHandler<TStateEnum>).StateEntered(currentState);
            }
            catch (Exception e)
            {
                Debug.LogError("Error in " + gameObject.name + " entering State " + state + ":\n" + e.StackTrace);
            }
        }
    }

    private bool currentStateCanFire(TEventEnum eventType)
    {
        if (this is IFSMEventRouteSpecifier<TEventEnum, TStateEnum>)
        {
            return (this as IFSMEventRouteSpecifier<TEventEnum, TStateEnum>).CanFireFrom(eventType, currentState);
        }
        return true;
    }

    private bool fsmActiveStateCanFire(TEventEnum eventType)
    {
        return currentState.ToString() == fsm.ActiveStateName || doesStateSendEvent(eventType);
    } 

    protected override bool ShouldWaitToSendEvent(TEventEnum eventType, int attemptNumber)
    {
        return !isGlobalEvent(eventType) && attemptNumber < 2 && 
               (!currentStateCanFire(eventType) || !fsmActiveStateCanFire(eventType));
    }

    protected override bool CanSendEvent(TEventEnum eventType)
    {
        if (!isGlobalEvent(eventType))
        {
            if (!currentStateCanFire(eventType))
            {
                Debug.LogError(gameObject.name + " (" + this.GetType().ToString() + "): " +
                               "Event " + eventType + " should not be fired" +
                               " from current state " + currentState);
                return false;
            }
            
            if (!fsmActiveStateCanFire(eventType))
            {
                Debug.LogError(gameObject.name + "(" + this.GetType().ToString() + "): " +
                               "Event " + eventType + " is not handled by " +
                               " intermediate state " + fsm.ActiveStateName);
                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    protected override void ApplyFSM()
    {
        base.ApplyFSM();
        
        ApplyStates();
        FSMStateHandlerApplier.Apply<TStateEnum>(this);
        FSMEventRoutesApplier.Apply<TEventEnum, TStateEnum>(this);
        FSMExecutableStateApplier.Apply<TStateEnum>(this);
    }

    void ApplyStates()
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
            state.ColorIndex = stateColour;
            return state;
        })).ToArray();
    }
#endif
}
