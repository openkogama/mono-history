using UnityEngine;

public class MVCameraBase : MonoBehaviour
{
	protected CameraType cameraType = CameraType.None;

	protected IgnoreInputTypes ignoreInputTypes;

	public CameraType CameraType => cameraType;

	public virtual void HandleInput()
	{
	}

	private void camController_onIgnoreInputTypes(object sender, OnIgnoreInputTypesArgs e)
	{
		ignoreInputTypes = e.inputTypes;
	}

	public virtual void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		targetTransform.position = ((Component)this).transform.position;
		targetTransform.rotation = ((Component)this).transform.rotation;
	}

	public virtual void Init(MVCameraController camController)
	{
		camController.onIgnoreInputTypes += camController_onIgnoreInputTypes;
	}

	public virtual void Enter(MVCameraController camController)
	{
	}

	public virtual void Exit(MVCameraController camController)
	{
	}

	public virtual void DrawPlaneMoved(Vector3 to)
	{
	}

	public virtual void Shake(float duration, float strength)
	{
	}

	public virtual void Respawn()
	{
	}
}
