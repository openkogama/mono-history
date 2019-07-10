using System.Collections.Generic;
using UnityEngine;

public class AvatarCamerasMobile : MonoBehaviour, IAvatarCameraController
{
	private AvatarCamerasWrapper avatarCamerasWrapper = new AvatarCamerasWrapper();

	private MVCameraController cameraController = new MVCameraController();

	[SerializeField]
	private AndroidFirstPersonCamera firstPersonMobileCamera;

	[SerializeField]
	private AndroidThirdPersonCamera thirdPersonMobileCamera;

	[SerializeField]
	private LobbyStateCamera lobbyStateCamera;

	[SerializeField]
	private TimeAttackFlagDebriefingCamera timeAttackFlagDebriefingCamera;

	[SerializeField]
	private TimeAttackFlagCountdownCamera timeAttackFlagCountdownCamera;

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		firstPersonMobileCamera.Initialize(avatarLocal);
		thirdPersonMobileCamera.Initialize(avatarLocal);
		lobbyStateCamera.Initialize(avatarLocal);
		timeAttackFlagDebriefingCamera.Initialize(avatarLocal);
		timeAttackFlagCountdownCamera.Initialize(avatarLocal);
		avatarCamerasWrapper.Add(firstPersonMobileCamera);
		avatarCamerasWrapper.Add(thirdPersonMobileCamera);
		avatarCamerasWrapper.Add(lobbyStateCamera);
		avatarCamerasWrapper.Add(timeAttackFlagDebriefingCamera);
		avatarCamerasWrapper.Add(timeAttackFlagCountdownCamera);
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
