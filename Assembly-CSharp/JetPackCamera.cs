using UnityEngine;

public class JetPackCamera : MVCameraBase
{
	private const float rotationSmoothTimeMouseControlled = 0.1f;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float aroundXInertia = 0.5f;

	public float aroundYInertiaMouseControlled = 0.5f;

	public float aroundYInertiaAvatarControlled = 1f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float scrollSpeed = 0.5f;

	public int xMinLimit = -87;

	public int xMaxLimit = 87;

	private Transform lookAtTransform;

	private Vector3 lookAtOffset;

	private float xAxisTarget;

	private float yAxisTarget;

	private float xAxis;

	private float yAxis;

	private float yAxisVelocity;

	private float xAxisVelocity;

	private float rotationSmoothTime = 0.1f;

	public Vector3 ComputeAvatarPositionFromTransform(Transform t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return t.position - Vector3.up * 2f;
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		lookAtTransform = ((Component)MVGameController.Instance.WOCM.WoAvatar.Avatar).transform;
		lookAtOffset = 2f * Vector3.up;
		((Component)this).transform.position = lookAtTransform.position + lookAtOffset;
		xAxisTarget = ((Component)camController).transform.eulerAngles.x;
		yAxisTarget = ((Component)camController).transform.eulerAngles.y;
		ResetRotationToTargetTransform(camController);
	}

	public override void Init(MVCameraController camController)
	{
		cameraType = CameraType.JetPackCamera;
		base.Init(camController);
	}

	private void ResetRotationToTargetTransform(MVCameraController camController)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = ((Component)camController).transform.eulerAngles;
		((Component)this).transform.eulerAngles = eulerAngles;
	}

	public override void HandleInput()
	{
		UpdateTargetRotation();
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		((Component)this).transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
		UpdatePosition();
		base.UpdateCamera(camController, targetTransform);
	}

	private void UpdatePosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = lookAtTransform.position + lookAtOffset;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void UpdateTargetRotation()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if ((Input.GetMouseButton(1) || Input.GetKey((KeyCode)120)) && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			float num = 30f;
			Vector3 val = new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * num, MVInputWrapper.GetAxisRaw("Mouse Y") * num, 0f);
			float num2 = Camera.mainCamera.fieldOfView / (float)Screen.height;
			yAxisTarget += val.x * num2;
			xAxisTarget += (0f - val.y) * num2;
			xAxisTarget = ClampAngle(xAxisTarget, xMinLimit, xMaxLimit);
			rotationSmoothTime = 0.1f;
		}
	}
}
