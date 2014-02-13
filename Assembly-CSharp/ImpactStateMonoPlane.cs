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

	public float ImpactDamage => impactDamage;

	public ImpactStateMonoPlane()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
	}

	public void SuspendImpactDamage()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
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
		Debug.LogError((object)"Did not find proper collision flag");
		return null;
	}

	public float UpdateImpactState(Vector3 curVelocity, Vector3 prevVelocity, List<MVControllerColliderHit> moveHits, MVInteractableBase interactableLocal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (curVelocity - prevVelocity) * Time.deltaTime;
		impactDamage = 0f;
		if (suspendImpactDamageCounter > 0)
		{
			suspendImpactDamageCounter--;
			return 0f;
		}
		if (collidedPrevFrame)
		{
			Vector3 val2 = prevVelocityChangeVector + val;
			float[] impactVals = GetImpactVals(collisionFlagsPrevFrame);
			float num = impactVals[0];
			float num2 = val2.magnitude * Time.deltaTime;
			num2 *= averageSoftnessPrevFrame;
			float num3 = num2 * impactVals[1];
			if (num3 > num)
			{
				if (num3 > 0f)
				{
					Debug.Log((object)("Damage " + num3 + " frame " + Time.frameCount));
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
		prevVelocityChangeVector = val;
		return impactDamage;
	}
}
