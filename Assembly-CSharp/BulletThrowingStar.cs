using System.Collections.Generic;
using UnityEngine;

public class BulletThrowingStar : MonoBehaviour
{
	public delegate void OnHitDelegate(VoxelHit hit, Ray lineOfFire);

	private CullingSubscriberBase cullingSubscriberBase;

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
	private AudioSource aSource;

	[SerializeField]
	private MeshRenderer[] meshRenderers;

	[SerializeField]
	private MeshFilter meshFilter;

	private float fallRate;

	private float rotationSpeedXMin = 20f;

	private float rotationSpeedXMax = 30f;

	private float rotationSpeedZMin = 2f;

	private float rotationSpeedZMax = 8f;

	private bool inAir;

	private bool isFalling;

	private bool hasHit;

	private bool hasHitStatic;

	private float speed;

	private float rangeStraight;

	private float rangeFall;

	private float downwardForce;

	private bool hasNotified;

	private float totalDistTravelled;

	private VoxelHit voxelHit = default;

	private Vector3 airRotation = default;

	private Vector3 direction;

	private Transform tfrm;

	private float coolOffStartTime;

	private float coolOffDuration = 3.5f;

	private PoolEnums initiatedPoolEnum;

	private void Awake()
	{
		tfrm = transform;
		enabled = false;
	}

	public static BulletThrowingStar CreateBullet(PoolEnums poolEnum, Vector3 pos)
	{
		BulletThrowingStar bulletThrowingStar = PrefabPool.Instance.EnumPoolManager.Instantiate<BulletThrowingStar>(poolEnum);
		bulletThrowingStar.transform.localPosition = pos;
		bulletThrowingStar.ignoreWoIDs.Clear();
		bulletThrowingStar.isFired = false;
		bulletThrowingStar.fallRate = 0f;
		bulletThrowingStar.rotationSpeedXMin = 20f;
		bulletThrowingStar.rotationSpeedXMax = 30f;
		bulletThrowingStar.rotationSpeedZMin = 2f;
		bulletThrowingStar.rotationSpeedZMax = 8f;
		bulletThrowingStar.onHitLocal = null;
		bulletThrowingStar.onHit = null;
		bulletThrowingStar.initiatedPoolEnum = poolEnum;
		return bulletThrowingStar;
	}

	public void Fire(float speed, float rangeStraight, Ray lineOfFire, HashSet<int> ignoreWoIDs, float rangeFall, float fallRate)
	{
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.fallRate = fallRate;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			this.rangeFall = rangeFall;
			this.fallRate = fallRate;
			this.rangeStraight = rangeStraight;
			this.speed = speed;
			inAir = true;
			isFalling = false;
			hasHit = false;
			hasHitStatic = false;
			hasNotified = false;
			downwardForce = 0f;
			totalDistTravelled = 0f;
			coolOffStartTime = 0f;
			Vector3 localPosition = tfrm.localPosition;
			Vector3 vector = FindTargetPos(rangeStraight);
			tfrm.localRotation = Quaternion.LookRotation((vector - localPosition).normalized);
			tfrm.localPosition = localPosition;
			if ((bool)pSystem)
			{
				pSystem.Play();
			}
			if ((bool)aSource)
			{
				aSource.loop = true;
				aSource.Play();
			}
			direction = tfrm.forward;
			airRotation = new Vector3(Random.Range(rotationSpeedXMin, rotationSpeedXMax), 0f, Random.Range(rotationSpeedZMin, rotationSpeedZMax));
			enabled = true;
			cullingSubscriberBase = new CullingSubscriberBase(1f, transform.position, OnStateChanged);
			cullingSubscriberBase.DistanceBandIndex = 5;
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

	private void Update()
	{
		if (inAir)
		{
			tfrm.Rotate(airRotation * Time.deltaTime * speed);
			if (totalDistTravelled > rangeStraight && !isFalling)
			{
				isFalling = true;
			}
			Vector3 vector = direction * speed * Time.deltaTime;
			if (isFalling)
			{
				downwardForce += (0f - fallRate) * speed * speed * Time.deltaTime * Time.deltaTime;
				vector.y += downwardForce;
			}
			Vector3 vector2 = tfrm.localPosition + vector;
			Vector3 vector3 = vector2 - tfrm.localPosition;
			Vector3 normalized = vector3.normalized;
			if (DoCollisionCheck(tfrm.localPosition, vector.magnitude, normalized, out voxelHit))
			{
				vector2 = voxelHit.point;
				inAir = false;
				hasHit = true;
			}
			Debug.DrawLine(tfrm.localPosition, vector2 + normalized * 10f, Color.red);
			totalDistTravelled += vector3.magnitude;
			tfrm.localPosition = vector2;
			if (totalDistTravelled > rangeStraight + rangeFall)
			{
				inAir = false;
			}
			cullingSubscriberBase.Position = transform.position;
			return;
		}
		bool flag = true;
		if ((bool)aSource && aSource.isPlaying)
		{
			aSource.Stop();
		}
		if ((bool)pSystem)
		{
			pSystem.Stop();
			if (pSystem.IsAlive())
			{
				flag = false;
			}
		}
		if (hasHit && !hasNotified)
		{
			hasNotified = true;
			if (onHit != null)
			{
				onHit(voxelHit, lineOfFire);
			}
			if (onHitLocal != null)
			{
				onHitLocal(voxelHit, lineOfFire);
			}
			if (!hasHitStatic)
			{
				hasHitStatic = true;
				coolOffStartTime = Time.time;
				int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
				if (worldObjectClient != null)
				{
					hasHitStatic = worldObjectClient.InteractionDataHandlerBase == null;
				}
			}
		}
		if (coolOffStartTime + coolOffDuration > Time.time && hasHitStatic)
		{
			flag = false;
			float num = (Time.time - coolOffStartTime) / coolOffDuration;
			MeshRenderer[] array = meshRenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				Material[] materials = meshRenderer.materials;
				foreach (Material material in materials)
				{
					material.color = new Color(material.color.r, material.color.g, material.color.b, 1f - num);
				}
			}
		}
		if (flag)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
			PrefabPool.Instance.EnumPoolManager.Return(this, initiatedPoolEnum);
		}
	}

	private bool DoCollisionCheck(Vector3 pos, float dist, Vector3 dir, out VoxelHit voxelHit)
	{
		Ray ray = new Ray(pos, dir);
		return DoBulletCollision(ray, out voxelHit, dist, ignoreWoIDs);
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
		LayerMask layerMask = -5;
		layerMask = (int)layerMask & ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
		if (CollisionDetection.MVHit(lineOfFire, out var voxelHit, maxRange, ignoreWoIDs, layerMask))
		{
			Debug.DrawLine(lineOfFire.origin, lineOfFire.GetPoint(voxelHit.distance), Color.yellow, 10f);
			return voxelHit.point;
		}
		return gameObject.transform.position + lineOfFire.direction * maxRange;
	}
}
