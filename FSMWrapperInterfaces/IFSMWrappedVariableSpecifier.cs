
public interface IFSMWrappedVariableSpecifier
{
    FSMVariableWrapper[] GetWrappedVariables();
}

public static class FSMWrappedVariableApplier
{
    public static void OnAwake(this IFSMWrappedVariableSpecifier wvs, PlayMakerFSM fsm)
    {
        foreach (var wrapper in wvs.GetWrappedVariables())
        {
            wrapper.Initialise(fsm);
        }
    }
    
#if UNITY_EDITOR
    public static void Apply(FSMWrapper fsmw)
    {
        if (!(fsmw is IFSMWrappedVariableSpecifier))
        {
            return;
        }
        foreach (var wrapper in (fsmw as IFSMWrappedVariableSpecifier).GetWrappedVariables()) {
            if (wrapper.name != "" && !fsmw.targetFsm.Variables.Contains(wrapper.name)) {
                wrapper.AddTo(fsmw.targetFsm.Variables);
                wrapper.Initialise(fsmw.fsm);
            }
        }
    }
#endif
}
