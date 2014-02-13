using System.Collections.Generic;
using UnityEngine;

public class ImpactState
{
	private const int suspendImpactDamageFrames = 1;

	public Vector3 prevVelocityChangeVector = Vector3.zero;

	private bool collidedPrevFrame;

	private float averageSoftnessPrevFrame = 1f;

	private float maxAccBeforeDamageDealt = 35f;

	private float impactDamageMultiplier = 200f;

	private float impactDamage;

	private int suspendImpactDamageCounter = 1;

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
		suspendImpactDamageCounter = 1;
		prevVelocityChangeVector = Vector3.zero;
		collidedPrevFrame = false;
	}

	private float CalcImpactDamage(float velocityChange)
	{
		return velocityChange * impactDamageMultiplier;
	}

	public float UpdateImpactState(Vector3 curVelocity, Vector3 prevVelocity, List<MVControllerColliderHit> moveHits, MVInteractableBase interactableLocal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
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
			float num = maxAccBeforeDamageDealt * Time.deltaTime;
			float magnitude = val2.magnitude;
			magnitude *= averageSoftnessPrevFrame;
			if (magnitude > num)
			{
				impactDamage = CalcImpactDamage(magnitude);
			}
			prevVelocityChangeVector = Vector3.zero;
		}
		else
		{
			prevVelocityChangeVector = val;
		}
		if (moveHits.Count > 0)
		{
			collidedPrevFrame = true;
			averageSoftnessPrevFrame = 0f;
			foreach (MVControllerColliderHit moveHit in moveHits)
			{
				averageSoftnessPrevFrame += interactableLocal.HandleModifierEffect(AvatarModifierEffect.Softness, moveHit.material.physicalProperties.softness);
			}
			averageSoftnessPrevFrame /= moveHits.Count;
		}
		else
		{
			collidedPrevFrame = false;
			averageSoftnessPrevFrame = -1f;
		}
		return impactDamage;
	}
}
