using UnityEngine;

public class HamsterWheelBounceState
{
	private const float maxHeight = 10f;

	private const float minBounceVal = 4.5f;

	private const float bounceStrengthThres = 10f;

	private const float hamsterBallBouncyness = 0.6f;

	private readonly MVInteractableBase interactable;

	private Vector3 bounceVelocity = Vector3.zero;

	private bool bounced;

	public bool Bounced => bounced;

	public HamsterWheelBounceState(MVInteractableBase interactable)
	{
		this.interactable = interactable;
	}

	public Vector3 ApplyBounceVelocityMaterials(Vector3 velocity)
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
			UpdateBounceStateMaterial(moveHit);
		}
	}

	private void UpdateBounceStateMaterial(MVControllerColliderHit mvControllerColliderHit)
	{
		MVControllerColliderHit mVControllerColliderHit = default;
		bool flag = false;
		bounced = false;
		float num = interactable.HandleModifierEffect(AvatarModifierEffect.Bounciness, mvControllerColliderHit.material.physicalProperties.bouncyness + 0.6f);
		if (num > 0f)
		{
			Vector3 impactVelocity = mvControllerColliderHit.impactVelocity;
			impactVelocity.Normalize();
			impactVelocity = -impactVelocity;
			float num2 = Vector3.Dot(impactVelocity, mvControllerColliderHit.slopeNormal);
			if (num2 > 0f)
			{
				float num3 = num2 * mvControllerColliderHit.impactVelocity.magnitude * num;
				if (num3 > 4.5f)
				{
					mVControllerColliderHit = mvControllerColliderHit;
					flag = true;
					bounced = true;
				}
			}
		}
		if (flag)
		{
			float targetJumpHeight = Mathf.Clamp(MVPhysics.CalculateJumpForceFromVerticalVelocity(mVControllerColliderHit.impactVelocity.magnitude) * num, 0f, 10f * num);
			float num4 = MVPhysics.CalculateJumpVerticalSpeed(targetJumpHeight);
			bounceVelocity = GetOutVectorFromInVector(mVControllerColliderHit.slopeNormal, mVControllerColliderHit.impactVelocity);
			bounceVelocity.Normalize();
			if (num4 < 10f)
			{
				num4 *= num4 / 10f;
			}
			bounceVelocity *= num4;
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
