using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableFSMWrapper :
	FSMWrapper<DroppableFSMWrapper.Events, DroppableFSMWrapper.States>,
	IFSMEventRouteSpecifier<DroppableFSMWrapper.Events, DroppableFSMWrapper.States>,
	IFSMWrappedVariableSpecifier,
	IDroppableDroppedHandler,
	IDroppableHoverHandler
{
	public enum Events
	{
		StartHover,
		Dropped,
		EndHover
	}
	
	public enum States
	{
		NotHovering,
		Hovering
	}

	public Events[] EventsFrom(States state)
	{
		switch (state)
		{
			case States.NotHovering:
				return new Events[] { Events.StartHover };
			case States.Hovering:
				return new Events[] { Events.Dropped, Events.EndHover };
		}
		return new Events[] { };
	}

	public FSMVariableWrapper[] GetWrappedVariables()
	{
		return new FSMVariableWrapper[]
		{
			dragging, dropped
		};
	}
	
	[System.Serializable]
	public class DraggableWrapper : FSMBehaviourWrapper<Draggable> {}
	[HiddenFSMVariable]
	public DraggableWrapper dragging;
	[HiddenFSMVariable]
	public DraggableWrapper dropped;

	public void OnHoverStart(Draggable draggable)
	{
		dragging.val = draggable;
		SendEvent(Events.StartHover);
	}

	public void OnHoverEnd(Draggable draggable)
	{
		dragging.val = draggable;
		if (currentState == States.Hovering)
		{
			SendEvent(Events.EndHover);
		}
	}

	public void OnDropped(Draggable draggable)
	{
		dropped.val = draggable;
		SendEvent(Events.Dropped);
	}
}
