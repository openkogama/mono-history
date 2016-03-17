using UnityEngine.Events;

public class MuteToggleExecute : ToggleHandler
{
	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		MVCameraController.Mute = toggleState;
		toggleCallback(toggleState);
	}
}
