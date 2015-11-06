using System.Collections;
using System.Collections.Generic;
using MV.Common;
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
		enabled = false;
	}

	public static Bullet CreateBullet(Bullet prefab, Vector3 pos)
	{
		return Object.Instantiate(prefab, pos, Quaternion.identity) as Bullet;
	}

	public void Fire(float speed, float range, Ray lineOfFire, HashSet<int> ignoreWoIDs)
	{
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			StartCoroutine(DoFire(speed, range));
		}
	}

	private IEnumerator DoFire(float speed, float maxRange)
	{
		bool inAir = true;
		bool hasHit = false;
		float startTime = Time.time;
		Vector3 startPos = gameObject.transform.position;
		Vector3 targetPos = FindTargetPos(maxRange);
		transform.rotation = Quaternion.LookRotation((startPos - targetPos).normalized);
		float range = Vector3.Distance(targetPos, lineOfFire.origin);
		float airTime = range / speed;
		transform.position = startPos;
		if ((bool)GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Play();
		}
		VoxelHit voxelHit = default;
		while (inAir)
		{
			float interpTime = (Time.time - startTime) / airTime;
			if (interpTime >= 1f)
			{
				inAir = false;
			}
			transform.position = Vector3.Lerp(startPos, targetPos, interpTime);
			Vector3 collidePos = Vector3.Lerp(lineOfFire.origin, targetPos, interpTime);
			if (interpTime >= 0f)
			{
				if (DoCollisionCheck(collidePos, out voxelHit))
				{
					inAir = false;
					hasHit = true;
				}
			}
			else
			{
				Debug.Log("Skipped 1");
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
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer r in array)
		{
			r.enabled = false;
		}
		if ((bool)GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Stop();
			while (GetComponent<ParticleSystem>().IsAlive())
			{
				yield return 0;
			}
		}
		Object.Destroy(gameObject);
	}

	private bool DoCollisionCheck(Vector3 pos, out VoxelHit voxelHit)
	{
		Ray ray = new Ray(pos, lineOfFire.direction);
		return DoBulletCollision(ray, out voxelHit, 2f, ignoreWoIDs);
	}

	private static bool DoBulletCollision(Ray ray, out VoxelHit voxelHit, float distance, HashSet<int> ignoreWoIDs)
	{
		LayerMask layerMask = -5;
		layerMask = (int)layerMask & ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
		if (CollisionDetection.MVHit(ray, out voxelHit, distance, ignoreWoIDs, layerMask))
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
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
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			LayerMask layerMask = -5;
			layerMask = (int)layerMask & ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
			if (CollisionDetection.MVHit(lineOfFire, out var voxelHit, maxRange, ignoreWoIDs, layerMask))
			{
				Debug.DrawLine(lineOfFire.origin, lineOfFire.GetPoint(voxelHit.distance), Color.yellow, 10f);
				return voxelHit.point;
			}
		}
		return gameObject.transform.position + lineOfFire.direction * maxRange;
	}
}
