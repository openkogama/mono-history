using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public delegate void OnHitDelegate(VoxelHit hit, Ray lineOfFire);

	public OnHitDelegate onHit;

	public OnHitDelegate onHitLocal;

	private HashSet<int> ignoreWoIDs = new HashSet<int>();

	private bool isFired;

	private Ray lineOfFire;

	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	private ParticleSystem pSystem;

	[SerializeField]
	private MeshRenderer[] meshRenderers;

	[SerializeField]
	private MeshFilter meshFilter;

	private bool hit;

	private bool hasCleaned;

	private float currentAirTime;

	private float maxAirTime;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private Transform localTransform;

	private PoolEnums initiatedPoolType;

	private MonoBehaviour pooledObjectReference;

	public PoolEnums InitiatedPoolType
	{
		get
		{
			return initiatedPoolType;
		}
		set
		{
			initiatedPoolType = value;
		}
	}

	public MonoBehaviour PooledObjectReference
	{
		get
		{
			return pooledObjectReference;
		}
		set
		{
			pooledObjectReference = value;
		}
	}

	private void Awake()
	{
		enabled = false;
	}

	public static Bullet CreateBullet(PoolEnums bulletType, Vector3 pos)
	{
		Bullet bullet = PrefabPool.Instance.EnumPoolManager.Instantiate<Bullet>(bulletType);
		bullet.onHit = null;
		bullet.onHitLocal = null;
		bullet.ignoreWoIDs.Clear();
		bullet.transform.localPosition = pos;
		bullet.isFired = false;
		bullet.hit = false;
		bullet.hasCleaned = false;
		bullet.currentAirTime = 0f;
		bullet.maxAirTime = 0f;
		bullet.initiatedPoolType = bulletType;
		return bullet;
	}

	public void ResetBullet()
	{
		onHit = null;
		onHitLocal = null;
		ignoreWoIDs.Clear();
		isFired = false;
		hit = false;
		hasCleaned = false;
		currentAirTime = 0f;
		maxAirTime = 0f;
	}

	public void ReturnToPool(PoolEnums bulletType)
	{
		PrefabPool.Instance.EnumPoolManager.Return(pooledObjectReference, bulletType);
	}

	public void Fire(float speed, float range, Ray lineOfFire, HashSet<int> ignoreWoIDs)
	{
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			DoFire(speed, range);
			enabled = true;
			if (pooledObjectReference == null)
			{
				pooledObjectReference = this;
			}
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = true;
			}
		}
	}

	private void Update()
	{
		currentAirTime += Time.deltaTime;
		if (!hit && currentAirTime <= maxAirTime)
		{
			float num = currentAirTime / maxAirTime;
			localTransform.position = Vector3.Lerp(startPosition, targetPosition, num);
			Vector3 pos = Vector3.Lerp(lineOfFire.origin, targetPosition, num);
			if (num >= 0f && DoCollisionCheck(pos, out var voxelHit))
			{
				hit = true;
				if (onHit != null)
				{
					onHit(voxelHit, lineOfFire);
				}
				if (onHitLocal != null)
				{
					onHitLocal(voxelHit, lineOfFire);
				}
			}
			return;
		}
		if (!hasCleaned)
		{
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = false;
			}
			if ((bool)pSystem)
			{
				pSystem.Stop();
			}
			hasCleaned = true;
		}
		if ((bool)pSystem)
		{
			if (!pSystem.IsAlive())
			{
				ReturnToPool(initiatedPoolType);
			}
		}
		else
		{
			ReturnToPool(initiatedPoolType);
		}
	}

	private void DoFire(float speed, float maxRange)
	{
		localTransform = GetComponent<Transform>();
		startPosition = localTransform.position;
		targetPosition = FindTargetPos(maxRange);
		localTransform.localRotation = Quaternion.LookRotation((startPosition - targetPosition).normalized);
		float num = Vector3.Distance(targetPosition, lineOfFire.origin);
		maxAirTime = num / speed;
		if ((bool)trailRenderer)
		{
			trailRenderer.Clear();
		}
		if ((bool)pSystem)
		{
			pSystem.Play();
		}
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
