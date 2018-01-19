using System;
using System.Collections.Generic;
using UnityEngine;

public class LimbController
{
	private MVAvatar avatar;

	private Transform limbTransform;

	private Quaternion limbsOriginalRotation;

	private Quaternion modelRotationOffset;

	private Quaternion interpolateTowardsYawRotation;

	private Quaternion interpolateTowardsPitchRotation;

	private Quaternion previousLimbRotation;

	private float interpolationSpeed = 5f;

	private float elapsedInterpolationTime;

	private float maxYaw;

	private float maxPitch;

	private float rotationDuration;

	private float elapsedInterpolateAnimationTime;

	private bool shouldRotate;

	private string currentAnimation;

	private List<string> blendAnimations;

	private List<string> cancelAnimations;

	private bool isEventControllingLimb;

	public Quaternion InterpolateTowardsYawRotation => interpolateTowardsYawRotation;

	public Quaternion InterpolateTowardsPitchRotation => interpolateTowardsPitchRotation;

	public float InterpolationSpeed
	{
		get
		{
			return interpolationSpeed;
		}
		set
		{
			interpolationSpeed = value;
		}
	}

	public bool IsEventControllingLimb
	{
		set
		{
			isEventControllingLimb = value;
		}
	}

	public string CurrentAnimation
	{
		set
		{
			currentAnimation = value;
		}
	}

	public void Initialize(AvatarLimbManager limbManager, MVAvatar avatar, BodyData.PartIndex partIndex, Quaternion modelRotationOffset, Quaternion originalRotation, List<string> blendAnimations, List<string> cancelAnimations, float maxYaw, float maxPitch)
	{
		this.avatar = avatar;
		this.modelRotationOffset = modelRotationOffset;
		this.blendAnimations = blendAnimations;
		this.cancelAnimations = cancelAnimations;
		this.maxYaw = maxYaw;
		this.maxPitch = maxPitch;
		limbTransform = avatar.Body.BodyData.GetPartBone(partIndex).transform;
		previousLimbRotation = limbTransform.rotation;
		limbsOriginalRotation = originalRotation;
		limbManager.OnAvatarRotate = (Action)Delegate.Combine(limbManager.OnAvatarRotate, new Action(FinishInterpolation));
	}

	public void TrySetNewRotation(Quaternion yawRotation, Quaternion PitchRotation)
	{
		if (!isEventControllingLimb)
		{
			SetNewRotation(yawRotation, PitchRotation);
		}
	}

	public void TrySetNewRotation(Quaternion yawRotation, Quaternion PitchRotation, float duration)
	{
		if (!isEventControllingLimb)
		{
			SetNewRotation(yawRotation, PitchRotation, duration);
		}
	}

	public void SetNewRotation(Quaternion yawRotation, Quaternion PitchRotation)
	{
		ResetInterpolation();
		interpolateTowardsYawRotation = yawRotation;
		interpolateTowardsPitchRotation = PitchRotation;
	}

	public void SetNewRotation(Quaternion yawRotation, Quaternion PitchRotation, float duration)
	{
		SetNewRotation(yawRotation, PitchRotation);
		rotationDuration = duration;
	}

	public void StopRotating()
	{
		if (!isEventControllingLimb)
		{
			shouldRotate = false;
		}
	}

	public void UpdateRotation()
	{
		UpdateRotationDuration();
		if (shouldRotate)
		{
			if (IsCancelRotation(currentAnimation))
			{
				return;
			}
			Quaternion interpolateTowardsRotation = ((!ShouldBlendWithAnimation(currentAnimation)) ? AddAndClampRotations(interpolateTowardsYawRotation, interpolateTowardsPitchRotation) : CalculateBlendedRotation());
			UpdateInterpolation(interpolateTowardsRotation);
		}
		else
		{
			InterpolateTowardsAnimation(currentAnimation);
		}
		previousLimbRotation = limbTransform.rotation;
	}

	private void UpdateRotationDuration()
	{
		if (rotationDuration > 0f)
		{
			rotationDuration -= Time.deltaTime;
			if (rotationDuration <= 0f)
			{
				shouldRotate = false;
				elapsedInterpolateAnimationTime = 0f;
			}
		}
	}

	private bool IsCancelRotation(string currentAnimation)
	{
		for (int i = 0; i < cancelAnimations.Count; i++)
		{
			if (currentAnimation == cancelAnimations[i])
			{
				return true;
			}
		}
		return false;
	}

	private bool ShouldBlendWithAnimation(string currentAnimation)
	{
		if (isEventControllingLimb)
		{
			return false;
		}
		for (int i = 0; i < blendAnimations.Count; i++)
		{
			if (currentAnimation == blendAnimations[i])
			{
				return true;
			}
		}
		return false;
	}

	private Quaternion CalculateBlendedRotation()
	{
		Vector3 forward = limbTransform.forward;
		forward = avatar.Transform.InverseTransformPoint(avatar.Transform.position + forward);
		Quaternion yawRotation = GetYawRotation(forward);
		Quaternion pitchRotation = GetPitchRotation(forward);
		elapsedInterpolationTime = 0f;
		Quaternion rotation = interpolateTowardsYawRotation * yawRotation;
		Quaternion rotation2 = interpolateTowardsPitchRotation * pitchRotation;
		return AddAndClampRotations(rotation, rotation2);
	}

	private void UpdateInterpolation(Quaternion interpolateTowardsRotation)
	{
		elapsedInterpolationTime += Time.deltaTime * interpolationSpeed;
		Quaternion rotation = Quaternion.Lerp(previousLimbRotation, avatar.Transform.rotation * interpolateTowardsRotation * Quaternion.Inverse(limbsOriginalRotation) * modelRotationOffset, elapsedInterpolationTime);
		limbTransform.rotation = rotation;
	}

	private void InterpolateTowardsAnimation(string currentAnimation)
	{
		elapsedInterpolateAnimationTime += Time.deltaTime * interpolationSpeed;
		if (elapsedInterpolateAnimationTime < 1f + Time.deltaTime * interpolationSpeed && !IsCancelRotation(currentAnimation))
		{
			Quaternion rotation = Quaternion.Slerp(previousLimbRotation, limbTransform.rotation, elapsedInterpolateAnimationTime);
			limbTransform.rotation = rotation;
		}
	}

	private Quaternion GetYawRotation(Vector3 localDirection)
	{
		float angle = MathFunctions.SignedYawFromLocalDirection(localDirection);
		return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.up);
	}

	private Quaternion GetPitchRotation(Vector3 localDirection)
	{
		float angle = MathFunctions.PitchFromLocalDirection(localDirection);
		return MathFunctions.QuaternionFromAngleAndAxis(angle, Vector3.right);
	}

	private Quaternion AddAndClampRotations(Quaternion rotation1, Quaternion rotation2)
	{
		Quaternion result = rotation1 * rotation2;
		Vector3 eulerAngles = result.eulerAngles;
		if (eulerAngles.x > maxPitch && eulerAngles.x <= 180f)
		{
			eulerAngles.x = maxPitch;
		}
		if (eulerAngles.x < 360f - maxPitch && eulerAngles.x > 180f)
		{
			eulerAngles.x = 360f - maxPitch;
		}
		if (eulerAngles.y > maxYaw && eulerAngles.y <= 180f)
		{
			eulerAngles.y = maxYaw;
		}
		if (eulerAngles.y < 360f - maxYaw && eulerAngles.y > 180f)
		{
			eulerAngles.y = 360f - maxYaw;
		}
		result.eulerAngles = eulerAngles;
		return result;
	}

	public void StartBlendingWithAnimation(string animation)
	{
		for (int i = 0; i < blendAnimations.Count; i++)
		{
			if (blendAnimations[i] == animation)
			{
				return;
			}
		}
		blendAnimations.Add(animation);
	}

	public void StopBlendingWithAnimation(string animation)
	{
		for (int i = 0; i < blendAnimations.Count; i++)
		{
			if (blendAnimations[i] == animation)
			{
				blendAnimations.RemoveAt(i);
				break;
			}
		}
	}

	public void ResetInterpolation()
	{
		elapsedInterpolationTime = 0f;
		elapsedInterpolateAnimationTime = 0f;
		shouldRotate = true;
		rotationDuration = 0f;
	}

	private void FinishInterpolation()
	{
		elapsedInterpolationTime = 1f;
	}
}
