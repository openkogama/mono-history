using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemCubeGun : PickupItemWithDelay
{
	[SerializeField]
	private float minDistanceToCubeFire = 0.8f;

	[SerializeField]
	private ObscuredInt maxAmmo = 10;

	[SerializeField]
	private float speed = 30f;

	[SerializeField]
	private float range = 200f;

	[SerializeField]
	private Transform chargeObject;

	[SerializeField]
	private ObscuredFloat fireIntervalSecondary = 0.5f;

	[SerializeField]
	private GUICellCursor primaryCursor;

	[SerializeField]
	private GUICellCursor secondaryCursor;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private CubeBullet cubeBullet;

	private float prevFireTime;

	private byte material;

	private bool waitingToFire;

	private ObscuredInt currentAmmo = 0;

	private bool fireMain;

	private bool fireSecondary;

	private bool showingCursors;

	private bool hasLeftVehicle;

	public override AvatarItemType Type => AvatarItemType.CubeGun;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	private void Awake()
	{
		currentAmmo = Mathf.Max(maxAmmo, currentAmmo);
		primaryCursor = UnityEngine.Object.Instantiate(primaryCursor);
		secondaryCursor = UnityEngine.Object.Instantiate(secondaryCursor);
	}

	private void Start()
	{
		showingCursors = ShowCursors();
		if (showingCursors)
		{
			primaryCursor.fadeInTime = 0.2f;
			secondaryCursor.fadeInTime = 0.2f;
			primaryCursor.fadeOutTime = 0.2f;
			secondaryCursor.fadeOutTime = 0.2f;
			secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
		}
	}

	private void Update()
	{
		if (showingCursors)
		{
			HandleCursors();
		}
		if (IsAmmoDepleted && cubeBullet.MeshRenderer.enabled)
		{
			cubeBullet.MeshRenderer.enabled = false;
		}
		else if (!IsAmmoDepleted && !cubeBullet.MeshRenderer.enabled)
		{
			cubeBullet.MeshRenderer.enabled = true;
		}
	}

	public override void OnEquip()
	{
		base.OnEquip();
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, TM._("Hold shoot button to remove cubes."));
		Dictionary<object, object> data = dictionary;
		NotificationController.PushNotification(NotificationType.PlayerTip, NotificationsManager.eNotificationPanel.secondary, data);
	}

	private bool DoLineOfFireCheck(out VoxelHit hit)
	{
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
		return CollisionDetection.MVHit(ray, out hit, range, new HashSet<int> { owner.WorldObjectOwner.Id }, layerMask);
	}

	public override void OnLeaveVehicleWithWeapon()
	{
		base.OnLeaveVehicleWithWeapon();
		hasLeftVehicle = true;
		showingCursors = false;
		primaryCursor.FadeState = FadeState.FadeOut;
		secondaryCursor.FadeState = FadeState.FadeOut;
		primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
		secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
	}

	public override void OnEnterVehicleWithWeapon()
	{
		base.OnEnterVehicleWithWeapon();
		hasLeftVehicle = false;
		showingCursors = ShowCursors();
	}

	protected override void OnHolstered()
	{
		base.OnHolstered();
		showingCursors = false;
		primaryCursor.FadeState = FadeState.FadeOut;
		secondaryCursor.FadeState = FadeState.FadeOut;
		primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
		secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
	}

	protected override void OnUnholstered()
	{
		base.OnUnholstered();
		showingCursors = ShowCursors();
	}

	private bool CanInsertCubeAtCubePos(IntVector cubePos)
	{
		Vector3 vector = SharedCubeFunctions.LocalToWorld(MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, cubePos);
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
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);
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
					primaryCursor.SetCursorCube(cubePos, MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject);
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
		if (showingCursors)
		{
			primaryCursor.FadeOverride = FadeOverride.FadeAllOut;
			secondaryCursor.FadeOverride = FadeOverride.FadeAllOut;
			primaryCursor.Destroy();
			secondaryCursor.Destroy();
		}
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = Mathf.Max(maxAmmo, currentAmmo);
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)newState["itemData"];
		material = (byte)dictionary["material"];
		cubeBullet.SetCubeMaterial(material);
	}

	protected override void OnFire(bool isLocal)
	{
		if (hasLeftVehicle || fireSecondary || IsAmmoDepleted)
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
		CubeGunBulletObject cubeGunBulletObject = CubeGunBulletObject.Create(owner, muzzlePoint.position, material);
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		cubeGunBulletObject.Bullet.Fire(owner.GetAbsolutProjectileSpeed(speed), range, lineOfFire, owner.IgnoreWOIDs);
		currentAmmo = (int)currentAmmo - 1;
	}

	protected void OnFireSecondary(bool isLocal)
	{
		MVGameControllerBase.AudioManager.Play("CubeGun", audioSource, muzzlePoint.position);
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		int num = -5 & ~(1 << LayerMask.NameToLayer("Player"));
		num &= ~(1 << (LayerMask.NameToLayer("Logic") & 0x1F));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, null, num))
		{
			point = voxelHit.point;
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
			if (voxelHit.isCubeHit)
			{
				float toughness = MVGameControllerBase.Game.MaterialRepository.GetMaterial(voxelHit.cube.FaceMaterials[0]).physicalProperties.toughness;
				if (toughness != 0f)
				{
					currentAmmo = (int)currentAmmo + 1;
					if (isLocal)
					{
						MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, float.PositiveInfinity);
					}
				}
				if (toughness == 0f && worldObjectClient is MVCubeModelFineGrainedTerrain)
				{
					currentAmmo = (int)currentAmmo + 1;
					if (isLocal)
					{
						MVCubeModelFineGrainedTerrain mVCubeModelFineGrainedTerrain = (MVCubeModelFineGrainedTerrain)worldObjectClient;
						mVCubeModelFineGrainedTerrain.RemoveCube(voxelHit.cubePos);
						mVCubeModelFineGrainedTerrain.HandleDelta();
					}
					SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleCubeDust, point, 1f);
				}
			}
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = PrefabPool.Instance.EnumPoolManager.Instantiate<RailRay>(PoolEnums.CubeGunRay);
		railRay.target = point;
		railRay.transform.localPosition = muzzlePoint.position;
		railRay.Reset();
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (!isFiring && !waitingToFire)
		{
			StartFire();
		}
	}

	public override void TriggerEnd()
	{
		Execute();
		fireMain = false;
		fireSecondary = false;
		waitingToFire = false;
		isFiring = false;
	}

	private void StartFire()
	{
		isFiring = true;
		fireMain = true;
		fireSecondary = false;
		if (Time.time - prevFireTime > (float)fireInterval)
		{
			prevFireTime = Time.time - (float)fireInterval;
		}
	}

	private bool ShowCursors()
	{
		if (owner == null)
		{
			return false;
		}
		if (IsHolstered)
		{
			return false;
		}
		if (owner.IsLocal && (owner.WorldObjectOwner is MVAvatarLocal || owner.WorldObjectOwner is MVVehicleBase))
		{
			return true;
		}
		return false;
	}

	public override void UpdateControllerUpdate()
	{
		if (isFiring)
		{
			DoAutoFire();
		}
	}

	private void DoAutoFire()
	{
		float num = Time.time - prevFireTime;
		bool flag = fireSecondary;
		fireSecondary = num > (float)fireIntervalSecondary;
		fireMain = !fireSecondary;
		if (fireSecondary && !flag)
		{
			chargeObject.gameObject.SetActive(value: true);
		}
	}

	private void Execute()
	{
		if (Time.time - prevFireTime <= (float)fireInterval)
		{
			waitingToFire = true;
			return;
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
	}

	private static bool GetCubePosFromFineGrainedTerrain(VoxelHit voxelHit, float maxDistanceToEdge, ref IntVector pos)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
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

	public static IntVector GetCubePos(VoxelHit voxelHit)
	{
		IntVector pos = default;
		if (!GetCubePosFromFineGrainedTerrain(voxelHit, 0.2f, ref pos))
		{
			return SharedCubeFunctions.WorldToLocal(MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>().GameObject, voxelHit.point + voxelHit.normal * 0.1f);
		}
		return pos;
	}
}
