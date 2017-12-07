using System;
using UnityEngine;

public class AvatarPointingHandler : MonoBehaviour
{
	private const float maxYaw = 90f;

	private const float maxPitch = 90f;

	public Action<bool> OnIsPointingChange;

	private Vector3 prevLookDirection;

	private AvatarLimbManager limbManager;

	private float pointingDuration = 0.8f;

	private float elapsedPointingTime;

	private Vector3 pointingDirection;

	private bool shouldPoint = true;

	private bool isLocal;

	private Quaternion remoteYawRotation;

	private Quaternion remotePitchRotation;

	private float networkMessageCooldown;

	private bool isActive = true;

	public void Initialize(bool isLocal, AvatarLimbManager limbManager)
	{
		this.limbManager = limbManager;
		this.isLocal = isLocal;
		remoteYawRotation = Quaternion.identity;
		remotePitchRotation = Quaternion.identity;
		if (!isLocal)
		{
			pointingDuration = 1.5f;
		}
	}

	public void StartPointing()
	{
		if (isActive)
		{
			elapsedPointingTime = pointingDuration;
			pointingDirection = prevLookDirection;
			if (OnIsPointingChange != null)
			{
				OnIsPointingChange(obj: true);
			}
		}
	}

	public void UpdatePointing(Vector3 localLookDirection)
	{
		elapsedPointingTime -= Time.deltaTime;
		if (elapsedPointingTime > 0f)
		{
			Quaternion clampedYawRotation;
			Quaternion clampedPitchRotation;
			if (isLocal)
			{
				clampedYawRotation = GetClampedYawRotation(pointingDirection);
				clampedPitchRotation = GetClampedPitchRotation(pointingDirection);
				UpdateNetworkMessage(clampedYawRotation * clampedPitchRotation);
			}
			else
			{
				clampedYawRotation = remoteYawRotation;
				clampedPitchRotation = remotePitchRotation;
			}
			if (shouldPoint)
			{
				HandlePointing(clampedYawRotation, clampedPitchRotation);
			}
			else
			{
				StopPointing();
			}
		}
		else if (OnIsPointingChange != null)
		{
			OnIsPointingChange(obj: false);
		}
		shouldPoint = true;
		prevLookDirection = localLookDirection;
	}

	private void UpdateNetworkMessage(Quaternion rotation)
	{
		networkMessageCooldown -= Time.deltaTime;
		if (networkMessageCooldown <= 0f)
		{
			if (shouldPoint)
			{
				MVGameControllerBase.OperationRequests.UpdatePointingAndHeadRotation(rotation);
			}
			else
			{
				MVGameControllerBase.OperationRequests.UpdateHeadRotation(Quaternion.identity);
			}
			ResetNetworkMessageDelay(pointingDuration);
			if (limbManager.DelayHeadRotationNetworkMessage != null)
			{
				limbManager.DelayHeadRotationNetworkMessage(1.1f);
			}
		}
	}

	public void ResetNetworkMessageDelay(float networkMessageDelay)
	{
		networkMessageCooldown = networkMessageDelay;
	}

	private void HandlePointing(Quaternion yawRotation, Quaternion pitchRotation)
	{
		if (yawRotation.eulerAngles.y < 180f || yawRotation.eulerAngles.y > 340f)
		{
			limbManager.LimbRotator.SetLimbRotation(BodyData.PartIndex.RArm, yawRotation, pitchRotation, elapsedPointingTime);
			limbManager.LimbRotator.StopLimbRotation(BodyData.PartIndex.LArm);
		}
		else
		{
			limbManager.LimbRotator.SetLimbRotation(BodyData.PartIndex.LArm, yawRotation, pitchRotation, elapsedPointingTime);
			limbManager.LimbRotator.StopLimbRotation(BodyData.PartIndex.RArm);
		}
	}

	private void StopPointing()
	{
		limbManager.LimbRotator.StopLimbRotation(BodyData.PartIndex.RArm);
		limbManager.LimbRotator.StopLimbRotation(BodyData.PartIndex.LArm);
	}

	private Quaternion GetClampedYawRotation(Vector3 localDirection)
	{
		float num = MathFunctions.SignedYawFromLocalDirection(localDirection);
		HandleYawDeadZone(num);
		float angle = Mathf.Clamp(num, -90f, 90f);
		return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.up);
	}

	private Quaternion GetClampedPitchRotation(Vector3 localDirection)
	{
		float value = MathFunctions.PitchFromLocalDirection(localDirection);
		float angle = Mathf.Clamp(value, -90f, 90f);
		return Quaternion.AngleAxis(angle, Vector3.right);
	}

	private void HandleYawDeadZone(float yaw)
	{
		if (yaw < -135f)
		{
			shouldPoint = false;
		}
		if (yaw > 135f)
		{
			shouldPoint = false;
		}
	}

	public void SetRotationRemotely(float yaw, float pitch)
	{
		if (isActive)
		{
			remoteYawRotation.eulerAngles = new Vector3(0f, yaw, 0f);
			remotePitchRotation.eulerAngles = new Vector3(pitch, 0f, 0f);
			elapsedPointingTime = pointingDuration;
			if (OnIsPointingChange != null)
			{
				OnIsPointingChange(obj: true);
			}
		}
	}

	private void OnDisable()
	{
		isActive = false;
	}

	private void OnEnable()
	{
		isActive = true;
	}
}
