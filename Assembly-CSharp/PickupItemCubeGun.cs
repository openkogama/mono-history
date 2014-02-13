using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemCubeGun : PickupItemWithDelay
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
			primaryCursor = Object.Instantiate((Object)(object)primaryCursor) as GUICellCursor;
			secondaryCursor = Object.Instantiate((Object)(object)secondaryCursor) as GUICellCursor;
			secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
		}
		((Component)chargeObject).gameObject.SetActiveRecursively(false);
		currentAmmo = ammo;
	}

	private void Update()
	{
		if (ShowCursors())
		{
			HandleCursors();
		}
		if (fireSecondary && (Object)(object)((Component)this).audio.clip != (Object)(object)chargeSound)
		{
			((Component)this).audio.clip = chargeSound;
			((Component)this).audio.loop = true;
			((Component)this).audio.Play();
		}
		if (IsAmmoDepleted && ((Component)cubeBullet).renderer.enabled)
		{
			((Component)cubeBullet).renderer.enabled = false;
		}
		else if (!IsAmmoDepleted && !((Component)cubeBullet).renderer.enabled)
		{
			((Component)cubeBullet).renderer.enabled = true;
		}
	}

	private bool DoLineOfFireCheck(out VoxelHit hit)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		LayerMask val = LayerMask.op_Implicit(1 << LayerMask.NameToLayer("Default"));
		return CollisionDetection.MVHit(ray, out hit, range, new HashSet<int> { owner.WorldObjectOwner.Id }, LayerMask.op_Implicit(val));
	}

	private bool CanInsertCubeAtCubePos(IntVector cubePos)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = SharedCubeFunctions.LocalToWorld(MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, cubePos);
		Vector3 val2 = val - owner.LookOrigin;
		if (val2.magnitude < minDistanceToCubeFire)
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
				MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
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
					primaryCursor.SetCursorCube(cubePos, MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject);
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

	public override void OnStateChanged(Hashtable newState)
	{
		currentAmmo = ammo;
		Hashtable hashtable = (Hashtable)newState["itemData"];
		material = (byte)hashtable["material"];
		((Component)this).GetComponentInChildren<CubeBullet>().SetCubeMaterial(material);
	}

	protected override void OnFire(bool isLocal)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
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
		((Component)bullet).GetComponentInChildren<CubeBullet>().SetCubeMaterial(material);
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(HandleCubeHitLocal));
		}
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleCubeHit));
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(speed), range: range, ignoreWoIDs: owner.IgnoreWOIDs);
		MVGameController.Instance.AudioManager.Play("cubeFire", firePrimary, muzzlePoint.position, 0.6f, SoundRangeDistance.Long);
		currentAmmo--;
	}

	protected void OnFireSecondary(bool isLocal)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).audio.clip = releaseSound;
		((Component)this).audio.loop = false;
		((Component)this).audio.Play();
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		int num = -5 & ~(1 << LayerMask.NameToLayer("Player"));
		num &= ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, null, num))
		{
			point = voxelHit.point;
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (worldObjectClient is MVCubeModelFineGrainedTerrain)
			{
				currentAmmo++;
				if (isLocal)
				{
					MVCubeModelFineGrainedTerrain mVCubeModelFineGrainedTerrain = (MVCubeModelFineGrainedTerrain)worldObjectClient;
					mVCubeModelFineGrainedTerrain.RemoveCube(voxelHit.cubePos);
					mVCubeModelFineGrainedTerrain.HandleDelta();
				}
				MVGameController.Instance.AudioManager.Play("cube Destroyed", cubeDestroyedSound, point, 0.6f, SoundRangeDistance.Long);
			}
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = Object.Instantiate((Object)(object)railGunRayPrefab, muzzlePoint.position, Quaternion.identity) as RailRay;
		railRay.target = point;
		railRay.startColor = Color.yellow;
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (isFiring)
		{
			Debug.Log((object)"Got TriggerStart, but were firing");
		}
		else if (!waitingToFire)
		{
			((MonoBehaviour)this).StartCoroutine(DoAutoFire());
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
		if (Time.time - prevFireTime > fireInterval)
		{
			prevFireTime = Time.time - fireInterval;
		}
		while (isFiring)
		{
			float timeFiring = Time.time - prevFireTime;
			bool prevFireSecondary = fireSecondary;
			fireSecondary = timeFiring > fireIntervalSecondary;
			fireMain = !fireSecondary;
			if (fireSecondary && !prevFireSecondary)
			{
				((Component)chargeObject).gameObject.SetActiveRecursively(true);
			}
			yield return 0;
		}
		while (Time.time - prevFireTime <= fireInterval)
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
			((Component)chargeObject).gameObject.SetActiveRecursively(false);
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
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
						foreach (Vector3 val in array)
						{
							if (Vector3.Distance(edgeVerticesWorld[0], val) < 0.01f)
							{
								num++;
							}
							if (Vector3.Distance(edgeVerticesWorld[1], val) < 0.01f)
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
		MVCubeModelFineGrainedTerrain singletonWorldObject = MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
		singletonWorldObject.AddCube(cubePos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(material)));
		singletonWorldObject.HandleDelta();
	}

	private void HandleCubeHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.AudioManager.Play("cubeLanded", cubeLandedSound, voxelHit.point, 0.6f, SoundRangeDistance.Long);
	}

	private IntVector GetCubePos(VoxelHit voxelHit)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		IntVector pos = default;
		if (!GetCubePosFromFineGrainedTerrain(voxelHit, 0.2f, ref pos))
		{
			return SharedCubeFunctions.WorldToLocal(MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, voxelHit.point + voxelHit.normal * 0.1f);
		}
		return pos;
	}
}
