using UnityEngine;

public class CEAvatarAccessory : ESStateBase
{
	private MVWorldObjectClient currentBody;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		MVGameControllerLegacyUI.CharacterEditorController.HideEditorTools();
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(0f, 0f);
		if (MVGameControllerLegacyUI.CharacterEditorController != null)
		{
			currentBody = MVGameControllerLegacyUI.CharacterEditorController.CurrentBody;
		}
		else
		{
			currentBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
		}
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
		MVGameControllerBase.CameraController.SetCamera(CameraType.EditorCamera);
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(currentBody);
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: true);
	}
}
