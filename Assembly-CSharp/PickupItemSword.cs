using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PickupItemSword : PickupItemWithDelay
{
	public Animation swordAnim;

	public GameObject bloodParticlesPrefab;

	public float pushMagnitude = 500f;

	public float pushRadius = 3f;

	public float hitDamage = 25f;

	public float velocityDamageFactor = 0.2f;

	public float velocityPushFactor = 10f;

	public AudioSource hitAudioSource;

	private bool checkingOverlaps;

	public override AvatarItemType Type => AvatarItemType.Sword;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	protected override void OnFire(bool isLocal)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		swordAnim.Play();
		isFiring = false;
		MVGameController.Instance.AudioManager.Play("sword swing", ((Component)this).audio, muzzlePoint.position);
		if (isLocal)
		{
		}
	}

	private IEnumerator DoOverlapCheck()
	{
		checkingOverlaps = true;
		HashSet<MVWorldObjectClient> hitWos = new HashSet<MVWorldObjectClient>();
		while (checkingOverlaps)
		{
			Collider[] hits = Physics.OverlapSphere(muzzlePoint.position, pushRadius);
			Collider[] array = hits;
			foreach (Collider h in array)
			{
				MVWorldObjectClient wo = MVWorldObjectClientManager.GetMVObject(((Component)h).transform);
				if (wo != null && !owner.IgnoreWOIDs.Contains(wo.Id))
				{
					hitWos.Add(wo);
				}
			}
			yield return 0;
		}
		Vector3 dir = owner.LookDirection;
		dir.y = 0.02f;
		dir.Normalize();
		bool hitOpponent = false;
		foreach (MVWorldObjectClient wo2 in hitWos)
		{
			if (wo2.Id != owner.WorldObjectOwner.Id)
			{
				InteractionDataHandlerBase interactionHandler = wo2.GameObject.GetComponent<InteractionDataHandlerBase>();
				if ((Object)(object)interactionHandler != (Object)null)
				{
					hitOpponent = true;
					Vector3 impulse = dir * pushMagnitude;
					float dmg = hitDamage;
					interactionHandler.HandleInteraction(SwordHitPackage.Create(impulse, dmg), interactionIsLocal: false);
					Object.Instantiate((Object)(object)bloodParticlesPrefab, wo2.GetTargetPosition(), Quaternion.identity);
				}
			}
		}
		if (hitOpponent)
		{
			MVRigidBody rigidBody = ((Component)owner).GetComponent<MVRigidBody>();
			if ((Object)(object)rigidBody != (Object)null)
			{
				rigidBody.AddImpulse(-dir * 700f);
			}
			hitAudioSource.Play();
		}
	}

	public void StartOverlapCheck()
	{
		if (owner.IsLocal && !checkingOverlaps)
		{
			((MonoBehaviour)this).StartCoroutine(DoOverlapCheck());
		}
	}

	public void StopOverlapCheck()
	{
		if (owner.IsLocal)
		{
			checkingOverlaps = false;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.DrawWireSphere(muzzlePoint.position, pushRadius);
	}
}
