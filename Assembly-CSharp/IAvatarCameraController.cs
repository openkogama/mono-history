using System.Collections.Generic;

public interface IAvatarCameraController
{
	void Initialize(MVAvatarLocal avatarLocal);

	void ActivateCameraController();

	void SetCamera(CameraType cameraType);

	void SetCamera(MVCameraBase cameraBase);

	void PushCamera(CameraType cameraType);

	void PushCamera(MVCameraBase cameraBase);

	void RemoveCamera(CameraType cameraType);

	void RemoveCamera(MVCameraBase cameraBase);

	List<MVCameraBase> GetCameraBases();
}
