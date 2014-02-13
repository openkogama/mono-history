using UnityEngine;

public class CEAvatarAccessory : ESStateBase
{
	private MVWorldObjectClient currentBody;

	private JetPackMode jetPackMode;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private AEditController EditController => MVGameController.Instance.EditController;

	private CharacterEditorController CharacterEditorController => MVGameController.Instance.CharacterEditorController;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
		EditController.HideEditorTools();
		CharacterEditorController.HideAnimationToggles();
		CharacterEditorController.HideAvatarTools();
		if (jetPackMode == null)
		{
			jetPackMode = WOCM.AvatarLocal.AvatarModes.JetPackMode;
		}
		jetPackMode.YMovementSpeedScale = 0f;
		jetPackMode.XZMovementSpeedScale = 0f;
		currentBody = AvatarSelectionAnimator.Instance.Bodies[CharacterEditorController.CurrentBodyIndex];
		MVGameController.Instance.Game.CameraController.SetCamera(CameraType.AvatarAccessory);
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(currentBody);
		WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: false);
	}

	public override void Execute(EditorStateMachine esm)
	{
		base.Execute(esm);
	}

	public override void Exit(EditorStateMachine esm)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.Exit(esm);
		MVGameController.Instance.Game.CameraController.SetCamera(CameraType.JetPackCamera);
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(currentBody);
		WOCM.AvatarLocal.LaserPointer.SetLaserCubeVisible(visible: true);
	}
}
