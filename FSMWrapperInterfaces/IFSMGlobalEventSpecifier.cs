using System;

public interface IFSMGlobalEventSpecifier<out TEventEnum> 
    where TEventEnum : IConvertible
{
    TEventEnum[] GlobalEvents();
}

public static class FSMGlobalEventApplier
{
    public static bool isGlobalEvent<TEventEnum>(this IFSMGlobalEventSpecifier<TEventEnum> ges, TEventEnum eventType)
        where TEventEnum : IConvertible
    {
        return Array.IndexOf(ges.GlobalEvents(), eventType) >= 0;
    }
}
