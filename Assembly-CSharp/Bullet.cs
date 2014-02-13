using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public delegate void OnHitDelegate(VoxelHit hit, Ray lineOfFire);

	public OnHitDelegate onHit;

	public OnHitDelegate onHitLocal;

	private HashSet<int> ignoreWoIDs;

	private bool isFired;

	private Ray lineOfFire;

	private void Awake()
	{
		((Behaviour)this).enabled = false;
	}

	public static Bullet CreateBullet(Bullet prefab, Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Object.Instantiate((Object)(object)prefab, pos, Quaternion.identity) as Bullet;
	}

	public void Fire(float speed, float range, Ray lineOfFire, HashSet<int> ignoreWoIDs)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			((MonoBehaviour)this).StartCoroutine(DoFire(speed, range));
		}
	}

	private IEnumerator DoFire(float speed, float maxRange)
	{
		bool inAir = true;
		bool hasHit = false;
		float startTime = Time.time;
		Vector3 startPos = ((Component)this).gameObject.transform.position;
		Vector3 targetPos = FindTargetPos(maxRange);
		Transform transform = ((Component)this).transform;
		Vector3 val = startPos - targetPos;
		transform.rotation = Quaternion.LookRotation(val.normalized);
		float range = Vector3.Distance(targetPos, lineOfFire.origin);
		float airTime = range / speed;
		((Component)this).transform.position = startPos;
		if (Object.op_Implicit((Object)(object)((Component)this).particleSystem))
		{
			((Component)this).particleSystem.Play();
		}
		Debug.DrawLine(lineOfFire.origin, targetPos, Color.red);
		VoxelHit voxelHit = default;
		while (inAir)
		{
			float interpTime = (Time.time - startTime) / airTime;
			if (interpTime >= 1f)
			{
				inAir = false;
			}
			((Component)this).transform.position = Vector3.Lerp(startPos, targetPos, interpTime);
			Vector3 collidePos = Vector3.Lerp(lineOfFire.origin, targetPos, interpTime);
			if (interpTime > 0f && DoCollisionCheck(collidePos, out voxelHit))
			{
				inAir = false;
				hasHit = true;
			}
			yield return 0;
		}
		if (!hasHit)
		{
			voxelHit.point = targetPos;
		}
		if (hasHit)
		{
			if (onHit != null)
			{
				onHit(voxelHit, lineOfFire);
			}
			if (onHitLocal != null)
			{
				onHitLocal(voxelHit, lineOfFire);
			}
		}
		MeshRenderer[] renderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer r in array)
		{
			((Renderer)r).enabled = false;
		}
		if (Object.op_Implicit((Object)(object)((Component)this).particleSystem))
		{
			((Component)this).particleSystem.Stop();
			while (((Component)this).particleSystem.IsAlive())
			{
				yield return 0;
			}
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private bool DoCollisionCheck(Vector3 pos, out VoxelHit voxelHit)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = new Ray(pos, lineOfFire.direction);
		return DoBulletCollision(ray, out voxelHit, 2f, ignoreWoIDs);
	}

	private static bool DoBulletCollision(Ray ray, out VoxelHit voxelHit, float distance, HashSet<int> ignoreWoIDs)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		LayerMask val = LayerMask.op_Implicit(-5);
		val = LayerMask.op_Implicit(LayerMask.op_Implicit(val) & ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F)));
		if (CollisionDetection.MVHit(ray, out voxelHit, distance, ignoreWoIDs, LayerMask.op_Implicit(val)))
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (worldObjectClient.PlayInteractionType == PlayInteractionType.Solid)
			{
				return true;
			}
			MVWorldObjectClient hitInteractionHandlingWO = worldObjectClient.GetHitInteractionHandlingWO();
			if (hitInteractionHandlingWO == null)
			{
				return false;
			}
			voxelHit.woId = hitInteractionHandlingWO.Id;
			return true;
		}
		return false;
	}

	public Vector3 FindTargetPos(float maxRange)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		LayerMask val = LayerMask.op_Implicit(-5);
		val = LayerMask.op_Implicit(LayerMask.op_Implicit(val) & ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F)));
		if (CollisionDetection.MVHit(lineOfFire, out var voxelHit, maxRange, ignoreWoIDs, LayerMask.op_Implicit(val)))
		{
			Debug.DrawLine(lineOfFire.origin, lineOfFire.GetPoint(voxelHit.distance), Color.yellow, 10f);
			return voxelHit.point;
		}
		return ((Component)this).gameObject.transform.position + lineOfFire.direction * maxRange;
	}
}
