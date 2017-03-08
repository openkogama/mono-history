using UnityEngine;

public class FirstPersonDeathCamera : MVCameraBase
{
	[Header("Settings")]
	[SerializeField]
	private Vector3 cameraOffset;

	[Tooltip("Strength of screen flash.")]
	[Range(0f, 1f)]
	[SerializeField]
	private float flashStrength;

	[SerializeField]
	[Header("Dependencies")]
	private GodzillaGUI gui;

	public override CameraType CameraType => CameraType.GodzillaModeMainCamera;

	public override void Awake()
	{
		gui.transform.parent = null;
	}

	private void OnDestroy()
	{
		if (gui != null)
		{
			Object.Destroy(gui.gameObject);
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
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
		transform.parent = MVGameControllerBase.WOCM.AvatarLocal.Body.BodyData.GetPartBone(BodyData.PartIndex.Head).transform;
		transform.Translate(cameraOffset * transform.lossyScale.y);
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		camController.AvatarCameraFade.enabled = true;
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = 1f;
		transform.parent = null;
	}
}
