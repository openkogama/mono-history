using UnityEngine;
using UnityEngine.Events;

public class ToggleHandlerTest : ToggleHandler
{
	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		toggleCallback(toggleState);
		Debug.Log("Toggle is " + toggleState);
	}
}
