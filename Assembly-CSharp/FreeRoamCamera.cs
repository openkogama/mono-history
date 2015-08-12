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

	public override CameraType CameraType => CameraType.FreeRoam;

	public Vector3 ComputeAvatarPositionFromTransform(Transform t)
	{
		return t.position - Vector3.up * 2f;
	}

	public override void Enter(MVCameraController camController)
	{
		base.transform.position = MVGameController.WOCM.AvatarLocal.GameObject.transform.position + lookAtOffset;
		Transform transform = base.transform;
		Vector3 vector = new Vector3(0f, 100000f, 0f);
		MVGameController.WOCM.AvatarLocal.WorldPosition = vector;
		transform.position = vector;
		lookAtOffset = 2f * Vector3.up;
		xAxisVelocity = (yAxisVelocity = 0f);
		xAxis = (xAxisTarget = camController.transform.eulerAngles.x);
		yAxis = (yAxisTarget = camController.transform.eulerAngles.y);
		base.transform.eulerAngles = camController.transform.eulerAngles;
	}

	public override void Exit(MVCameraController camController)
	{
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.AlternateCameraControls))
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
			xAxisTarget = Mathf.Clamp(NormalizeAngle(xAxisTarget), xMinLimit, xMaxLimit);
			float y = 0f;
			if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveUp))
			{
				y = 1f;
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDown))
			{
				y = -1f;
			}
			movementMap.HandleInputState(fromFrameUpdate: true);
			Vector3 vector = transform.rotation * movementMap.Direction + new Vector3(0f, y, 0f);
			vector.Normalize();
			currentVelocity += vector * movementSpeed * Time.deltaTime;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		currentVelocity *= 0.95f;
		transform.position += currentVelocity * Time.deltaTime;
		transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
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
