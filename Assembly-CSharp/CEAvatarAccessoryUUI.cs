using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class CEAvatarAccessoryUUI : ESStateBase
{
	private MVWorldObjectClient currentBody;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditUIState x, BaseEventData y) =>
		{
			x.Set(ActiveEditStateUI.AvatarManagement);
		});
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(0f, 0f);
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(SetBodyFocus);
			});
		}
		else
		{
			currentBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
		}
	}

	private void SetBodyFocus(MVBody body)
	{
		currentBody = body;
		MVGameControllerBase.CameraController.SetCamera(CameraType.AvatarAccessory);
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(currentBody);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: false);
	}

	public override void Execute(EditorStateMachine esm)
	{
		base.Execute(esm);
	}

	public override void Exit(EditorStateMachine esm)
	{
		base.Exit(esm);
		MVGameControllerBase.CameraController.SetCamera(CameraType.AvatarEditModeCamera);
		((AvatarEditModeCamera)MVGameControllerBase.CameraController.CurCamera).ResetPosition(currentBody.Transform.position + Vector3.up);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: true);
	}
}
