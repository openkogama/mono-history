using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InputToPlayerMovementAndroid : IInputToPlayerMovement
{
	private bool jump;

	private bool jumpFrameUpdate;

	private bool jumpFixedUpdate;

	private Vector3 direction = Vector3.zero;

	public Vector3 Direction => direction;

	public bool Jump => jump || jumpFrameUpdate || jumpFixedUpdate;

	public void HandleInputState(bool fromFrameUpdate)
	{
		if (MVGameControllerBase.PlayModeUI.InLobbyState || MVInputWrapper.IsInGameInputSuppressed)
		{
			return;
		}
		float axis = CrossPlatformInputManager.GetAxis("Vertical");
		float axis2 = CrossPlatformInputManager.GetAxis("Horizontal");
		direction = new Vector3(axis2, 0f, axis);
		if (MVInputWrapper.GetBooleanControl(KogamaControls.Jump))
		{
			if (fromFrameUpdate)
			{
				jumpFrameUpdate = true;
			}
			else
			{
				jumpFixedUpdate = true;
			}
		}
		if (!fromFrameUpdate)
		{
			jump = jumpFrameUpdate || jumpFixedUpdate;
			jumpFrameUpdate = (jumpFixedUpdate = false);
		}
	}
}
