using System.Collections.Generic;
using UnityEngine;

public class ImpactState
{
	public Vector3 prevVelocityChangeVector = Vector3.zero;

	public bool collidedPrevFrame;

	public float averageSoftnessPrevFrame = 1f;

	private float maxAccBeforeDamageDealt = 35f;

	private float impactDamageMultiplier = 200f;

	private float impactDamage;

	private bool suspendImpactDamage;

	public float ImpactDamage => impactDamage;

	public ImpactState()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SetValuesToTweakSheet();
	}

	private void SetValuesToTweakSheet()
	{
		maxAccBeforeDamageDealt = AvatarTweakSheet.ImpactState.maxAccBeforeDamageDealt;
		impactDamageMultiplier = AvatarTweakSheet.ImpactState.impactDamageMultiplier;
	}

	public void SuspendImpactDamage()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		suspendImpactDamage = true;
		prevVelocityChangeVector = Vector3.zero;
		collidedPrevFrame = false;
	}

	public float CalcImpactDamage(float velocityChange)
	{
		return velocityChange * impactDamageMultiplier;
	}

	public void UpdateImpactState(Vector3 curVelocityChangeVector, List<MVControllerColliderHit> moveHits)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		impactDamage = 0f;
		if (suspendImpactDamage)
		{
			return;
		}
		if (collidedPrevFrame)
		{
			Vector3 val = prevVelocityChangeVector + curVelocityChangeVector;
			float num = maxAccBeforeDamageDealt * Time.deltaTime;
			float magnitude = val.magnitude;
			magnitude *= averageSoftnessPrevFrame;
			if (magnitude > num)
			{
				impactDamage = CalcImpactDamage(magnitude);
			}
			prevVelocityChangeVector = Vector3.zero;
		}
		else
		{
			prevVelocityChangeVector = curVelocityChangeVector;
		}
		if (moveHits.Count > 0)
		{
			collidedPrevFrame = true;
			averageSoftnessPrevFrame = 0f;
			foreach (MVControllerColliderHit moveHit in moveHits)
			{
				averageSoftnessPrevFrame += moveHit.material.physicalProperties.softness;
			}
			averageSoftnessPrevFrame /= moveHits.Count;
		}
		else
		{
			collidedPrevFrame = false;
			averageSoftnessPrevFrame = -1f;
		}
	}
}
