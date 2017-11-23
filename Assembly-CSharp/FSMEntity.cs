using System.Collections.Generic;
using UnityEngine;

public class FSMEntity
{
	protected StateTransitionTable transitionTable;

	protected IState currentState;

	public object prevEvent;

	public object curEvent;

	public object nextEvent;

	protected string stateName;

	protected Dictionary<object, object> data = new Dictionary<object, object>();

	private bool clearStack = true;

	private Stack<EditorEvent> stateStack = new Stack<EditorEvent>();

	protected bool lockState;

	public Dictionary<object, object> Data => data;

	public bool LockState
	{
		get
		{
			return lockState;
		}
		set
		{
			lockState = value;
		}
	}

	public object Event
	{
		set
		{
			if (lockState)
			{
				Debug.LogWarning("State is locked, could not set state to: " + value);
				return;
			}
			nextEvent = value;
			if (value == null)
			{
				currentState.Exit(this);
				currentState = null;
				return;
			}
			IState state = transitionTable.GetState(value);
			if (state != null)
			{
				if (currentState != null)
				{
					currentState.Exit(this);
				}
				stateName = value.ToString();
				currentState = state;
				nextEvent = null;
				prevEvent = curEvent;
				curEvent = value;
				currentState.Enter(this);
				Data.Clear();
			}
			if (clearStack)
			{
				stateStack.Clear();
			}
			else
			{
				clearStack = true;
			}
		}
	}

	public virtual void Update()
	{
		if (currentState != null)
		{
			currentState.Execute(this);
		}
	}

	public void PushState(EditorEvent nextState)
	{
		if (!lockState)
		{
			PushState(nextState, EditorEvent.UndefinedState);
		}
	}

	public void PushState(EditorEvent nextState, EditorEvent overridePushState)
	{
		if (!lockState)
		{
			if (overridePushState == EditorEvent.UndefinedState)
			{
				stateStack.Push((EditorEvent)(int)curEvent);
			}
			else
			{
				stateStack.Push(overridePushState);
			}
			clearStack = false;
			Event = nextState;
		}
	}

	public bool PopState()
	{
		if (lockState)
		{
			return false;
		}
		if (stateStack.Count > 0)
		{
			EditorEvent editorEvent = stateStack.Pop();
			clearStack = false;
			Event = editorEvent;
			return true;
		}
		return false;
	}

	public void ClearStateStack()
	{
		if (0 < stateStack.Count)
		{
			EditorEvent editorEvent = stateStack.Pop();
			clearStack = true;
			Event = editorEvent;
		}
	}
}
