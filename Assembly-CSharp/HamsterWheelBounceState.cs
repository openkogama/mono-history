using System.Collections.Generic;
using UnityEngine;

public class HamsterWheelBounceState
{
	private float maxHeight = 10f;

	private float impactVelSlopeNormalMinDot = 0.2f;

	private float minBounceVal = 40f;

	private float bounceStrengthThres = 10f;

	private Vector3 bounceVelocity = Vector3.zero;

	private bool bounced;

	private bool wasGrounded;

	private Vector3 addedVelocity = Vector3.zero;

	public bool Bounced => bounced;

	public HamsterWheelBounceState()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
	}

	private void SetValuesToTweakSheet()
	{
		maxHeight = AvatarTweakSheet.BounceState.maxHeight;
		impactVelSlopeNormalMinDot = AvatarTweakSheet.BounceState.impactVelSlopeNormalMinDot;
		minBounceVal = AvatarTweakSheet.BounceState.minBounceVal;
	}

	public Vector3 ApplyBounceVelocityMaterials(Vector3 velocity)
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

	public Vector3 ApplyBouncyness(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = velocity + addedVelocity;
		addedVelocity = Vector3.zero;
		return result;
	}

	public void UpdateBouncyness(List<MVControllerColliderHit> moveHits, Vector3 velocity, bool isGrounded)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		float num = 25f;
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			MVControllerColliderHit current = moveHit;
			float num2 = Vector3.Dot(current.hit.normal, Vector3.up);
			if (num2 > 0f && isGrounded && !wasGrounded && current.impactVelocity.y < 0f)
			{
				float num3 = Mathf.Min(0f - current.impactVelocity.y, num);
				addedVelocity += current.hit.normal * num3 * 1.42f;
			}
			if (num2 <= 0f)
			{
				float num4 = Mathf.Min(current.impactVelocity.magnitude, num);
				addedVelocity += current.hit.normal * num4 * 0.65f;
			}
		}
		wasGrounded = isGrounded;
	}

	public void UpdateBounceStateMaterial(List<MVControllerColliderHit> moveHits, MVInteractableBase interactableLocal)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		MVControllerColliderHit mVControllerColliderHit = default;
		bool flag = false;
		bounced = false;
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			MVControllerColliderHit current = moveHit;
			float num = interactableLocal.HandleModifierEffect(AvatarModifierEffect.Bounciness, current.material.physicalProperties.bouncyness);
			if (num > 0f)
			{
				Vector3 val = current.impactVelocity;
				val.Normalize();
				val = -val;
				if (Vector3.Dot(val, current.slopeNormal) > impactVelSlopeNormalMinDot && current.impactVelocity.magnitude * num > minBounceVal)
				{
					mVControllerColliderHit = current;
					flag = true;
					bounced = true;
					break;
				}
			}
		}
		if (flag)
		{
			float num2 = interactableLocal.HandleModifierEffect(AvatarModifierEffect.Bounciness, mVControllerColliderHit.material.physicalProperties.bouncyness);
			float targetJumpHeight = Mathf.Clamp(MVPhysics.CalculateJumpForceFromVerticalVelocity(mVControllerColliderHit.impactVelocity.magnitude) * num2, 0f, maxHeight * num2);
			float num3 = MVPhysics.CalculateJumpVerticalSpeed(targetJumpHeight);
			bounceVelocity = GetOutVectorFromInVector(mVControllerColliderHit.slopeNormal, mVControllerColliderHit.impactVelocity);
			bounceVelocity.Normalize();
			if (num3 < bounceStrengthThres)
			{
				num3 *= num3 / bounceStrengthThres;
			}
			bounceVelocity *= num3;
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
