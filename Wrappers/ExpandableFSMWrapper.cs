using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandableFSMWrapper : 
	FSMWrapper<ExpandableFSMWrapper.Events, ExpandableFSMWrapper.States>,
	IFSMEventRouteSpecifier<ExpandableFSMWrapper.Events, ExpandableFSMWrapper.States>,
	IFSMWrappedVariableSpecifier
{
	
	
	public enum States
	{
		Expanded,
		Contracted
	}

	public enum Events
	{
		Expand,
		Contract
	}

	public Events[] EventsFrom(States state)
	{
		switch (state)
		{
			case States.Expanded:
				return new[] {Events.Contract};
			case States.Contracted:
				return new[] {Events.Expand};
		}

		return new Events[] { };
	}

	public FSMBoolWrapper isExpanded = new FSMBoolWrapper();

	public FSMVariableWrapper[] GetWrappedVariables()
	{
		return new FSMVariableWrapper[] {isExpanded};
	}

	public void Expand()
	{
		if (!isExpanded.val)
		{
			isExpanded.val = true;
			SendEvent(Events.Expand);
		}
	}

	public void Contract()
	{
		if (isExpanded.val)
		{
			isExpanded.val = false;
			SendEvent(Events.Contract);
		}
	}

	public void Toggle()
	{
		if (isExpanded.val)
		{
			isExpanded.val = false;
			SendEvent(Events.Contract);
		}
		else
		{
			isExpanded.val = true;
			SendEvent(Events.Expand);
		}
	}
}
