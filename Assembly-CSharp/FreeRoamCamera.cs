using UnityEngine;

public class FreeRoamCamera : MVCameraBase
{
	public int xMinLimit = -87;

	public int xMaxLimit = 87;

	private Vector3 lookAtOffset;

	public float xAxisTarget;

	public float yAxisTarget;

	public float xAxis;

	public float yAxis;

	private float yAxisVelocity;

	private float xAxisVelocity;

	private float rotationSmoothTime = 1.1f;

	private float movementSpeed = 10f;

	private float mouseSensitivity = 5f;

	private Vector3 currentVelocity = Vector3.zero;

	private MovementMap movementMap = new MovementMap();

	public FreeRoamCamera()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
	}

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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position + lookAtOffset;
		Transform transform = ((Component)this).transform;
		Vector3 val = new Vector3(0f, 100000f, 0f);
		MVGameController.Instance.WOCM.AvatarLocal.WorldPosition = val;
		transform.position = val;
		lookAtOffset = 2f * Vector3.up;
		xAxisVelocity = (yAxisVelocity = 0f);
		xAxis = (xAxisTarget = ((Component)camController).transform.eulerAngles.x);
		yAxis = (yAxisTarget = ((Component)camController).transform.eulerAngles.y);
		((Component)this).transform.eulerAngles = ((Component)camController).transform.eulerAngles;
	}

	public override void Exit(MVCameraController camController)
	{
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
		if (xMinLimit > xMaxLimit)
		{
			Debug.LogError((object)"xMaxLimit is less than xMinLimit!");
		}
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKey((KeyCode)308))
		{
			rotationSmoothTime = Mathf.Max(0.1f, rotationSmoothTime - MVInputWrapper.GetAxis("Mouse ScrollWheel"));
		}
		else
		{
			movementSpeed = Mathf.Max(0.1f, movementSpeed + MVInputWrapper.GetAxis("Mouse ScrollWheel") * 20f);
		}
		if ((ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			yAxisTarget += MVInputWrapper.GetAxisRaw("Mouse X") * mouseSensitivity;
			xAxisTarget += (0f - MVInputWrapper.GetAxisRaw("Mouse Y")) * mouseSensitivity;
			yAxisTarget = Mathf.Clamp(yAxisTarget, yAxis - 45f, yAxis + 45f);
			xAxisTarget = Mathf.Clamp(xAxisTarget, xAxis - 45f, xAxis + 45f);
			xAxisTarget = Mathf.Clamp(NormalizeAngle(xAxisTarget), (float)xMinLimit, (float)xMaxLimit);
			float num = 0f;
			if (MVInputWrapper.GetKey((KeyCode)101))
			{
				num = 1f;
			}
			else if (MVInputWrapper.GetKey((KeyCode)99))
			{
				num = -1f;
			}
			movementMap.HandleInputState();
			Vector3 val = ((Component)this).transform.rotation * movementMap.Direction + new Vector3(0f, num, 0f);
			val.Normalize();
			currentVelocity += val * movementSpeed * Time.deltaTime;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		currentVelocity *= 0.95f;
		Transform transform = ((Component)this).transform;
		transform.position += currentVelocity * Time.deltaTime;
		((Component)this).transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
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
	}
}
