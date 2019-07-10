using System.Collections.Generic;
using UnityEngine;

public class AvatarCamerasDesktopBuildMode : MonoBehaviour
{
	private AvatarCamerasWrapper avatarCamerasWrapper = new AvatarCamerasWrapper();

	private MVCameraController cameraController = new MVCameraController();

	[SerializeField]
	private JetPackCamera jetPackCamera;

	[SerializeField]
	private AvatarEditModeCamera avatarEditModeCamera;

	public void Initialize(MVBuildModeAvatarLocal avatarLocal)
	{
		jetPackCamera.Initialize(avatarLocal);
		avatarEditModeCamera.Initialize(avatarLocal);
		avatarCamerasWrapper.Add(jetPackCamera);
		avatarCamerasWrapper.Add(avatarEditModeCamera);
		cameraController.Initialize(GetCameraBases());
		if (!MVGameControllerBase.MainCameraManager.IsCameraControllerSet())
		{
			ActivateCameraController();
		}
	}

	public void ActivateCameraController()
	{
		MVGameControllerBase.MainCameraManager.SetCameraController(cameraController);
	}

	public void SetCamera(CameraType cameraType)
	{
		cameraController.SetCamera(cameraType);
	}

	public void SetCamera(MVCameraBase cameraBase)
	{
		cameraController.SetCamera(cameraBase);
	}

	public void PushCamera(CameraType cameraType)
	{
		cameraController.PushCamera(cameraType);
	}

	public void PushCamera(MVCameraBase cameraBase)
	{
		cameraController.PushCamera(cameraBase);
	}

	public void RemoveCamera(CameraType cameraType)
	{
		cameraController.RemoveCamera(cameraType);
	}

	public void RemoveCamera(MVCameraBase cameraBase)
	{
		cameraController.RemoveCamera(cameraBase);
	}

	public List<MVCameraBase> GetCameraBases()
	{
		return avatarCamerasWrapper.GetCameraBases();
	}
}
