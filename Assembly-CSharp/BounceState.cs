using UnityEngine;

public class BounceState
{
	private const float maxHeight = 10f;

	private const float bounceMinVal = 1f;

	private const float bounceStrengthThres = 10f;

	private Vector3 bounceVelocity = Vector3.zero;

	private readonly MVInteractable interactable;

	private bool bounced;

	public bool Bounced => bounced;

	public BounceState(MVInteractable interactable)
	{
		this.interactable = interactable;
	}

	public Vector3 ApplyBounceVelocity(Vector3 velocity)
	{
		if (bounceVelocity != Vector3.zero)
		{
			velocity = bounceVelocity;
		}
		bounced = false;
		bounceVelocity = Vector3.zero;
		return velocity;
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		if (!moveHit.testWithOutMoving)
		{
			UpdateBounceState(moveHit);
		}
	}

	private void UpdateBounceState(MVControllerColliderHit mvControllerColliderHit)
	{
		MVControllerColliderHit mVControllerColliderHit = default;
		bool flag = false;
		bounced = false;
		float num = interactable.HandleModifierEffect(AvatarModifierEffect.Bounciness, mvControllerColliderHit.material.PhysicalProperties.bouncyness);
		if (num > 0f)
		{
			Vector3 impactVelocity = mvControllerColliderHit.impactVelocity;
			impactVelocity.Normalize();
			impactVelocity = -impactVelocity;
			float num2 = Vector3.Dot(impactVelocity, mvControllerColliderHit.slopeNormal);
			float num3 = mvControllerColliderHit.impactVelocity.magnitude * 0.5f;
			float num4 = num3 * num2;
			if (num4 > 1f)
			{
				mVControllerColliderHit = mvControllerColliderHit;
				flag = true;
				bounced = true;
			}
		}
		if (flag)
		{
			float targetJumpHeight = Mathf.Clamp(MVPhysics.CalculateJumpForceFromVerticalVelocity(mVControllerColliderHit.impactVelocity.magnitude) * num, 0f, 10f * num);
			float num5 = MVPhysics.CalculateJumpVerticalSpeed(targetJumpHeight);
			bounceVelocity = GetOutVectorFromInVector(mVControllerColliderHit.slopeNormal, mVControllerColliderHit.impactVelocity);
			bounceVelocity.Normalize();
			if (num5 < 10f)
			{
				num5 *= num5 / 10f;
			}
			bounceVelocity *= num5;
		}
	}

	private static Vector3 GetOutVectorFromInVector(Vector3 normal, Vector3 inVector)
	{
		inVector = -inVector;
		Vector3 vector = Vector3.Project(inVector, normal * inVector.magnitude);
		Vector3 vector2 = vector - inVector;
		return vector + vector2;
	}
}
