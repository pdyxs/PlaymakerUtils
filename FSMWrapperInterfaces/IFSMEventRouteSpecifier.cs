using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
#if UNITY_EDITOR
using System.Linq;
#endif

public interface IFSMEventRouteSpecifier<out TEventEnum, in TStateEnum> 
    where TStateEnum : IConvertible
    where TEventEnum : IConvertible
{
    TEventEnum[] EventsFrom(TStateEnum state);
}

public static class FSMEventRoutesApplier
{
    public static bool CanFireFrom<TEventEnum, TStateEnum>
        (
            this IFSMEventRouteSpecifier<TEventEnum, TStateEnum> ers,
            TEventEnum eventType, TStateEnum state
        )
        where TStateEnum : IConvertible
        where TEventEnum : IConvertible
    {
        return Array.IndexOf(ers.EventsFrom(state), eventType) >= 0;
    }
    
#if UNITY_EDITOR
    public static void Apply<TEventEnum, TStateEnum>(FSMWrapper fsmw)
        where TStateEnum : IConvertible
        where TEventEnum : IConvertible
    {
        if (!(fsmw is IFSMEventRouteSpecifier<TEventEnum, TStateEnum>))
        {
            return;
        }
        foreach (TStateEnum s in Enum.GetValues(typeof(TStateEnum)))
        {
            var sn = Enum.GetName(typeof(TStateEnum), s);
            FsmState state = fsmw.targetFsm.GetState(sn);
                
            var transitions = (fsmw as IFSMEventRouteSpecifier<TEventEnum, TStateEnum>).EventsFrom(s);
            var transitionNames = transitions.Select(
                (x) => Enum.GetName(typeof(TEventEnum), x));
            var missingTransitions = transitionNames.Where(
                x => state.Transitions.All(y => y.EventName != x));
            state.Transitions = state.Transitions.Concat(
                missingTransitions.Select(x => {
                    var t = new FsmTransition();
                    t.FsmEvent = fsmw.targetFsm.GetEvent(x);
                    return t;
                })).ToArray();

            foreach (var t in state.Transitions) {
                if (transitionNames.Contains(t.EventName)) {
                    t.ColorIndex = 4;
                    t.LinkStyle = FsmTransition.CustomLinkStyle.Circuit;
                }
            }
        }
    }
#endif
}
