// cratesmith 2018
using System;
#if UNITY_EDITOR
using System.Linq;
#endif
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Ecosystem.Utils;
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
public abstract class FSMWrapper<TEventEnum> : MonoBehaviour, ISerializationCallbackReceiver where TEventEnum:IConvertible
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

    public void SendEvent(TEventEnum eventType)
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
        UnityEditor.EditorApplication.delayCall += ApplyEvents;
    }

    void ApplyEvents()
    {
        if (!typeof(TEventEnum).IsEnum) 
        {
            throw new ArgumentException("FSMWrapper T must be an enumerated type");
        }

        if (fsm == null)
        {
            gameObject.AddComponent<PlayMakerFSM>();
        }

        var targetFsm = fsm.UsesTemplate ? fsm.FsmTemplate.fsm:fsm.Fsm;
        if (targetFsm==null)
        {
            return;
        }

        var eventNames = System.Enum.GetNames(typeof(TEventEnum));
        var missingEventNames = eventNames.Where(x => targetFsm.Events.All(y => y.Name != x));
        targetFsm.Events = targetFsm.Events.Concat(missingEventNames.Select(x=>new FsmEvent(x)))
            .ToArray();
    }
#endif
}