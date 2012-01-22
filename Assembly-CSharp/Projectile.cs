using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private bool isFired;

	private int instigatorActorNr;

	private string colFXPrefabName = string.Empty;

	private string avatarHitFXPrefabName = string.Empty;

	private Ray lineOfFire;

	private bool playFXAtTargetEnd;

	public bool avatarWasHit;

	private int ownerWorldId;

	public int InstigatorActorNr => instigatorActorNr;

	private void Awake()
	{
		((Behaviour)this).enabled = false;
	}

	public static Projectile CreateProjectile(string prefabName, string collisionFXPrefabName, string avatarHitFXPrefabName, Vector3 pos, Vector3 dir)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load(prefabName));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.position = pos;
		val2.transform.rotation = Quaternion.LookRotation(dir);
		Projectile component = val2.GetComponent<Projectile>();
		component.colFXPrefabName = collisionFXPrefabName;
		component.avatarHitFXPrefabName = avatarHitFXPrefabName;
		return component;
	}

	public void Fire(float speed, float range, int instigatorActorNr, Ray lineOfFire, int ownerId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			ownerWorldId = ownerId;
			isFired = true;
			this.instigatorActorNr = instigatorActorNr;
			((MonoBehaviour)this).StartCoroutine(DoFire(speed, range));
		}
	}

	private IEnumerator DoFire(float speed, float maxRange)
	{
		bool inAir = true;
		bool hasHit = false;
		float startTime = Time.time;
		Vector3 startPos = ((Component)this).gameObject.transform.position;
		Vector3 targetPos = startPos + FindTargetDir(startPos, maxRange) * maxRange;
		float range = Vector3.Distance(targetPos, startPos);
		float airTime = range / speed;
		while (inAir)
		{
			float interpTime = (Time.time - startTime) / airTime;
			if (interpTime >= 1f)
			{
				inAir = false;
			}
			Vector3 newPos = Vector3.Lerp(startPos, targetPos, interpTime);
			if (interpTime > 0f)
			{
				Vector3 hitPoint = Vector3.zero;
				if (DoCollisionCheck(((Component)this).gameObject.transform.position, newPos, ref hitPoint))
				{
					inAir = false;
					hasHit = true;
					PlayCollisionFX(hitPoint);
				}
			}
			if (!inAir)
			{
				break;
			}
			((Component)this).gameObject.transform.position = newPos;
			yield return 0;
		}
		if (!hasHit && playFXAtTargetEnd)
		{
			PlayCollisionFX(targetPos);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private bool DoCollisionCheck(Vector3 pos, Vector3 target, ref Vector3 hitPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(pos, target);
		if (num <= 0f)
		{
			return false;
		}
		Ray ray = new Ray(pos, ((Component)this).gameObject.transform.forward);
		if (CollisionDetection.MVHit(ray, out var voxelHit, num, new HashSet<int> { ownerWorldId }))
		{
			playFXAtTargetEnd = true;
			hitPoint = voxelHit.point;
			MVWorldObjectClient mVWorldObjectClient = MVGameController.Instance.WOCM.WorldObjects[voxelHit.woId];
			if (mVWorldObjectClient is MVAvatar)
			{
				avatarWasHit = true;
				if (instigatorActorNr != voxelHit.woId)
				{
					if (mVWorldObjectClient.OwnerActorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
					{
						(mVWorldObjectClient as MVAvatar).Health.Value -= 20f;
						AvatarController avatarController = (mVWorldObjectClient as MVAvatar).AvatarController;
						Vector3 direction = lineOfFire.direction;
						avatarController.ApplyImpulse(-direction.normalized * -700f, suspendImpactDamage: true);
						MVGameController.Instance.WOCM.WeCamera.CurCamera.Shake(0.4f, 1f);
					}
					return true;
				}
				return false;
			}
			if (mVWorldObjectClient is MVLogicObject)
			{
				return false;
			}
			Debug.Log((object)mVWorldObjectClient.GetType().Name);
			return true;
		}
		return false;
	}

	private Vector3 FindTargetDir(Vector3 startPos, float maxRange)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).gameObject.transform.position + lineOfFire.direction * maxRange;
		LayerMask val2 = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		val2 = LayerMask.op_Implicit(LayerMask.op_Implicit(val2) + (1 << (LayerMask.NameToLayer("Player") & 0x1F)));
		if (CollisionDetection.MVHit(lineOfFire, out var voxelHit, maxRange, new HashSet<int> { ownerWorldId }, LayerMask.op_Implicit(val2)))
		{
			val = voxelHit.point;
		}
		Vector3 val3 = val - startPos;
		return val3.normalized;
	}

	private void PlayCollisionFX(Vector3 pos)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!avatarWasHit)
		{
			Object.Instantiate(Resources.Load(colFXPrefabName), pos, Quaternion.identity);
		}
		else
		{
			Object.Instantiate(Resources.Load(avatarHitFXPrefabName), pos, Quaternion.identity);
		}
	}
}
