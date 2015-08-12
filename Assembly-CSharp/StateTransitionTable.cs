using System.Collections.Generic;
using UnityEngine;

public abstract class StateTransitionTable
{
	protected Dictionary<object, IState> table = new Dictionary<object, IState>();

	public void SetState(object evt, IState state)
	{
		table.Add(evt, state);
	}

	public IState GetState(object evt)
	{
		IState state = null;
		try
		{
			return table[evt];
		}
		catch (KeyNotFoundException)
		{
			Debug.LogWarning("KeyNotFoundException found");
			return null;
		}
	}
}
