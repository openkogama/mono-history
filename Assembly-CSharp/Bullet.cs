using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private class CollisionBullet
	{
		public enum State
		{
			Moving,
			Hit,
			OutOfRange
		}

		private readonly float speed;

		private readonly float range;

		private float distanceTraveled;

		private Vector3 currentPos;

		private Vector3 prevPos;

		private Ray ray = default;

		private readonly HashSet<int> ignoreWoIDs;

		public CollisionBullet(float range, float speed, Vector3 origin, Vector3 direction, HashSet<int> ignoreWoIDs)
		{
			currentPos = origin;
			prevPos = origin;
			ray.direction = direction;
			this.range = range;
			this.speed = speed;
			this.ignoreWoIDs = ignoreWoIDs;
		}

		public State Update(out VoxelHit voxelHit)
		{
			State result = State.Moving;
			prevPos = currentPos;
			float num = speed * Time.deltaTime;
			distanceTraveled += num;
			if (distanceTraveled > range)
			{
				float num2 = distanceTraveled - range;
				num -= num2;
				result = State.OutOfRange;
			}
			currentPos = ray.direction * num + prevPos;
			if (DoCollisionCheck(out voxelHit))
			{
				result = State.Hit;
			}
			return result;
		}

		private bool DoCollisionCheck(out VoxelHit voxelHit)
		{
			ray.origin = prevPos;
			return DoBulletCollision(ray, out voxelHit, speed * Time.deltaTime, ignoreWoIDs);
		}

		private static bool DoBulletCollision(Ray ray, out VoxelHit voxelHit, float distance, HashSet<int> ignoreWoIDs)
		{
			LayerMask layerMask = -5;
			layerMask = (int)layerMask & ~(1 << LayerMask.NameToLayer("Logic"));
			if (CollisionDetection.MVHit(ray, out voxelHit, distance, ignoreWoIDs, layerMask))
			{
				Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.up, Color.green, 10f);
				Debug.DrawLine(voxelHit.point, voxelHit.point + Vector3.right, Color.green, 10f);
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
	}

	public delegate void OnHitDelegate(VoxelHit hit, Ray lineOfFire);

	public OnHitDelegate onHit;

	public OnHitDelegate onHitLocal;

	private PoolEnums initiatedPoolType;

	private MonoBehaviour pooledObjectReference;

	private HashSet<int> ignoreWoIDs = new HashSet<int>();

	private bool isFired;

	private Ray lineOfFire;

	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	private ParticleSystem pSystem;

	[SerializeField]
	private MeshRenderer[] meshRenderers;

	private CollisionBullet collisionBullet;

	private bool hit;

	private bool hasCleaned;

	private float currentAirTime;

	private float maxAirTime;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private Transform localTransform;

	private CullingSubscriberBase cullingSubscriberBase;

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

	private void Update()
	{
		CollisionBullet.State state = collisionBullet.Update(out var voxelHit);
		if (state == CollisionBullet.State.Hit)
		{
			hit = true;
			if (onHit != null)
			{
				onHit(voxelHit, lineOfFire);
				onHit = null;
			}
			if (onHitLocal != null)
			{
				onHitLocal(voxelHit, lineOfFire);
				onHitLocal = null;
			}
		}
		currentAirTime += Time.deltaTime;
		if (!hit && currentAirTime <= maxAirTime)
		{
			float t = currentAirTime / maxAirTime;
			localTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
		}
		else
		{
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.enabled = false;
			}
		}
		cullingSubscriberBase.Position = transform.position;
		if (state != CollisionBullet.State.Hit && state != CollisionBullet.State.OutOfRange && !hasCleaned)
		{
			return;
		}
		if (!hasCleaned)
		{
			MeshRenderer[] array2 = meshRenderers;
			foreach (MeshRenderer meshRenderer2 in array2)
			{
				meshRenderer2.enabled = false;
			}
			if (pSystem != null)
			{
				pSystem.Stop();
			}
			hasCleaned = true;
		}
		if (pSystem == null)
		{
			ReturnToPool(initiatedPoolType);
		}
		else if (!pSystem.IsAlive())
		{
			ReturnToPool(initiatedPoolType);
		}
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
		ResetBullet();
		cullingSubscriberBase.Destroy();
		cullingSubscriberBase = null;
		PrefabPool.Instance.EnumPoolManager.Return(pooledObjectReference, bulletType);
	}

	public void Fire(float speed, float range, Ray lineOfFire, HashSet<int> ignoreWoIDs)
	{
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			enabled = true;
			DoFire(speed, range);
			if (pooledObjectReference == null)
			{
				pooledObjectReference = this;
			}
			cullingSubscriberBase = new CullingSubscriberBase(1f, transform.position, OnStateChanged);
			cullingSubscriberBase.DistanceBandIndex = 5;
		}
	}

	private void DoFire(float speed, float maxRange)
	{
		collisionBullet = new CollisionBullet(maxRange, speed, lineOfFire.origin, lineOfFire.direction, ignoreWoIDs);
		localTransform = transform;
		startPosition = localTransform.position;
		targetPosition = FindTargetPos(maxRange);
		localTransform.localRotation = Quaternion.LookRotation((startPosition - targetPosition).normalized);
		float num = Vector3.Distance(targetPosition, lineOfFire.origin);
		maxAirTime = num / speed;
		if (trailRenderer != null)
		{
			trailRenderer.Clear();
		}
		if (pSystem != null)
		{
			pSystem.Play();
		}
	}

	private void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool flag = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = flag;
		}
		if (trailRenderer != null)
		{
			trailRenderer.enabled = flag;
		}
		if (pSystem != null)
		{
			ParticleSystem.EmissionModule emission = pSystem.emission;
			emission.enabled = flag;
		}
	}

	public Vector3 FindTargetPos(float maxRange)
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			LayerMask layerMask = -5;
			layerMask = (int)layerMask & ~(1 << LayerMask.NameToLayer("Logic"));
			if (CollisionDetection.MVHit(lineOfFire, out var voxelHit, maxRange, ignoreWoIDs, layerMask))
			{
				Debug.DrawLine(lineOfFire.origin, lineOfFire.GetPoint(voxelHit.distance), Color.yellow, 10f);
				return voxelHit.point;
			}
		}
		return gameObject.transform.position + lineOfFire.direction * maxRange;
	}
}
