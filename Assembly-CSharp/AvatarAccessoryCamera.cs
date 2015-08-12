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

	private Vector3 avatarBodyCenter => focusObject.WorldPosition + Vector3.up * cameraHeightOffset;

	public override CameraType CameraType => CameraType.AvatarAccessory;

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		angle = (float)Math.PI;
		lookAtOffset = 2f * Vector3.up;
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		angleVelocity = 0f;
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt))
		{
			angleVelocity = MVInputWrapper.GetAxisRaw("Mouse X") * mouseSensitivity;
			return;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft))
		{
			angleVelocity = 2f;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight))
		{
			angleVelocity = -2f;
		}
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		angle += angleVelocity * Time.deltaTime;
		angle %= (float)Math.PI * 2f;
		Vector2 vector = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized * rotationRadius;
		Vector3 position = avatarBodyCenter + new Vector3(vector.x, 0f, vector.y) - lookAtOffset;
		MVGameController.WOCM.AvatarLocal.GameObject.transform.position = position;
		transform.position = MVGameController.WOCM.AvatarLocal.GameObject.transform.position + lookAtOffset;
		Vector2 vector2 = new Vector2(Mathf.Sin(angle + cameraLookAtAngleOffset), Mathf.Cos(angle + cameraLookAtAngleOffset)).normalized * rotationRadius;
		transform.LookAt(new Vector3(vector2.x, 0f, vector2.y) + avatarBodyCenter);
		base.UpdateCamera(camController, targetTransform);
	}

	public override void FocusOnObject(MVWorldObjectClient wo)
	{
		focusObject = wo;
	}
}
