using System.Collections.Generic;
using UnityEngine;

public class BounceState
{
	private float maxHeight = 10f;

	private float impactVelSlopeNormalMinDot = 0.2f;

	private float minBounceVal = 2f;

	public Vector3 bounceVelocity = Vector3.zero;

	public bool bounced;

	public BounceState()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
	}

	private void SetValuesToTweakSheet()
	{
		maxHeight = AvatarTweakSheet.BounceState.maxHeight;
		impactVelSlopeNormalMinDot = AvatarTweakSheet.BounceState.impactVelSlopeNormalMinDot;
		minBounceVal = AvatarTweakSheet.BounceState.minBounceVal;
	}

	public Vector3 ApplyBounceVelocity(Vector3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (bounceVelocity != Vector3.zero)
		{
			velocity = bounceVelocity;
		}
		bounceVelocity = Vector3.zero;
		return velocity;
	}

	public void UpdateBounceState(List<MVControllerColliderHit> moveHits)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		MVControllerColliderHit mVControllerColliderHit = default;
		bool flag = false;
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			MVControllerColliderHit current = moveHit;
			if (current.material.physicalProperties.bouncyness > 0f)
			{
				Vector3 val = current.impactVelocity;
				val.Normalize();
				val = -val;
				if (Vector3.Dot(val, current.slopeNormal) > impactVelSlopeNormalMinDot && current.impactVelocity.magnitude * current.material.physicalProperties.bouncyness > minBounceVal)
				{
					mVControllerColliderHit = current;
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			float bouncyness = mVControllerColliderHit.material.physicalProperties.bouncyness;
			float targetJumpHeight = Mathf.Clamp(MVPhysics.CalculateJumpForceFromVerticalVelocity(mVControllerColliderHit.impactVelocity.magnitude) * bouncyness, 0f, maxHeight * bouncyness);
			float num = MVPhysics.CalculateJumpVerticalSpeed(targetJumpHeight);
			bounceVelocity = GetOutVectorFromInVector(mVControllerColliderHit.slopeNormal, mVControllerColliderHit.impactVelocity);
			bounceVelocity.Normalize();
			bounceVelocity *= num;
		}
	}

	private static Vector3 GetOutVectorFromInVector(Vector3 normal, Vector3 inVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		inVector = -inVector;
		Vector3 val = Vector3.Project(inVector, normal * inVector.magnitude);
		Vector3 val2 = val - inVector;
		return val + val2;
	}
}
