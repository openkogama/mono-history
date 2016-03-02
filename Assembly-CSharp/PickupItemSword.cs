using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemSword : PickupItemWithDelay
{
	[SerializeField]
	private AudioSource audioSource;

	public Animation swordAnim;

	public GameObject bloodParticlesPrefab;

	public float pushMagnitude = 500f;

	public float pushRadius = 3f;

	public float hitDamage = 25f;

	public float velocityDamageFactor = 0.2f;

	public float velocityPushFactor = 10f;

	public AudioSource hitAudioSource;

	public Transform MuzzlePointHitTerrain;

	private bool checkingOverlaps;

	public override AvatarItemType Type => AvatarItemType.Sword;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	public override void UpdateWithDirection(Vector3 dir)
	{
	}

	protected override void OnFire(bool isLocal)
	{
		swordAnim.Play();
		isFiring = false;
		MVGameControllerBase.AudioManager.Play("sword swing", audioSource, muzzlePoint.position);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("sword swing", audioSource, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("sword swing", audioSource, muzzlePoint.position);
		}
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
				MVWorldObjectClient wo = MVWorldObjectClientManager.GetMVObject(h.transform);
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
				InteractionDataHandlerBase interactionHandler = wo2.InteractionDataHandlerBase;
				if (interactionHandler != null)
				{
					hitOpponent = true;
					Vector3 impulse = dir * pushMagnitude;
					float dmg = hitDamage;
					interactionHandler.HandleInteraction(SwordHitPackage.Create(impulse, dmg), interactionIsLocal: false);
					Object.Instantiate(bloodParticlesPrefab, wo2.GetTargetPosition(), Quaternion.identity);
				}
			}
		}
		DoRemoveCubes();
		if (hitOpponent)
		{
			MVRigidBody rigidBody = owner.GetComponent<MVRigidBody>();
			if (rigidBody != null)
			{
				rigidBody.AddImpulse(-dir * 700f);
			}
			hitAudioSource.Play();
		}
	}

	private void DoRemoveCubes()
	{
		Ray ray = new Ray(MuzzlePointHitTerrain.position - owner.LookDirection, owner.LookDirection);
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * 3f, Color.red, 10f);
		if (CollisionDetection.MVHit(ray, out var voxelHit, 3f, new HashSet<int>(), 1 << LayerMask.NameToLayer("Default")))
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (worldObjectClient.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain || worldObjectClient.WorldObjectType == WorldObjectType.CubeModelTerrainFineGrained)
			{
				MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, 20f);
			}
		}
	}

	public void StartOverlapCheck()
	{
		if (owner.IsLocal && !checkingOverlaps)
		{
			StartCoroutine(DoOverlapCheck());
		}
	}

	public void StopOverlapCheck()
	{
		if (owner.IsLocal)
		{
			checkingOverlaps = false;
		}
	}
}
