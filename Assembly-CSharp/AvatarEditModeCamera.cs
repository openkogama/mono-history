using UnityEngine;

public class AvatarEditModeCamera : JetPackCamera
{
	public override CameraType CameraType => CameraType.AvatarEditModeCamera;

	public void ResetPosition(Vector3 lookAtPosition)
	{
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition + Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		FocusOnPosition(lookAtPosition);
	}
}
