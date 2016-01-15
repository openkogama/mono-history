using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletThrowingStar : MonoBehaviour
{
	public delegate void OnHitDelegate(VoxelHit hit, Ray lineOfFire);

	public OnHitDelegate onHit;

	public OnHitDelegate onHitLocal;

	private HashSet<int> ignoreWoIDs;

	private bool isFired;

	private Ray lineOfFire;

	[SerializeField]
	private ParticleSystem pSystem;

	[SerializeField]
	private AudioSource aSource;

	[SerializeField]
	private MeshRenderer[] meshRenderers;

	private float fallRate;

	private float rotationSpeedXMin = 20f;

	private float rotationSpeedXMax = 30f;

	private float rotationSpeedZMin = 2f;

	private float rotationSpeedZMax = 8f;

	private void Awake()
	{
		enabled = false;
	}

	private void Start()
	{
	}

	public static BulletThrowingStar CreateBullet(BulletThrowingStar prefab, Vector3 pos)
	{
		return Object.Instantiate(prefab, pos, Quaternion.identity) as BulletThrowingStar;
	}

	public void Fire(float speed, float rangeStraight, Ray lineOfFire, HashSet<int> ignoreWoIDs, float rangeFall, float fallRate)
	{
		if (!isFired)
		{
			this.lineOfFire = lineOfFire;
			this.fallRate = fallRate;
			this.ignoreWoIDs = ignoreWoIDs;
			isFired = true;
			StartCoroutine(DoFire(speed, rangeStraight, rangeFall));
		}
	}

	private IEnumerator DoFire(float speed, float rangeStraight, float rangeFall)
	{
		bool inAir = true;
		bool isFalling = false;
		bool hasHit = false;
		Vector3 startPos = gameObject.transform.position;
		Vector3 targetPosStraight = FindTargetPos(rangeStraight);
		transform.rotation = Quaternion.LookRotation((targetPosStraight - startPos).normalized);
		Vector3 advanceDir = transform.forward;
		transform.position = startPos;
		if ((bool)pSystem)
		{
			pSystem.Play();
		}
		VoxelHit voxelHit = default;
		float totalDistTravelled = 0f;
		Vector3 downForce = Vector3.zero;
		float rotX = Random.Range(rotationSpeedXMin, rotationSpeedXMax);
		float rotZ = Random.Range(rotationSpeedZMin, rotationSpeedZMax);
		if (aSource != null)
		{
			aSource.loop = true;
			aSource.Play();
		}
		while (inAir)
		{
			transform.Rotate(new Vector3(rotX * Time.deltaTime * speed, 0f, rotZ * Time.deltaTime * speed));
			if (totalDistTravelled > rangeStraight && !isFalling)
			{
				isFalling = true;
			}
			Vector3 advanceStep = advanceDir * speed * Time.deltaTime;
			if (isFalling)
			{
				downForce += new Vector3(0f, (0f - fallRate) * speed * speed * Time.deltaTime * Time.deltaTime, 0f);
				advanceStep += downForce;
			}
			Vector3 targetPos = transform.position + advanceStep;
			if (DoCollisionCheck(transform.position, advanceStep.magnitude, (targetPos - transform.position).normalized, out voxelHit))
			{
				targetPos = voxelHit.point;
				inAir = false;
				hasHit = true;
			}
			Debug.DrawLine(transform.position, targetPos + (targetPos - transform.position).normalized * 10f, Color.red);
			totalDistTravelled += (targetPos - transform.position).magnitude;
			transform.position = targetPos;
			if (totalDistTravelled > rangeStraight + rangeFall)
			{
				inAir = false;
			}
			yield return 0;
		}
		if (aSource != null && aSource.isPlaying)
		{
			aSource.Stop();
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
		float coolOffStartTime = Time.time;
		float coolOffDuration = 3.5f;
		if ((bool)pSystem)
		{
			pSystem.Stop();
			while (pSystem.IsAlive())
			{
				yield return 0;
			}
		}
		if (hasHit)
		{
			bool hasHitStaticStructure = true;
			int woID = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient wo = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
			if (wo != null)
			{
				InteractionDataHandlerBase interactionHandler = wo.InteractionDataHandlerBase;
				hasHitStaticStructure = interactionHandler == null;
			}
			while (coolOffStartTime + coolOffDuration > Time.time && hasHitStaticStructure)
			{
				float t = (Time.time - coolOffStartTime) / coolOffDuration;
				MeshRenderer[] array = meshRenderers;
				foreach (MeshRenderer r in array)
				{
					Material[] materials = r.materials;
					foreach (Material m in materials)
					{
						m.color = new Color(m.color.r, m.color.g, m.color.b, 1f - t);
					}
				}
				yield return 0;
			}
		}
		MeshRenderer[] array2 = meshRenderers;
		foreach (MeshRenderer r2 in array2)
		{
			r2.enabled = false;
		}
		Object.Destroy(gameObject);
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
