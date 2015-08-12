using UnityEngine;

public class CEAvatarAccessory : ESStateBase
{
	private MVWorldObjectClient currentBody;

	private JetPackMode jetPackMode;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	private AEditController EditController => MVGameController.EditController;

	private CharacterEditorController CharacterEditorController => MVGameController.CharacterEditorController;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		EditController.CubeModelingController.HideEditorTools();
		if (jetPackMode == null)
		{
			jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
		}
		jetPackMode.YMovementSpeedScale = 0f;
		jetPackMode.XZMovementSpeedScale = 0f;
		if (MVGameController.CharacterEditorController != null)
		{
			currentBody = MVGameController.CharacterEditorController.CurrentBody;
		}
		else
		{
			currentBody = MVGameController.Game.LocalPlayer.Avatar.Body;
		}
		MVGameController.Game.CameraController.SetCamera(CameraType.AvatarAccessory);
		MVGameController.Game.CameraController.CurCamera.FocusOnObject(currentBody);
		WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: false);
	}

	public override void Execute(EditorStateMachine esm)
	{
		base.Execute(esm);
	}

	public override void Exit(EditorStateMachine esm)
	{
		base.Exit(esm);
		MVGameController.Game.CameraController.SetCamera(CameraType.JetPackCamera);
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		MVAvatarLocal avatarLocal = MVGameController.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		MVGameController.Game.CameraController.CurCamera.FocusOnObject(currentBody);
		WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: true);
	}
}
