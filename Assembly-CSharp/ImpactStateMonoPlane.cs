using System.Collections.Generic;
using UnityEngine;

public class ImpactStateMonoPlane
{
	private const int suspendImpactDamageFrames = 1;

	public Vector3 prevVelocityChangeVector = Vector3.zero;

	private bool collidedPrevFrame;

	private MVCollisionFlags collisionFlagsPrevFrame;

	private float averageSoftnessPrevFrame = 1f;

	private Dictionary<MVCollisionFlags, float[]> accThresMultipliers = new Dictionary<MVCollisionFlags, float[]>
	{
		{
			MVCollisionFlags.Above,
			new float[2] { 0f, 10000f }
		},
		{
			MVCollisionFlags.Below,
			new float[2] { 35f, 10000f }
		},
		{
			MVCollisionFlags.Sides,
			new float[2] { 0f, 10000f }
		}
	};

	private float impactDamage;

	private int suspendImpactDamageCounter = 1;

	private List<MVControllerColliderHit> moveHits = new List<MVControllerColliderHit>();

	public float ImpactDamage => impactDamage;

	public void SuspendImpactDamage()
	{
		suspendImpactDamageCounter = 1;
		prevVelocityChangeVector = Vector3.zero;
		collidedPrevFrame = false;
		collisionFlagsPrevFrame = MVCollisionFlags.None;
	}

	private float[] GetImpactVals(MVCollisionFlags collisionFlags)
	{
		if ((collisionFlags & MVCollisionFlags.Above) > MVCollisionFlags.None)
		{
			return accThresMultipliers[MVCollisionFlags.Above];
		}
		if ((collisionFlags & MVCollisionFlags.Sides) > MVCollisionFlags.None)
		{
			return accThresMultipliers[MVCollisionFlags.Sides];
		}
		if ((collisionFlags & MVCollisionFlags.Below) > MVCollisionFlags.None)
		{
			return accThresMultipliers[MVCollisionFlags.Below];
		}
		Debug.LogError("Did not find proper collision flag");
		return null;
	}

	public void HandleMoveHit(MVControllerColliderHit moveHit)
	{
		if (!moveHit.testWithOutMoving)
		{
			moveHits.Add(moveHit);
		}
	}

	public float UpdateImpactState(Vector3 curVelocity, Vector3 prevVelocity, MVInteractableBase interactableLocal)
	{
		Vector3 vector = (curVelocity - prevVelocity) * Time.deltaTime;
		impactDamage = 0f;
		if (suspendImpactDamageCounter > 0)
		{
			suspendImpactDamageCounter--;
			moveHits.Clear();
			return 0f;
		}
		if (collidedPrevFrame)
		{
			Vector3 vector2 = prevVelocityChangeVector + vector;
			float[] impactVals = GetImpactVals(collisionFlagsPrevFrame);
			float num = impactVals[0];
			float num2 = vector2.magnitude * Time.deltaTime;
			num2 *= averageSoftnessPrevFrame;
			float num3 = num2 * impactVals[1];
			if (num3 > num)
			{
				if (num3 > 0f)
				{
					Debug.Log("Damage " + num3 + " frame " + Time.frameCount);
				}
				impactDamage = num3;
			}
		}
		if (moveHits.Count > 0)
		{
			collidedPrevFrame = true;
			averageSoftnessPrevFrame = 0f;
			foreach (MVControllerColliderHit moveHit in moveHits)
			{
				averageSoftnessPrevFrame += interactableLocal.HandleModifierEffect(AvatarModifierEffect.Softness, moveHit.material.physicalProperties.softness);
				collisionFlagsPrevFrame |= moveHit.collisionFlags;
			}
			averageSoftnessPrevFrame /= moveHits.Count;
		}
		else
		{
			collidedPrevFrame = false;
			collisionFlagsPrevFrame = MVCollisionFlags.None;
			averageSoftnessPrevFrame = -1f;
		}
		prevVelocityChangeVector = vector;
		moveHits.Clear();
		return impactDamage;
	}
}
