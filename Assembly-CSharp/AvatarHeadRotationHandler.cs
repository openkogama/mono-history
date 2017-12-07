using System;
using UnityEngine;

public class AvatarHeadRotationHandler : MonoBehaviour
{
	private const float maxYaw = 90f;

	private const float maxPitch = 45f;

	private const float cameraPitchOffset = 25f;

	private const float networkMessageInterval = 1f;

	private Vector3 previousLookDirection;

	private float idleTime;

	private AvatarLimbManager limbManager;

	private bool shouldLean = true;

	private bool isLocal;

	private bool shouldUseRemoteRotation;

	private Quaternion remoteYawRotation;

	private Quaternion remotePitchRotation;

	private float networkMessageCooldown;

	private bool shouldSendNetworkMessage;

	public void Initialize(bool isLocal, AvatarLimbManager limbManager)
	{
		this.isLocal = isLocal;
		this.limbManager = limbManager;
		remoteYawRotation = Quaternion.identity;
		remotePitchRotation = Quaternion.identity;
		limbManager.OnAvatarRotate = (Action)Delegate.Combine(limbManager.OnAvatarRotate, new Action(ResetIdleTimer));
	}

	public void UpdateRotation(Vector3 localLookDirection)
	{
		UpdateIdleTimer(localLookDirection);
		Quaternion identity = Quaternion.identity;
		Quaternion identity2 = Quaternion.identity;
		if (shouldUseRemoteRotation)
		{
			identity = remoteYawRotation;
			identity2 = remotePitchRotation;
		}
		else
		{
			identity = GetClampedYawRotation(localLookDirection);
			identity2 = GetClampedPitchRotation(localLookDirection);
			if (isLocal)
			{
				UpdateNetworkMessage(identity * identity2);
			}
		}
		RotateTorso(identity, identity2);
		RotateHead(identity, identity2);
		previousLookDirection = localLookDirection;
		shouldLean = true;
	}

	private void UpdateIdleTimer(Vector3 localLookDirection)
	{
		if ((localLookDirection - previousLookDirection).magnitude > 0.001f)
		{
			ResetIdleTimer();
			shouldSendNetworkMessage = true;
		}
		idleTime += Time.deltaTime;
		if (idleTime >= 2f)
		{
			limbManager.LimbRotator.StartBlendingWithAnimation(BodyData.PartIndex.Head, "Idle");
		}
	}

	private void UpdateNetworkMessage(Quaternion rotation)
	{
		networkMessageCooldown -= Time.deltaTime;
		if (networkMessageCooldown <= 0f && shouldSendNetworkMessage)
		{
			MVGameControllerBase.OperationRequests.UpdateHeadRotation(rotation);
			ResetNetworkMessageCooldown(1f);
		}
	}

	public void ResetNetworkMessageCooldown(float networkMessageDelay)
	{
		if (networkMessageDelay > networkMessageCooldown)
		{
			networkMessageCooldown = networkMessageDelay;
		}
		shouldSendNetworkMessage = false;
	}

	private void RotateTorso(Quaternion yawRotation, Quaternion pitchRotation)
	{
		float num = CalculateTorsoRotationModifier(yawRotation.eulerAngles.y);
		Quaternion limbPitchRotation = pitchRotation;
		limbPitchRotation.x /= num;
		limbManager.LimbRotator.TrySetLimbRotation(BodyData.PartIndex.Torso, Quaternion.identity, limbPitchRotation);
	}

	private void RotateHead(Quaternion yawRotation, Quaternion pitchRotation)
	{
		float num = CalculateHeadPitchRotationModifier(yawRotation.eulerAngles.y);
		Quaternion limbPitchRotation = pitchRotation;
		limbPitchRotation.x /= num;
		limbManager.LimbRotator.TrySetLimbRotation(BodyData.PartIndex.Head, yawRotation, limbPitchRotation);
	}

	private Quaternion GetClampedYawRotation(Vector3 localDirection)
	{
		float yaw = MathFunctions.SignedYawFromLocalDirection(localDirection);
		yaw = HandleYawDeadZone(yaw);
		float angle = Mathf.Clamp(yaw, -90f, 90f);
		return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.up);
	}

	private Quaternion GetClampedPitchRotation(Vector3 localDirection)
	{
		float pitch = MathFunctions.PitchFromLocalDirection(localDirection);
		pitch = HandleCameraPitchOffset(pitch);
		float angle = Mathf.Clamp(pitch, -45f, 45f);
		return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.right);
	}

	private float HandleCameraPitchOffset(float pitch)
	{
		if (pitch <= 0f)
		{
			return pitch;
		}
		if (pitch > 0f && pitch < 25f)
		{
			return 0f;
		}
		pitch -= 25f;
		return pitch;
	}

	private float HandleYawDeadZone(float yaw)
	{
		if (yaw < -135f)
		{
			yaw = 0f;
			shouldLean = false;
		}
		if (yaw > 135f)
		{
			yaw = 0f;
			shouldLean = false;
		}
		return yaw;
	}

	private float CalculateTorsoRotationModifier(float yawAngle)
	{
		if (yawAngle > 180f)
		{
			yawAngle = 360f - yawAngle;
		}
		if (!shouldLean)
		{
			yawAngle = 90f;
		}
		return 1.5f / (1.01f - yawAngle / 90f);
	}

	private float CalculateHeadPitchRotationModifier(float yawAngle)
	{
		if (yawAngle > 180f)
		{
			yawAngle = 360f - yawAngle;
		}
		if (!shouldLean)
		{
			yawAngle = 90f;
		}
		float num = 0.3f;
		return 1f / (1.01f - num - yawAngle / 90f + num * 2f);
	}

	public void SetRotationRemotely(float yaw, float pitch)
	{
		ResetIdleTimer();
		shouldUseRemoteRotation = true;
		remoteYawRotation.eulerAngles = new Vector3(0f, yaw, 0f);
		remoteYawRotation = ClampQuaternion(remoteYawRotation);
		remotePitchRotation.eulerAngles = new Vector3(pitch, 0f, 0f);
		remotePitchRotation = ClampQuaternion(remotePitchRotation);
	}

	private Quaternion ClampQuaternion(Quaternion rotation)
	{
		Vector3 eulerAngles = rotation.eulerAngles;
		float num = eulerAngles.x;
		float num2 = eulerAngles.y;
		if (num > 45f && num <= 180f)
		{
			num = 45f;
		}
		else if (num < 315f && num > 180f)
		{
			num = 315f;
		}
		if (num2 > 90f && num2 <= 180f)
		{
			num2 = 90f;
		}
		else if (num2 < 270f && num2 > 180f)
		{
			num2 = 270f;
		}
		eulerAngles.x = num;
		eulerAngles.y = num2;
		rotation.eulerAngles = eulerAngles;
		return rotation;
	}

	private void ResetIdleTimer()
	{
		idleTime = 0f;
		limbManager.LimbRotator.StopBlendingWithAnimation(BodyData.PartIndex.Head, "Idle");
	}

	public void ResetIdleTimer(EmoteTypes emoteType)
	{
		ResetIdleTimer();
	}
}
