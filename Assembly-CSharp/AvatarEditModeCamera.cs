using UnityEngine;

public class AvatarEditModeCamera : JetPackCamera
{
	public override CameraType CameraType => CameraType.AvatarEditModeCamera;

	public void ResetPosition(Vector3 lookAtPosition)
	{
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		Debug.LogWarning("Convert this to build mode avatar camera owned");
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition + Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		FocusOnPosition(lookAtPosition);
	}
}
