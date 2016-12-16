using UnityEngine;

public class GodzillaCamera2D : MVCameraBase
{
	[SerializeField]
	private float rotOffset = 10f;

	[SerializeField]
	private float baseDistanceToAvatar = 4f;

	private float scale;

	public override CameraType CameraType => CameraType.GodzillaMode2DCamera;

	public void SetScale(float scale)
	{
		this.scale = scale;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		transform.eulerAngles = new Vector3(rotOffset, 0f, 0f);
		transform.position = MVGameControllerBase.WOCM.AvatarLocal.LookAtPos - transform.rotation * Vector3.forward * baseDistanceToAvatar * scale;
		base.UpdateCamera(camController, targetTransform);
	}
}
