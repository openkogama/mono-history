using UnityEngine;

public class VehicleCamera : PlaymodeCamera
{
	private float rotationAroundY;

	private Transform originalTransformParent;

	public Transform LookAtTransform;

	public float RotationAroundY
	{
		get
		{
			return rotationAroundY;
		}
		set
		{
			rotationAroundY = value;
		}
	}

	public float GetSignedAnglePitch
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			Quaternion rotation = ((Component)this).transform.rotation;
			Vector3 normal = rotation * Vector3.right;
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.x = 0f;
			Vector3 v = Quaternion.Euler(eulerAngles) * Vector3.forward;
			Vector3 v2 = rotation * Vector3.forward;
			return MathFunctions.SignedAngle(v2, v, normal);
		}
	}

	public override void Respawn()
	{
		base.Respawn();
		rotationAroundY = 0f;
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
		Debug.Log((object)"VehicleCamera init");
		originalTransformParent = ((Component)this).transform.parent;
		((Component)this).transform.parent = null;
	}

	public override void Enter(MVCameraController cameraController)
	{
		base.Enter(cameraController);
		lookAtTransform = LookAtTransform;
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		Debug.Log((object)"VehicleCamera exit");
		((Component)this).transform.parent = originalTransformParent;
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		base.HandleInput(cameraController);
		UpdateTargetRotation();
	}

	private void UpdateTargetRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = targetRot.eulerAngles;
		float num = 0f - eulerAngles.x;
		float y = eulerAngles.y;
		autoRotate = Screen.lockCursor;
		if (autoRotate && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			rotationAroundY += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
			rotationAroundY = Mathf.Clamp(rotationAroundY, -90f, 90f);
			Quaternion rotation = lookAtTransform.rotation;
			Quaternion val = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
			Quaternion val2 = Quaternion.AngleAxis(rotationAroundY, Vector3.up);
			Quaternion val3 = val2 * val;
			y = val3.eulerAngles.y;
			num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
			aroundYInertia = aroundYInertiaMouseControlled;
		}
		num = ((!(num < -180f)) ? Mathf.Clamp(num, 0f - maximumY, 1000f) : Mathf.Clamp(num, -1000f, -360f - minimumY));
		eulerAngles = new Vector3(0f - num, y, 0f);
		targetRot = Quaternion.Euler(eulerAngles);
	}
}
