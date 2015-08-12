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

	public override CameraType CameraType => CameraType.JetPackCamera;

	public Vector3 ComputeAvatarPositionFromTransform(Transform t)
	{
		return t.position - Vector3.up * 2f;
	}

	public override void Enter(MVCameraController camController)
	{
		lookAtTransform = MVGameController.WOCM.AvatarLocal.GameObject.transform;
		lookAtOffset = 2f * Vector3.up;
		transform.position = lookAtTransform.position + lookAtOffset;
		xAxis = (xAxisTarget = camController.transform.eulerAngles.x);
		yAxis = (yAxisTarget = camController.transform.eulerAngles.y);
		ResetRotationToTargetTransform(camController);
	}

	public void SetCameraToAvatarEulerHack()
	{
		transform.eulerAngles = MVGameController.WOCM.AvatarLocal.GameObject.transform.eulerAngles;
		xAxis = (xAxisTarget = transform.eulerAngles.x);
		yAxis = (yAxisTarget = transform.eulerAngles.y);
	}

	private void ResetRotationToTargetTransform(MVCameraController camController)
	{
		Vector3 eulerAngles = camController.transform.eulerAngles;
		transform.eulerAngles = eulerAngles;
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt) && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			float num = 30f;
			Vector3 vector = new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * num, MVInputWrapper.GetAxisRaw("Mouse Y") * num, 0f);
			float num2 = Camera.main.fieldOfView / (float)Screen.height;
			yAxisTarget += vector.x * num2;
			xAxisTarget += (0f - vector.y) * num2;
			xAxisTarget = NormalizeAngle(xAxisTarget);
			xAxisTarget = Mathf.Clamp(xAxisTarget, xMinLimit, xMaxLimit);
			rotationSmoothTime = 0.1f;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
		transform.position = lookAtTransform.position + lookAtOffset;
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
		float num = wo.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * 0.8f;
		float a = num / Mathf.Tan(num2 * ((float)Math.PI / 180f));
		a = Mathf.Max(a, 4f);
		Transform transform = MVGameController.WOCM.AvatarLocal.GameObject.transform;
		Vector3 worldCenter = SharedCubeFunctions.GetWorldCenter(wo.GameObject.transform);
		Vector3 vector = worldCenter - transform.position;
		transform.position += vector.normalized * (vector.magnitude - a);
		base.transform.position = transform.position + lookAtOffset;
		base.transform.LookAt(worldCenter);
		xAxis = (xAxisTarget = NormalizeAngle(base.transform.eulerAngles.x));
		yAxis = (yAxisTarget = base.transform.eulerAngles.y);
		xAxisVelocity = (yAxisVelocity = 0f);
		MVGameController.Game.CameraController.StartTransitionCam(2f, soft: true);
	}
}
