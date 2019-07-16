using System;
using UnityEngine;

public class JetPackCamera : MVCameraBase
{
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

	protected Vector3 lookAtOffset;

	protected float xAxisTarget;

	protected float yAxisTarget;

	protected float xAxis;

	protected float yAxis;

	protected float yAxisVelocity;

	protected float xAxisVelocity;

	private float rotationSmoothTime = 0.1f;

	private Camera mainCamera;

	private const float rotationSmoothTimeMouseControlled = 0.1f;

	protected MVBuildModeAvatarLocal avatarLocal;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	public override CameraType CameraType => CameraType.EditorCamera;

	public virtual void Initialize(MVBuildModeAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
	}

	public override void Enter(MVCameraController camController)
	{
		lookAtTransform = avatarLocal.GameObject.transform;
		lookAtOffset = 2f * Vector3.up;
		transform.position = lookAtTransform.position + lookAtOffset;
		xAxis = (xAxisTarget = MVGameControllerBase.MainCameraManager.transform.eulerAngles.x);
		yAxis = (yAxisTarget = MVGameControllerBase.MainCameraManager.transform.eulerAngles.y);
		mainCamera = Camera.main;
		ResetRotationToTargetTransform();
	}

	public override void Reset()
	{
		base.Reset();
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.y = avatarLocal.GameObject.transform.eulerAngles.y;
		yAxis = (yAxisTarget = eulerAngles.y);
		xAxis = (xAxisTarget = eulerAngles.x);
		transform.eulerAngles = eulerAngles;
	}

	private void ResetRotationToTargetTransform()
	{
		Vector3 eulerAngles = MVGameControllerBase.MainCameraManager.transform.eulerAngles;
		transform.eulerAngles = eulerAngles;
	}

	public void HandleInput(MVCameraController cameraController)
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt) && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			float num = 3f;
			Vector3 vector = new Vector3(MVInputWrapper.GetAxisRaw("Mouse X") * num, MVInputWrapper.GetAxisRaw("Mouse Y") * num, 0f);
			float num2 = mainCamera.fieldOfView / (float)Screen.height;
			yAxisTarget += vector.x * num2;
			xAxisTarget += (0f - vector.y) * num2;
			xAxisTarget = NormalizeAngle(xAxisTarget);
			xAxisTarget = Mathf.Clamp(xAxisTarget, xMinLimit, xMaxLimit);
			rotationSmoothTime = 0.1f;
		}
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		HandleInput(camController);
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
		transform.position = lookAtTransform.position + lookAtOffset;
		base.UpdateCamera(camController, targetTransform);
	}

	protected static float NormalizeAngle(float angle)
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

	public override void FocusOnObject(MVWorldObjectClient wo, float transitionTime = 2f, Vector3 avatarOffset = default(Vector3), Vector3 cameraOffset = default(Vector3))
	{
		float num = wo.ComputeObjectRadius();
		Debug.Log("r " + num);
		float num2 = mainCamera.fieldOfView * 0.5f * 0.7f;
		float num3 = num / Mathf.Tan(num2 * ((float)Math.PI / 180f));
		float num4 = num3;
		Vector3 worldPivot = wo.WorldPivot;
		Transform transform = avatarLocal.GameObject.transform;
		Vector3 vector = worldPivot - (transform.position + lookAtOffset);
		Vector3 position = transform.position;
		position += vector.normalized * (vector.magnitude - num4) + avatarOffset;
		transform.position = position;
		SetToPosition(transform.position);
		LookAt(worldPivot + cameraOffset);
		MVGameControllerBase.MainCameraManager.StartTransitionCam(transitionTime, soft: true);
	}

	public void FocusOnPointFromAvatarPosition(Vector3 focusPoint, Vector3 avatarPosition)
	{
		avatarLocal.Transform.position = GetLookAtAvatarPosition(avatarPosition);
		SetToPosition(avatarLocal.Transform.position);
		LookAt(focusPoint);
	}

	public void ResetDistanceAndDirectionToAvatar(Vector3 lookAtPosition)
	{
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		float magnitude = (mVSpawnPointBlue.WorldPosition - lookAtPosition).magnitude;
		Vector3 vector = avatarLocal.WorldPosition - lookAtPosition;
		vector.y = 0f;
		float magnitude2 = vector.magnitude;
		vector.Normalize();
		avatarLocal.WorldPosition += vector * (magnitude - magnitude2);
		FocusOnPosition(lookAtPosition);
	}

	public void FocusOnPosition(Vector3 lookAtPosition, float transitionTime = 2f)
	{
		Transform transform = avatarLocal.GameObject.transform;
		SetToPosition(transform.position);
		LookAt(lookAtPosition);
		MVGameControllerBase.MainCameraManager.StartTransitionCam(transitionTime, soft: true);
	}

	private void SetToPosition(Vector3 position)
	{
		transform.position = position + lookAtOffset;
	}

	private Vector3 GetLookAtAvatarPosition(Vector3 position)
	{
		return position - lookAtOffset;
	}

	private void LookAt(Vector3 position)
	{
		transform.LookAt(position);
		xAxis = (xAxisTarget = NormalizeAngle(transform.eulerAngles.x));
		yAxis = (yAxisTarget = transform.eulerAngles.y);
		xAxisVelocity = (yAxisVelocity = 0f);
	}
}
