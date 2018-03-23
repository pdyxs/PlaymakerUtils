using System;
using System.Linq;
using HutongGames.PlayMaker;

public interface IFSMExecutableStateSpecifier<in TStateEnum> 
	where TStateEnum : IConvertible
{
	 Type[] GetStateActions(TStateEnum state);
}

public static class FSMExecutableStateApplier
{
	public const string EventName = "CodeExecutionComplete";

#if UNITY_EDITOR	
	public static void Apply<TStateEnum>(FSMWrapper fsmw)
		where TStateEnum : IConvertible
	{
		if (!(fsmw is IFSMExecutableStateSpecifier<TStateEnum>))
		{
			return;
		}
		
		foreach (TStateEnum s in Enum.GetValues(typeof(TStateEnum)))
		{
			var actions = (fsmw as IFSMExecutableStateSpecifier<TStateEnum>).GetStateActions(s);
			if (actions.Length == 0)
			{
				continue;
			}
			
			var sn = Enum.GetName(typeof(TStateEnum), s);
			FsmState state = fsmw.targetFsm.GetState(sn);
			
			var missingActions = actions.Where(
				x => !Array.Exists(state.Actions, (a) => a.GetType() == x));
			state.Actions = state.Actions.Concat(
				missingActions.Select(x => (FsmStateAction)Activator.CreateInstance(x))
			).ToArray();
		}
		
	}
#endif
}