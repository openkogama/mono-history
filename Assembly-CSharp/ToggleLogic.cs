using UnityEngine;
using UnityEngine.Events;

public class ToggleLogic : ToggleHandler
{
	[SerializeField]
	private ToggleStatHandlerBase toggleStatHandlerBase;

	private void Update()
	{
		toggleStatHandlerBase.ToggleState = MVGameControllerBase.CameraController.IsLogicRendered;
	}

	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		MVGameControllerBase.CameraController.IsLogicRendered = toggleState;
		toggleCallback(toggleState);
	}
}
