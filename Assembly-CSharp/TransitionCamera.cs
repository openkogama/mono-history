using UnityEngine;

public class TransitionCamera : MVCameraBase
{
	private float fieldOfView;

	private float rotPercentage = 1f;

	private bool superSoft;

	private Vector3 prevCameraPosition;

	private Quaternion prevCameraRotation;

	private float time = 5f;

	public override float FieldOfView => fieldOfView;

	public float RotPercentage => rotPercentage;

	public override CameraType CameraType => CameraType.TransitionCamera;

	public void InitTransition(MVCameraController camController, Transform targetCameraTransform, float transitionTime = 2f, bool soft = false)
	{
		prevCameraPosition = camController.transform.position;
		prevCameraRotation = camController.transform.localRotation;
		fieldOfView = camController.FieldOfView;
		transform.position = prevCameraPosition;
		transform.localRotation = prevCameraRotation;
		time = transitionTime;
		superSoft = soft;
		rotPercentage = 0f;
	}

	public void AbortTransition()
	{
		rotPercentage = 1f;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		rotPercentage += 1f / time * Time.deltaTime;
		if (rotPercentage > 1f)
		{
			rotPercentage = 1f;
		}
		if (superSoft)
		{
			Quaternion quaternion = RotateTowardsX(transform.eulerAngles, camController.CurCamera.transform.eulerAngles, rotPercentage);
			Quaternion quaternion2 = RotateTowardsY(transform.eulerAngles, camController.CurCamera.transform.eulerAngles, rotPercentage);
			transform.position = Vector3.Slerp(transform.position, camController.CurCamera.transform.position, rotPercentage);
			transform.localRotation = quaternion2 * quaternion;
		}
		else
		{
			Quaternion quaternion3 = RotateTowardsX(prevCameraRotation.eulerAngles, camController.CurCamera.transform.eulerAngles, rotPercentage);
			Quaternion quaternion4 = RotateTowardsY(prevCameraRotation.eulerAngles, camController.CurCamera.transform.eulerAngles, rotPercentage);
			transform.position = Vector3.Slerp(prevCameraPosition, camController.CurCamera.transform.position, rotPercentage);
			transform.localRotation = quaternion4 * quaternion3;
		}
		base.UpdateCamera(camController, targetTransform);
	}

	private Quaternion RotateTowardsY(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		eulerTo.x = 0f;
		eulerTo.z = 0f;
		Quaternion b = Quaternion.Euler(eulerTo);
		eulerFrom.x = 0f;
		eulerFrom.z = 0f;
		Quaternion a = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(a, b, percentage);
	}

	private Quaternion RotateTowardsX(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		eulerTo.y = 0f;
		eulerTo.z = 0f;
		Quaternion b = Quaternion.Euler(eulerTo);
		eulerFrom.y = 0f;
		eulerFrom.z = 0f;
		Quaternion a = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(a, b, percentage);
	}
}
