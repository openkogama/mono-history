using System.Collections.Generic;
using UnityEngine;

public class MVInputHandlerPrioritizer : MonoBehaviour
{
	private List<IInputHandler> inputHandlers = new List<IInputHandler>();

	public void Register(IInputHandler inputHandler)
	{
		inputHandlers.Add(inputHandler);
		inputHandlers.Sort((IInputHandler a, IInputHandler b) => a.Priority - b.Priority);
	}

	public void Unregister(IInputHandler inputHandler)
	{
		inputHandlers.Remove(inputHandler);
	}

	public void Update()
	{
		try
		{
			for (int i = 0; i < inputHandlers.Count; i++)
			{
				IInputHandler inputHandler = inputHandlers[i];
				if (inputHandler.HandleInput())
				{
					break;
				}
			}
		}
		finally
		{
			MVInputWrapper.Reset();
		}
	}
}
