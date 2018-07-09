using UnityEngine;

public class FirstPersonDeathCamera : MVCameraBase
{
	[SerializeField]
	[Header("Settings")]
	private Vector3 cameraOffset;

	[Range(0f, 1f)]
	[SerializeField]
	[Tooltip("Strength of screen flash.")]
	private float flashStrength;

	[SerializeField]
	[Header("Dependencies")]
	private GodzillaGUI gui;

	public override CameraType CameraType => CameraType.GodzillaModeMainCamera;

	public override void Awake()
	{
		gui.transform.SetParent(null, worldPositionStays: false);
	}

	private void OnDestroy()
	{
		if (gui != null)
		{
			Object.Destroy(gui.gameObject);
		}
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		camController.AvatarCameraFade.enabled = false;
		gui.Flash(0.9f);
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = 0.9f;
		transform.SetParent(MVGameControllerBase.WOCM.AvatarLocal.Body.BodyData.GetPartBone(BodyData.PartIndex.Head).transform, worldPositionStays: false);
		transform.localPosition = cameraOffset;
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		camController.AvatarCameraFade.enabled = true;
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = 1f;
		transform.parent = null;
	}
}
