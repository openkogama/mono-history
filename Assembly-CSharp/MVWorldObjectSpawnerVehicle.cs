using System.Collections.Generic;
using UnityEngine;

public class MVWorldObjectSpawnerVehicle : MVWorldObjectSpawner
{
	private GameObject groundAura;

	private GreyOutObjectScript pickupItemObjectScript;

	private GameCoinLogic gameCoinLogic;

	private Vector3 displayObjectOffset = new Vector3(0f, 1.8f, 0f);

	protected float cullDistance = 145f;

	protected bool disabledByLod;

	public int SpawnWorldObjectID => spawnWorldObjectID;

	public GameCoinLogic GameCoinLogic => gameCoinLogic;

	public MVWorldObjectSpawnerVehicle(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		previewLayerMask |= LayerFlags.Player;
		gameCoinLogic = new GameCoinLogic(gameObject, Data, displayObjectOffset);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		gameCoinLogic.OnDataUpdate(Data);
	}

	public override void Destroy()
	{
		if (gameCoinLogic != null)
		{
			gameCoinLogic.OnDestroy(Data);
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
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild(spawnWorldObjectID);
		InitializeCommon();
		interactionFlags |= mVVehicleBase.InteractionFlags;
		interactionFlags |= InteractionFlags.DirectlySelectable;
		pickupItemObjectScript = gameObject.AddComponent<GreyOutObjectScript>();
		pickupItemObjectScript.hiddenShader = Shader.Find("Custom/Pickup Unavailable");
		if (pickupItemObjectScript.hiddenShader == null)
		{
			Debug.LogError("hiddenShader not found");
		}
		pickupItemObjectScript.pickupObject = MVGameController.WOCM.GetWorldObjectClient(spawnWorldObjectID).GameObject;
		pickupItemObjectScript.InitializeOriginalMaterials();
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
			if (MVGameController.Game.IsPlaying)
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
		MVGameController.WOCM.GetWorldObjectClient(SpawnWorldObjectID)?.Select(color);
	}

	protected override bool Use(int userWoID)
	{
		Debug.Log("SpawnObjectID " + spawnWorldObjectID);
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError("SpawnWorldObject is null");
			return false;
		}
		if (!gameCoinLogic.CanUse())
		{
			Debug.Log("Need " + gameCoinLogic.PurchaseAmount + " GameCoins to use this vehicle");
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
		if (MVGameController.Game.PlayerController.SpawnVehicleWithDriver(Id, userWoID, driverSeat))
		{
			Debug.Log("Succesfully send spawn vehicle");
			MVGameController.Game.GameCoinManager.Consume(gameCoinLogic);
			return true;
		}
		return true;
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			pickupItemObjectScript.GreyIn();
		}
		return worldObjectClient.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(spawnWorldObjectID);
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
		if (mVWorldObjectSpawnerVehicle.Data.ContainsKey("gameCoinAmount"))
		{
			if (!Data.ContainsKey("gameCoinAmount"))
			{
				return false;
			}
			if (Data["gameCoinAmount"] != mVWorldObjectSpawnerVehicle.Data["gameCointAmount"])
			{
				return false;
			}
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
