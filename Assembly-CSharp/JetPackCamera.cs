using System;
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		lookAtTransform = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform;
		lookAtOffset = 2f * Vector3.up;
		((Component)this).transform.position = lookAtTransform.position + lookAtOffset;
		xAxis = (xAxisTarget = ((Component)camController).transform.eulerAngles.x);
		yAxis = (yAxisTarget = ((Component)camController).transform.eulerAngles.y);
		ResetRotationToTargetTransform(camController);
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
		if (xMinLimit > xMaxLimit)
		{
			Debug.LogError((object)"xMaxLimit is less than xMinLimit!");
		}
	}

	public void SetCameraToAvatarEulerHack()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.eulerAngles = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.eulerAngles;
		xAxis = (xAxisTarget = ((Component)this).transform.eulerAngles.x);
		yAxis = (yAxisTarget = ((Component)this).transform.eulerAngles.y);
	}

	private void ResetRotationToTargetTransform(MVCameraController camController)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = ((Component)camController).transform.eulerAngles;
		((Component)this).transform.eulerAngles = eulerAngles;
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if ((Input.GetMouseButton(1) || Input.GetKey((KeyCode)120)) && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			float num = 30f;
			Vector3 val = new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * num, MVInputWrapper.GetAxisRaw("Mouse Y") * num, 0f);
			float num2 = Camera.mainCamera.fieldOfView / (float)Screen.height;
			yAxisTarget += val.x * num2;
			xAxisTarget += (0f - val.y) * num2;
			xAxisTarget = Mathf.Clamp(xAxisTarget, (float)xMinLimit, (float)xMaxLimit);
			rotationSmoothTime = 0.1f;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		((Component)this).transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
		((Component)this).transform.position = lookAtTransform.position + lookAtOffset;
		base.UpdateCamera(camController, targetTransform);
	}

	private static float NormalizeAngle(float angle)
	{
		while (angle < -180f)
		{
			angle += 360f;
		}
		while (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public override void FocusOnObject(MVWorldObjectClient wo)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		float num = wo.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * 0.8f;
		float num3 = num / Mathf.Tan(num2 * ((float)Math.PI / 180f));
		num3 = Mathf.Max(num3, 4f);
		Transform transform = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform;
		Vector3 worldCenter = SharedCubeFunctions.GetWorldCenter(wo.GameObject.transform);
		Vector3 val = worldCenter - transform.position;
		transform.position += val.normalized * (val.magnitude - num3);
		((Component)this).transform.position = transform.position + lookAtOffset;
		((Component)this).transform.LookAt(worldCenter);
		xAxis = (xAxisTarget = NormalizeAngle(((Component)this).transform.eulerAngles.x));
		yAxis = (yAxisTarget = ((Component)this).transform.eulerAngles.y);
		xAxisVelocity = (yAxisVelocity = 0f);
		MVGameController.Instance.Game.CameraController.StartTransitionCam(2f, soft: true);
	}
}
