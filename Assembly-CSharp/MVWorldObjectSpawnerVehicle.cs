using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MVWorldObjectSpawnerVehicle : MVWorldObjectSpawner
{
	private CullingSubscriberBase cullingSubscriberBase;

	private GameObject lodGameObject;

	private GameObject groundAura;

	private bool initFlag;

	private GreyOutObjectScript pickupItemObjectScript;

	private SpawnerObject spawnerObject;

	public int SpawnWorldObjectID => spawnWorldObjectID;

	public MVWorldObjectSpawnerVehicle(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.SpawnerObjectPrefab, worldObjects)
	{
		previewLayerMask |= LayerFlags.Player;
		spawnerObject = (SpawnerObject)component;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 localScale = gameObject.transform.localScale;
		localScale.y = 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, localScale);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
	}

	public override void Destroy()
	{
		if (initFlag)
		{
			useInteractor.OnDestroy(Data);
		}
		base.Destroy();
		if (cullingSubscriberBase != null)
		{
			cullingSubscriberBase.Destroy();
			cullingSubscriberBase = null;
		}
	}

	public override void Initialize()
	{
		if (GetChild("spawnWorldObjectID") == null)
		{
			Debug.LogError("Could not get spawnPoint child spawnWorldObjectID. This should only happend when creating a spawner with a new vehicle");
			return;
		}
		base.Initialize();
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild(spawnWorldObjectID);
		useInteractor = new UseInteractor(this, spawnerObject.UseInteractorRotator, reset: true, triggerBoxEvents.Collider, Use, CheckCanUse, 3.5f);
		triggerBoxEvents.TriggerEnterOverride += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExitOverride += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(spawnerObject.UseInteractorRotator);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(spawnerObject.UseInteractorRotator);
		useInteractor.AddRequirement(useRequirement2);
		useInteractor.UpdateData(Data);
		lodGameObject = mVVehicleBase.Visualization.gameObject;
		InitializeCommon();
		interactionFlags |= mVVehicleBase.InteractionFlags;
		interactionFlags |= InteractionFlags.DirectlySelectable;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		pickupItemObjectScript = gameObject.AddComponent<GreyOutObjectScript>();
		pickupItemObjectScript.hiddenShader = MVGameControllerBase.MaterialLoader.PickupItemShader;
		if (pickupItemObjectScript.hiddenShader == null)
		{
			Debug.LogError("hiddenShader not found");
		}
		pickupItemObjectScript.pickupObject = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnWorldObjectID).GameObject;
		pickupItemObjectScript.InitializeOriginalMaterials();
		initFlag = true;
		SetupCulling();
	}

	protected void SetupCulling()
	{
		PositionChanged = (UnityAction<MVWorldObjectClient, PositionChangedEventArgs>)Delegate.Combine(PositionChanged, new UnityAction<MVWorldObjectClient, PositionChangedEventArgs>(OnPositionChanged));
		cullingSubscriberBase = new CullingSubscriberBase(2.5f, WorldPosition, OnStateChanged);
		cullingSubscriberBase.DistanceBandIndex = 2;
	}

	private void OnPositionChanged(MVWorldObjectClient arg0, PositionChangedEventArgs positionChangedEventArgs)
	{
		cullingSubscriberBase.Position = positionChangedEventArgs.NewPos;
	}

	protected virtual void OnStateChanged(CullingGroupEvent cullingGroupEvent)
	{
		bool active = CullingApiWrapper.Visible(cullingGroupEvent, cullingSubscriberBase.DistanceBandIndex);
		lodGameObject.SetActive(active);
		groundAura.SetActive(active);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	private void InitializeCommon()
	{
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild("spawnWorldObjectID");
		groundAura = UnityEngine.Object.Instantiate(PrefabPool.Instance.ParticleCFX_GroundAura, Vector3.zero, Quaternion.identity);
		groundAura.transform.parent = gameObject.transform;
		groundAura.transform.localPosition = Vector3.zero + Vector3.up * (0f - mVVehicleBase.GetLocalBounds(BoundsContext.BoxVisualization).extents.y) * 0.9f;
		groundAura.transform.rotation = Quaternion.identity;
		documentationType = mVVehicleBase.DocumentationType;
	}

	protected override void OnSpawnStateChange(SpawnState spawnState)
	{
		if (spawnState == SpawnState.None)
		{
			Debug.LogError("SpawnState is none");
		}
		switch (spawnState)
		{
		case SpawnState.Listening:
			pickupItemObjectScript.GreyIn();
			groundAura.SetActive(value: true);
			break;
		case SpawnState.Taken:
			if (MVGameControllerBase.Game.IsPlaying)
			{
				pickupItemObjectScript.GreyOut();
			}
			groundAura.SetActive(value: false);
			break;
		}
	}

	public override void Select(Color color)
	{
		MVGameControllerBase.WOCM.GetWorldObjectClient(SpawnWorldObjectID)?.Select(color);
	}

	protected override bool CheckCanUse(MVInteractableBase avatarInteractable)
	{
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			return false;
		}
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisableVehicles) || avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisablePickups))
		{
			return false;
		}
		return true;
	}

	protected override bool Use(int userWoID)
	{
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError("SpawnWorldObject is null");
			return false;
		}
		VehicleSeatManager vehicleSeatManager = worldObjectClient.GameObject.GetComponent<VehicleSeatManager>();
		if (vehicleSeatManager == null)
		{
			Debug.LogError("No vehicleSeatManager");
			return false;
		}
		VehicleSeatBase driverSeat = vehicleSeatManager.DriverSeat;
		if (driverSeat == null)
		{
			Debug.LogError("No driver seat");
			return false;
		}
		if (MVGameControllerBase.Game.PlayerController.SpawnVehicleWithDriver(Id, userWoID, driverSeat))
		{
			return true;
		}
		return false;
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			pickupItemObjectScript.GreyIn();
		}
		return worldObjectClient.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		return worldObjectClient.OnExitObject(e);
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		if (!(wo is MVWorldObjectSpawnerVehicle))
		{
			Debug.LogError("Not a vehicle spawner");
			return false;
		}
		MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)wo;
		if (Data.ContainsKey("gameCoinAmount") || Data.ContainsKey("starAmount") || Data.ContainsKey("levelAmount"))
		{
			return false;
		}
		MVWorldObjectClient child = mVWorldObjectSpawnerVehicle.GetChild("spawnWorldObjectID");
		if (child == null)
		{
			Debug.LogError("Did not find other spawnWorldObject");
			return false;
		}
		return GetChild("spawnWorldObjectID").CompareWithKoGaMaPackage(child, koGaMaPackageClient, ref insertedByProfileId);
	}
}
