using System;
using UnityEngine;

public class AvatarAccessoryCamera : MVCameraBase
{
	public float sensitivityX = 15f;

	public float rotationRadius = 6f;

	public float cameraLookAtAngleOffset = 1.9f;

	public float cameraHeightOffset = 2f;

	public float mouseSensitivity = 10f;

	private Vector3 lookAtOffset;

	private Transform lookAtTransform;

	private MVWorldObjectClient focusObject;

	private float angle;

	private float angleVelocity;

	private Vector3 avatarBodyCenter
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return focusObject.WorldPosition + Vector3.up * cameraHeightOffset;
		}
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		base.Enter(camController);
		angle = (float)Math.PI;
		lookAtOffset = 2f * Vector3.up;
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		angleVelocity = 0f;
		if (Input.GetMouseButton(1))
		{
			angleVelocity = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
			return;
		}
		if (Input.GetKey((KeyCode)97) || Input.GetKey((KeyCode)276))
		{
			angleVelocity = 2f;
		}
		if (Input.GetKey((KeyCode)100) || Input.GetKey((KeyCode)275))
		{
			angleVelocity = -2f;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		angle += angleVelocity * Time.deltaTime;
		angle %= (float)Math.PI * 2f;
		Vector2 val = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
		Vector2 val2 = val.normalized * rotationRadius;
		Vector3 position = avatarBodyCenter + new Vector3(val2.x, 0f, val2.y) - lookAtOffset;
		MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position = position;
		((Component)this).transform.position = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position + lookAtOffset;
		Vector2 val3 = new Vector2(Mathf.Sin(angle + cameraLookAtAngleOffset), Mathf.Cos(angle + cameraLookAtAngleOffset));
		Vector2 val4 = val3.normalized * rotationRadius;
		((Component)this).transform.LookAt(new Vector3(val4.x, 0f, val4.y) + avatarBodyCenter);
		base.UpdateCamera(camController, targetTransform);
	}

	public override void FocusOnObject(MVWorldObjectClient wo)
	{
		focusObject = wo;
	}
}
