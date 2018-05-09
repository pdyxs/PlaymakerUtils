using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
#if UNITY_EDITOR
using System.Linq;
#endif

public interface IFSMStateHandler<in TStateEnum> 
	where TStateEnum : IConvertible
{
	void StateEntered(TStateEnum state);
}

#if UNITY_EDITOR
public static class FSMStateHandlerApplier
{
	public static void Apply<TStateEnum>(FSMWrapper fsmw)
		where TStateEnum : IConvertible
	{
		//check states for signal wrapper action
		var allStateNames = Enum.GetNames(typeof(TStateEnum));
		foreach (var state in fsmw.targetFsm.States)
		{
			bool shouldContainSignal = false;
			if (allStateNames.Contains(state.Name))
			{
				state.ColorIndex = (fsmw as IFSMStateDescriber).stateColour;
				shouldContainSignal = true;
			}

			if (shouldContainSignal)
			{
				if (Array.Find(state.Actions, (a) => a is SignalWrapper) == null)
				{
					var newAction = new SignalWrapper(typeof(TStateEnum));
					state.Actions = new FsmStateAction[] {newAction}.Concat(
						state.Actions
					).ToArray();
				}
			}
			else
			{
				var i = Array.FindIndex(state.Actions, 
					(a) => (a is SignalWrapper && (a as SignalWrapper).enumType == typeof(TStateEnum).ToString()));
				if (i >= 0)
				{
					var dest = new FsmStateAction[state.Actions.Length - 1];
					if( i > 0 )
						Array.Copy(state.Actions, 0, dest, 0, i);

					if( i < state.Actions.Length - 1 )
						Array.Copy(state.Actions, i + 1, dest, i, state.Actions.Length - i - 1);

					state.Actions = dest;
				}
			}
            
			state.SaveActions();
		}
	}
}
#endif