using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class PickupItemModelGun : PickupItemWithDelay
{
	public float minDistanceToCubeFire = 0.8f;

	public int ammo = 10;

	public RailRay railGunRayPrefab;

	public float speed = 30f;

	public float range = 200f;

	public Transform chargeObject;

	public float fireIntervalSecondary = 1.3f;

	public Bullet rocketPrefab;

	public GUICellCursor primaryCursor;

	public GUICellCursor secondaryCursor;

	public AudioClip cubeLandedSound;

	public AudioClip chargeSound;

	public AudioClip releaseSound;

	public AudioClip cubeDestroyedSound;

	public AudioClip firePrimary;

	private float prevFireTime;

	private byte material;

	private bool waitingToFire;

	private int currentAmmo = 10;

	private bool fireMain = true;

	private bool fireSecondary;

	public CubeBullet cubeBullet;

	public override AvatarItemType Type => AvatarItemType.CubeGun;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => currentAmmo <= 0;

	protected override void OnStart()
	{
		if (ShowCursors())
		{
			primaryCursor = UnityEngine.Object.Instantiate(primaryCursor);
			secondaryCursor = UnityEngine.Object.Instantiate(secondaryCursor);
			secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
		}
		chargeObject.gameObject.SetActive(value: false);
		currentAmmo = ammo;
	}

	private void Update()
	{
		if (ShowCursors())
		{
			HandleCursors();
		}
		if (fireSecondary && GetComponent<AudioSource>().clip != chargeSound)
		{
			GetComponent<AudioSource>().clip = chargeSound;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play();
		}
		if (IsAmmoDepleted && cubeBullet.GetComponent<Renderer>().enabled)
		{
			cubeBullet.GetComponent<Renderer>().enabled = false;
		}
		else if (!IsAmmoDepleted && !cubeBullet.GetComponent<Renderer>().enabled)
		{
			cubeBullet.GetComponent<Renderer>().enabled = true;
		}
	}

	private bool DoLineOfFireCheck(out VoxelHit hit)
	{
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
		return CollisionDetection.MVHit(ray, out hit, range, new HashSet<int> { owner.WorldObjectOwner.Id }, layerMask);
	}

	private bool CanInsertCubeAtCubePos(IntVector cubePos)
	{
		Vector3 vector = SharedCubeFunctions.LocalToWorld(MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, cubePos);
		if ((vector - owner.LookOrigin).magnitude < minDistanceToCubeFire)
		{
			return false;
		}
		return true;
	}

	private void HandleCursors()
	{
		if (DoLineOfFireCheck(out var hit))
		{
			if (fireSecondary)
			{
				primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
				MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(hit.woId);
				if (worldObjectClient is MVCubeModelFineGrainedTerrain)
				{
					if (hit.cubePos != secondaryCursor.LocalPos)
					{
						secondaryCursor.SetCursorCube(hit.cubePos, worldObjectClient.GameObject);
						secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
						secondaryCursor.FadeState = FadeState.FadeIn;
					}
					else if (secondaryCursor.FadeState != FadeState.FadeIn)
					{
						secondaryCursor.FadeState = FadeState.FadeIn;
					}
				}
				else
				{
					secondaryCursor.FadeState = FadeState.FadeOut;
				}
			}
			else
			{
				secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
				IntVector cubePos = GetCubePos(hit);
				if (!CanInsertCubeAtCubePos(cubePos))
				{
					primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
				}
				else if (cubePos != primaryCursor.LocalPos)
				{
					primaryCursor.SetCursorCube(cubePos, MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject);
					primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
					primaryCursor.FadeState = FadeState.FadeIn;
				}
				else if (primaryCursor.FadeState != FadeState.FadeIn)
				{
					primaryCursor.FadeState = FadeState.FadeIn;
				}
			}
		}
		else
		{
			primaryCursor.FadeState = FadeState.FadeOut;
			secondaryCursor.FadeState = FadeState.FadeOut;
		}
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		if (ShowCursors())
		{
			primaryCursor.Destroy();
			secondaryCursor.Destroy();
		}
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		currentAmmo = ammo;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)newState["itemData"];
		material = (byte)dictionary["material"];
		GetComponentInChildren<CubeBullet>().SetCubeMaterial(material);
	}

	protected override void OnFire(bool isLocal)
	{
		if (IsAmmoDepleted)
		{
			return;
		}
		if (DoLineOfFireCheck(out var hit))
		{
			IntVector cubePos = GetCubePos(hit);
			if (!CanInsertCubeAtCubePos(cubePos))
			{
				return;
			}
		}
		Bullet bullet = Bullet.CreateBullet(rocketPrefab, muzzlePoint.position);
		bullet.GetComponentInChildren<CubeBullet>().SetCubeMaterial(material);
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(HandleCubeHitLocal));
		}
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleCubeHit));
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(speed), range: range, ignoreWoIDs: owner.IgnoreWOIDs);
		MVGameController.AudioManager.Play("cubeFire", firePrimary, muzzlePoint.position, 0.6f, SoundRangeDistance.Long);
		currentAmmo--;
	}

	protected void OnFireSecondary(bool isLocal)
	{
		GetComponent<AudioSource>().clip = releaseSound;
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		int num = -5 & ~(1 << LayerMask.NameToLayer("Player"));
		num &= ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, null, num))
		{
			point = voxelHit.point;
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (worldObjectClient is MVCubeModelFineGrainedTerrain)
			{
				currentAmmo++;
				if (isLocal)
				{
					MVGameController.Game.World.RuntimeEventManager.SendRuntimeEvent(new SingleCubeFineGrainedEvent(voxelHit.cubePos));
				}
				MVGameController.AudioManager.Play("cube Destroyed", cubeDestroyedSound, point, 0.6f, SoundRangeDistance.Long);
			}
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = UnityEngine.Object.Instantiate(railGunRayPrefab, muzzlePoint.position, Quaternion.identity) as RailRay;
		railRay.target = point;
		railRay.startColor = Color.yellow;
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (isFiring)
		{
			Debug.Log("Got TriggerStart, but were firing");
		}
		else if (!waitingToFire)
		{
			StartCoroutine(DoAutoFire());
		}
	}

	private bool ShowCursors()
	{
		if (owner.IsLocal && owner.WorldObjectOwner is MVAvatarLocal)
		{
			return true;
		}
		return false;
	}

	private IEnumerator DoAutoFire()
	{
		isFiring = true;
		fireMain = true;
		fireSecondary = false;
		if (Time.time - prevFireTime > (float)fireInterval)
		{
			prevFireTime = Time.time - (float)fireInterval;
		}
		while (isFiring)
		{
			float timeFiring = Time.time - prevFireTime;
			bool prevFireSecondary = fireSecondary;
			fireSecondary = timeFiring > fireIntervalSecondary;
			fireMain = !fireSecondary;
			if (fireSecondary && !prevFireSecondary)
			{
				chargeObject.gameObject.SetActive(value: true);
			}
			yield return 0;
		}
		while (Time.time - prevFireTime <= (float)fireInterval)
		{
			waitingToFire = true;
			yield return 0;
		}
		if (fireMain)
		{
			OnFire(owner.IsLocal);
			prevFireTime = Time.time;
		}
		if (fireSecondary)
		{
			OnFireSecondary(owner.IsLocal);
			prevFireTime = Time.time;
			chargeObject.gameObject.SetActive(value: false);
		}
		fireMain = false;
		fireSecondary = false;
		waitingToFire = false;
		if (!IsAmmoDepleted)
		{
		}
	}

	private bool GetCubePosFromFineGrainedTerrain(VoxelHit voxelHit, float maxDistanceToEdge, ref IntVector pos)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(voxelHit.woId);
		if (worldObjectClient is MVCubeModelFineGrainedTerrain)
		{
			Edge edge = Cube.GetEdge(worldObjectClient.GameObject, voxelHit.cube, voxelHit.face, voxelHit.point, voxelHit.cubePos);
			Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(worldObjectClient.GameObject, voxelHit.cube, voxelHit.face, edge, voxelHit.cubePos);
			float distance = 0f;
			if (MathFunctions.DistancePointLine(voxelHit.point, edgeVerticesWorld[0], edgeVerticesWorld[1], ref distance) && distance < maxDistanceToEdge)
			{
				foreach (int value in Enum.GetValues(typeof(Face)))
				{
					if (value == (int)voxelHit.face)
					{
						continue;
					}
					foreach (int value2 in Enum.GetValues(typeof(Edge)))
					{
						if (value2 == 0)
						{
							continue;
						}
						Vector3[] edgeVerticesWorld2 = Cube.GetEdgeVerticesWorld(worldObjectClient.GameObject, voxelHit.cube, (Face)value, (Edge)value2, voxelHit.cubePos);
						Debug.DrawLine(edgeVerticesWorld2[0], edgeVerticesWorld2[1], Color.cyan, 10f);
						int num = 0;
						Vector3[] array = edgeVerticesWorld2;
						foreach (Vector3 b in array)
						{
							if (Vector3.Distance(edgeVerticesWorld[0], b) < 0.01f)
							{
								num++;
							}
							if (Vector3.Distance(edgeVerticesWorld[1], b) < 0.01f)
							{
								num++;
							}
						}
						if (num == 2)
						{
							pos = Cube.GetCubePosAboveFace(voxelHit.cubePos, (Face)value);
							if (((MVCubeModelFineGrainedTerrain)worldObjectClient).GetCube(pos) == null)
							{
								return true;
							}
							return false;
						}
					}
				}
			}
		}
		return false;
	}

	private void HandleCubeHitLocal(VoxelHit voxelHit, Ray lineOfFire)
	{
		IntVector cubePos = GetCubePos(voxelHit);
		MVGameController.Game.World.RuntimeEventManager.SendRuntimeEvent(new SingleCubeFineGrainedEvent(cubePos, material));
	}

	private void HandleCubeHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameController.AudioManager.Play("cubeLanded", cubeLandedSound, voxelHit.point, 0.6f, SoundRangeDistance.Long);
	}

	private IntVector GetCubePos(VoxelHit voxelHit)
	{
		IntVector pos = default;
		if (!GetCubePosFromFineGrainedTerrain(voxelHit, 0.2f, ref pos))
		{
			return SharedCubeFunctions.WorldToLocal(MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, voxelHit.point + voxelHit.normal * 0.1f);
		}
		return pos;
	}
}
