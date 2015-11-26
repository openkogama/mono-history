using System.Collections.Generic;
using UnityEngine;

public class MVWorldObjectSpawnerVehicle : MVWorldObjectSpawner
{
	private GameObject groundAura;

	private bool initFlag;

	private GreyOutObjectScript pickupItemObjectScript;

	private Vector3 displayObjectOffset = new Vector3(0f, 1.8f, 0f);

	protected float cullDistance = 145f;

	protected bool disabledByLod;

	public int SpawnWorldObjectID => spawnWorldObjectID;

	public MVWorldObjectSpawnerVehicle(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		previewLayerMask |= LayerFlags.Player;
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
	}

	public override void Initialize()
	{
		if (GetChild("spawnWorldObjectID") == null)
		{
			Debug.LogError("Could not get spawnPoint child spawnWorldObjectID. This should only happend when creating a spawner with a new vehicle");
			return;
		}
		base.Initialize();
		useInteractor = new UseInteractor(Id, gameObject, reset: true, triggerBoxEvents.GetComponent<Collider>(), Use, CheckCanUse);
		triggerBoxEvents.TriggerEnterOverride += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExitOverride += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, displayObjectOffset);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject);
		useInteractor.AddRequirement(useRequirement2);
		useInteractor.UpdateData(Data);
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild(spawnWorldObjectID);
		InitializeCommon();
		interactionFlags |= mVVehicleBase.InteractionFlags;
		interactionFlags |= InteractionFlags.DirectlySelectable;
		interactionFlags |= InteractionFlags.CanUseLevel;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		pickupItemObjectScript = gameObject.AddComponent<GreyOutObjectScript>();
		pickupItemObjectScript.hiddenShader = Shader.Find("Custom/Pickup Unavailable");
		if (pickupItemObjectScript.hiddenShader == null)
		{
			Debug.LogError("hiddenShader not found");
		}
		pickupItemObjectScript.pickupObject = MVGameControllerBase.WOCM.GetWorldObjectClient(spawnWorldObjectID).GameObject;
		pickupItemObjectScript.InitializeOriginalMaterials();
		initFlag = true;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	private void InitializeCommon()
	{
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild("spawnWorldObjectID");
		groundAura = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/CFX_GroundAura"), Vector3.zero, Quaternion.identity);
		groundAura.transform.parent = gameObject.transform;
		groundAura.transform.localPosition = Vector3.zero + Vector3.up * (0f - mVVehicleBase.GetLocalBounds(BoundsContext.BoxVisualization).extents.y) * 0.9f;
		groundAura.transform.rotation = Quaternion.identity;
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

	public override void ChangeLOD(float distance)
	{
		if (distance < cullDistance)
		{
			disabledByLod = false;
			Renderer[] componentsInChildren = groundAura.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			Renderer[] componentsInChildren2 = groundAura.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer renderer2 in array2)
			{
				renderer2.enabled = false;
			}
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
		if (avatarInteractable.HasModifierEffect(AvatarModifierEffect.DisableVehicles))
		{
			return false;
		}
		return true;
	}

	protected override bool Use(int userWoID)
	{
		Debug.Log("SpawnObjectID " + spawnWorldObjectID);
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
		VehicleSeatManager component = worldObjectClient.GameObject.GetComponent<VehicleSeatManager>();
		if (component == null)
		{
			Debug.LogError("No vehicleSeatManager");
			return false;
		}
		VehicleSeatBase driverSeat = component.DriverSeat;
		if (driverSeat == null)
		{
			Debug.LogError("No driver seat");
			return false;
		}
		Debug.Log("Trying to spawn vehicle spawner ID is " + Id);
		if (MVGameControllerBase.Game.PlayerController.SpawnVehicleWithDriver(Id, userWoID, driverSeat))
		{
			Debug.Log("Succesfully send spawn vehicle");
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
