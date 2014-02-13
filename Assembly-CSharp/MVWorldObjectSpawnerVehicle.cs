using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVWorldObjectSpawnerVehicle : MVWorldObjectSpawner
{
	private GameObject groundAura;

	private PickupItemObjectScript pickupItemObjectScript;

	protected float cullDistance = 145f;

	protected bool disabledByLod;

	public int SpawnWorldObjectID => spawnWorldObjectID;

	public MVWorldObjectSpawnerVehicle(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		previewLayerMask |= LayerFlags.Player;
	}

	public override void Initialize()
	{
		if (GetChild("spawnWorldObjectID") == null)
		{
			Debug.LogError((object)"Could not get spawnPoint child spawnWorldObjectID. This should only happend when creating a spawner with a new vehicle");
			return;
		}
		base.Initialize();
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild(spawnWorldObjectID);
		InitializeCommon();
		interactionFlags |= mVVehicleBase.InteractionFlags;
		interactionFlags |= InteractionFlags.DirectlySelectable;
		pickupItemObjectScript = gameObject.AddComponent<PickupItemObjectScript>();
		pickupItemObjectScript.hiddenShader = Shader.Find("Custom/Pickup Unavailable");
		if ((Object)(object)pickupItemObjectScript.hiddenShader == (Object)null)
		{
			Debug.LogError((object)"hiddenShader not found");
		}
		pickupItemObjectScript.pickupObject = MVGameController.Instance.WOCM.GetWorldObjectClient(spawnWorldObjectID).GameObject;
		pickupItemObjectScript.InitializeOriginalMaterials();
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		InitializeCommon();
	}

	private void InitializeCommon()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		MVVehicleBase mVVehicleBase = (MVVehicleBase)GetChild("spawnWorldObjectID");
		groundAura = (GameObject)Object.Instantiate(Resources.Load("ParticleFX/CFX_GroundAura"), Vector3.zero, Quaternion.identity);
		groundAura.transform.parent = gameObject.transform;
		Transform val = groundAura.transform;
		Vector3 zero = Vector3.zero;
		Vector3 up = Vector3.up;
		Bounds localBounds = mVVehicleBase.GetLocalBounds(BoundsContext.BoxVisualization);
		val.localPosition = zero + up * (0f - localBounds.extents.y) * 0.9f;
		groundAura.transform.rotation = Quaternion.identity;
	}

	protected override void OnSpawnStateChange(SpawnState spawnState)
	{
		if (spawnState == SpawnState.None)
		{
			Debug.LogError((object)"SpawnState is none");
		}
		switch (spawnState)
		{
		case SpawnState.Listening:
			pickupItemObjectScript.Respawn();
			groundAura.active = true;
			break;
		case SpawnState.Taken:
			if (MVGameController.Instance.Game.IsPlaying)
			{
				pickupItemObjectScript.Take();
			}
			groundAura.active = false;
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
			foreach (Renderer val in array)
			{
				val.enabled = true;
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			Renderer[] componentsInChildren2 = groundAura.GetComponentsInChildren<Renderer>();
			Renderer[] array2 = componentsInChildren2;
			foreach (Renderer val2 in array2)
			{
				val2.enabled = false;
			}
		}
	}

	public override void Select(Color color)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.WOCM.GetWorldObjectClient(SpawnWorldObjectID)?.Select(color);
	}

	protected override bool Use(int userWoID)
	{
		Debug.Log((object)("SpawnObjectID " + spawnWorldObjectID));
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (worldObjectClient == null)
		{
			Debug.LogError((object)"SpawnWorldObject is null");
			return false;
		}
		VehicleSeatManager component = worldObjectClient.GameObject.GetComponent<VehicleSeatManager>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"No vehicleSeatManager");
			return false;
		}
		VehicleSeatBase driverSeat = component.DriverSeat;
		if ((Object)(object)driverSeat == (Object)null)
		{
			Debug.LogError((object)"No driver seat");
			return false;
		}
		Debug.Log((object)("Trying to spawn vehicle spawner ID is " + Id));
		if (MVGameController.Instance.Game.PlayerController.SpawnVehicleWithDriver(Id, userWoID, driverSeat))
		{
			Debug.Log((object)"Succesfully send spawn vehicle");
			return true;
		}
		return true;
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		if (spawnStateWrapper.SpawnState == SpawnState.Taken)
		{
			pickupItemObjectScript.Respawn();
		}
		return worldObjectClient.OnEnterObject(e);
	}

	public override bool OnExitObject(EditorStateMachine e)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(spawnWorldObjectID);
		return worldObjectClient.OnExitObject(e);
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		if (!(wo is MVWorldObjectSpawnerVehicle))
		{
			Debug.LogError((object)"Not a vehicle spawner");
			return false;
		}
		MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)wo;
		MVWorldObjectClient child = mVWorldObjectSpawnerVehicle.GetChild("spawnWorldObjectID");
		if (child == null)
		{
			Debug.LogError((object)"Did not find other spawnWorldObject");
			return false;
		}
		return GetChild("spawnWorldObjectID").CompareWithKoGaMaPackage(child, koGaMaPackageClient, ref insertedByProfileId);
	}
}
