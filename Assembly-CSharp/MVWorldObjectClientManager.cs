using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectClientManager : IWorldObjectManager
{
	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(MVWorldObjectClientManager));

	private Dictionary<int, MVPlayer> players;

	private FriendList friends;

	private int avatarId = -1;

	private bool updateLOD = true;

	private WorldObjectsIdsLodBookkeeping worldObjectsIdsLodBookkeeping = new WorldObjectsIdsLodBookkeeping(new List<int>());

	private readonly Dictionary<int, MVWorldObjectClient> worldObjects = new Dictionary<int, MVWorldObjectClient>();

	private readonly Dictionary<WorldObjectType, HashSet<int>> worldObjectTypeSets = new Dictionary<WorldObjectType, HashSet<int>>();

	private readonly Dictionary<int, int> gameObjectIdToWorldObjectIdMap = new Dictionary<int, int>();

	private readonly Queue<MVWorldObjectClient> pendingRegisterQueue = new Queue<MVWorldObjectClient>();

	private readonly Queue<MVWorldObjectClient> pendingUnregisterQueue = new Queue<MVWorldObjectClient>();

	private readonly Queue<int> pendingUngroupQueue = new Queue<int>();

	private readonly MVMaterialRepository materialRepository = new MVMaterialRepository();

	private readonly Dictionary<int, Link> links = new Dictionary<int, Link>();

	private readonly Queue<Link> pendingLinkQueue = new Queue<Link>();

	private readonly Queue<Link> pendingRemoveLinkQueue = new Queue<Link>();

	private readonly Dictionary<int, GameObject> linkObjects = new Dictionary<int, GameObject>();

	private readonly LinkGraph linkGraph = new LinkGraph();

	private MVWorldInventory worldInventory;

	private PlayerRepository playerRepository;

	private int localPlayerActorNumber = -1;

	private int rootGroupId = -1;

	private bool hierarchiesHasBeenCreated;

	private MVCubeModelPrototypeTerrain terrain;

	private MVCubeModelFineGrainedTerrain fineGrainedTerrain;

	private Bounds worldBounds = default;

	public HashSet<MVWorldObjectClient> worldObjectsToUpdate = new HashSet<MVWorldObjectClient>();

	public bool HierarchiesHasBeenCreated => hierarchiesHasBeenCreated;

	public MVGroup RootGroup
	{
		get
		{
			return (MVGroup)worldObjects[rootGroupId];
		}
		set
		{
			rootGroupId = value.Id;
		}
	}

	public Dictionary<int, MVWorldObjectClient> WorldObjects => worldObjects;

	public MVPlayer LocalPlayer => players[localPlayerActorNumber];

	public int LocalPlayerActorNumber
	{
		get
		{
			return localPlayerActorNumber;
		}
		set
		{
			if (localPlayerActorNumber == -1)
			{
				localPlayerActorNumber = value;
			}
		}
	}

	public MVWorldInventory WorldInventory => worldInventory;

	public MVMaterialRepository MaterialRepository => materialRepository;

	public PlayerRepository PlayerRepository => playerRepository;

	public Dictionary<int, MVPlayer> Players => players;

	public int AvatarId
	{
		set
		{
			avatarId = value;
		}
	}

	public MVCubeModelPrototypeTerrain Terrain
	{
		get
		{
			return terrain;
		}
		set
		{
			if (terrain != null)
			{
				Debug.LogError((object)"only one terrain allowed");
			}
			terrain = value;
		}
	}

	public MVCubeModelFineGrainedTerrain FineGrainedTerrain
	{
		get
		{
			return fineGrainedTerrain;
		}
		set
		{
			if (fineGrainedTerrain != null)
			{
				Debug.LogError((object)"only one terrain allowed");
			}
			fineGrainedTerrain = value;
		}
	}

	public Bounds WorldBounds
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return worldBounds;
		}
	}

	public MVAvatar WoAvatar
	{
		get
		{
			if (worldObjects.ContainsKey(avatarId))
			{
				return (MVAvatar)worldObjects[avatarId];
			}
			return null;
		}
	}

	public MVCameraController WeCamera { get; set; }

	public FriendList Friends => friends;

	public Dictionary<int, Link> Links => links;

	public Queue<Link> PendingLinks => pendingLinkQueue;

	public Dictionary<int, GameObject> LinkObjects => linkObjects;

	public LinkGraph LinkGraph => linkGraph;

	public event EventHandler<OnWorldObjectRegisterResponseEventArgs> OnWorldObjectRegisterResponse;

	public event EventHandler<OnTransferOwnershipResponseEventArgs> OnWorldObjectTransferOwnershipResponse;

	public event EventHandler<OnRequestedPrototypeCreatedEventArgs> OnRequestedPrototypeCreated;

	public event EventHandler<OnHierarchyLockedEventArgs> OnHierarchyLockedResponse;

	public event EventHandler<OnUngroupResponseEventArgs> OnUngroupResponse;

	public MVWorldObjectClientManager()
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		playerRepository = new PlayerRepository();
		worldInventory = new MVWorldInventory();
		players = new Dictionary<int, MVPlayer>();
		friends = new FriendList();
		LinkGraph linkGraph = this.linkGraph;
		linkGraph.OnResetNode = (LinkGraph.OnResetNodeDelegate)Delegate.Combine(linkGraph.OnResetNode, new LinkGraph.OnResetNodeDelegate(LinkGraph_OnResetNode));
	}

	public void Cleanup()
	{
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			value.Destroy();
		}
		worldObjects.Clear();
	}

	private void UpdateLOD()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)WeCamera != (Object)null))
		{
			return;
		}
		Terrain.ChangeLODTerrain();
		Vector3 position = ((Component)((Component)WeCamera).camera).transform.position;
		float num = 1000f;
		int num2 = Mathf.Max(1, Mathf.RoundToInt(num * Time.deltaTime));
		for (int i = 0; i < num2; i++)
		{
			if (worldObjectsIdsLodBookkeeping.currentPosition >= worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.Count)
			{
				worldObjectsIdsLodBookkeeping.currentPosition = 0;
			}
			if (worldObjects.TryGetValue(worldObjectsIdsLodBookkeeping.worldObjectsIdsLod[worldObjectsIdsLodBookkeeping.currentPosition], out worldObjectsIdsLodBookkeeping.currentWorldObject))
			{
				worldObjectsIdsLodBookkeeping.currentWorldObject.ChangeLOD(Vector3.Distance(worldObjectsIdsLodBookkeeping.currentWorldObject.GameObject.transform.position, position));
			}
			else
			{
				worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.RemoveAt(worldObjectsIdsLodBookkeeping.currentPosition);
			}
			worldObjectsIdsLodBookkeeping.currentPosition++;
		}
	}

	public void UpdateWorldBounds(Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector3 minVector = MathFunctions.GetMinVector(bounds.min, worldBounds.min);
		Vector3 maxVector = MathFunctions.GetMaxVector(bounds.max, worldBounds.max);
		worldBounds.SetMinMax(minVector, maxVector);
	}

	public void LinkGraph_OnResetNode(int id)
	{
		worldObjects[id].ResetLogic();
	}

	public bool AddLink(Link link)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected Obj, but got Unknown
		if (links.ContainsKey(link.id))
		{
			Debug.LogError((object)("Link with id: " + link.id + " already added!"));
			return false;
		}
		if (link.inputWOID <= 0 || link.outputWOID <= 0)
		{
			Debug.LogError((object)"Attempt to add link, but link is not connected to WorldObjects");
			return false;
		}
		if (!worldObjects.ContainsKey(link.inputWOID))
		{
			Debug.LogError((object)"Attempt to add link, but input-wo not registered");
			return false;
		}
		if (!worldObjects.ContainsKey(link.outputWOID))
		{
			Debug.LogError((object)"Attempt to add link, but output-wo not registered");
			return false;
		}
		links.Add(link.id, link);
		linkGraph.AddLink(link.outputWOID, link.inputWOID);
		GameObject val = (GameObject)Object.Instantiate(Resources.Load("Prefabs/LinkObject"));
		val.GetComponentInChildren<LinkObjectScript>().linkID = link.id;
		linkObjects.Add(link.id, val);
		WorldObjects[link.outputWOID].AddOutputLink(link);
		WorldObjects[link.inputWOID].AddInputLink(link);
		return true;
	}

	public bool RemoveLink(Link link)
	{
		if (!links.ContainsKey(link.id))
		{
			Debug.LogError((object)("Attempt to remove link with id: " + link.id + ", but link not registered!"));
			return false;
		}
		links.Remove(link.id);
		linkGraph.RemoveLink(link.outputWOID, link.inputWOID);
		Object.Destroy((Object)(object)linkObjects[link.id]);
		linkObjects.Remove(link.id);
		worldObjects[link.outputWOID].RemoveOutputLink(link);
		worldObjects[link.inputWOID].RemoveInputLink(link);
		return true;
	}

	public void ResetLogicFromId(int worldObjectID)
	{
		Debug.Log((object)("ResetLogicFromId: " + worldObjectID));
		linkGraph.ResetChunk(worldObjectID);
	}

	public void AddPendingLink(Link link)
	{
		pendingLinkQueue.Enqueue(link);
	}

	public void RemovePendingLink(Link link)
	{
		if (!links.ContainsKey(link.id))
		{
			Debug.LogError((object)"Attempt to RemovePending link, but link not registered");
			return;
		}
		RemoveLink(link);
		pendingRemoveLinkQueue.Enqueue(link);
	}

	public void HandleAddLinkResponse(bool success, int linkID)
	{
		if (success)
		{
			Link link = pendingLinkQueue.Dequeue();
			link.id = linkID;
			AddLink(link);
		}
		else
		{
			pendingLinkQueue.Dequeue();
		}
	}

	public void HandleRemoveLinkResponse(bool success)
	{
		if (success)
		{
			pendingRemoveLinkQueue.Dequeue();
			return;
		}
		Link link = pendingRemoveLinkQueue.Dequeue();
		AddLink(link);
	}

	public int SelectionSetCount(HashSet<int> selectionSet)
	{
		int num = 0;
		foreach (int item in selectionSet)
		{
			num += SubtreeCount(item);
		}
		return num;
	}

	public int SubtreeCount(int id)
	{
		int num = 0;
		num++;
		if ((object)MVGameController.Instance.WOCM.GetWorldObjectClient(id).GetType() == typeof(MVGroup))
		{
			foreach (MVWorldObjectClient child in ((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(id)).Children)
			{
				num += SubtreeCount(child.Id);
			}
		}
		return num;
	}

	public void GetAllWoIds(int id, HashSet<int> ids)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
		ids.Add(worldObjectClient.Id);
		if ((object)worldObjectClient.GetType() != typeof(MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)worldObjectClient).Children)
		{
			GetAllWoIds(child.Id, ids);
		}
	}

	public int CountType(int id, Type type)
	{
		int num = 0;
		if ((object)MVGameController.Instance.WOCM.GetWorldObjectClient(id).GetType() == type)
		{
			num++;
		}
		if ((object)MVGameController.Instance.WOCM.GetWorldObjectClient(id).GetType() == typeof(MVGroup))
		{
			foreach (MVWorldObjectClient child in ((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(id)).Children)
			{
				num += CountType(child.Id, type);
			}
		}
		return num;
	}

	private void AddWorldObjectToTypeSet(MVWorldObjectClient wo)
	{
		if (!worldObjectTypeSets.ContainsKey(wo.WorldObjectType))
		{
			worldObjectTypeSets.Add(wo.WorldObjectType, new HashSet<int>());
		}
		worldObjectTypeSets[wo.WorldObjectType].Add(wo.Id);
	}

	private void RemoveWorldObjectFromTypeSet(MVWorldObjectClient wo)
	{
		if (!worldObjectTypeSets.ContainsKey(wo.WorldObjectType))
		{
			Debug.LogError((object)("RemoveFromTypeSet failed. There is no HashSet defined for type: " + wo.WorldObjectType));
			return;
		}
		worldObjectTypeSets[wo.WorldObjectType].Remove(wo.Id);
		if (worldObjectTypeSets[wo.WorldObjectType].Count <= 0)
		{
			worldObjectTypeSets.Remove(wo.WorldObjectType);
		}
	}

	public void BuildObjectHierarchies()
	{
		if (hierarchiesHasBeenCreated)
		{
			Debug.LogError((object)"BuildObjectHierarchies can only be called once!");
			return;
		}
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		foreach (MVWorldObjectClient value in WorldObjects.Values)
		{
			if (value is MVGroup)
			{
				list.Add(value);
			}
		}
		foreach (MVWorldObjectClient item in list)
		{
			MVGroup mVGroup = (MVGroup)item;
			mVGroup.BuildHierarchy(isLoadingWorld: true);
		}
		hierarchiesHasBeenCreated = true;
	}

	public List<MVWorldObjectClient> GetWorldObjectsByType(WorldObjectType type)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		if (!worldObjectTypeSets.ContainsKey(type))
		{
			return list;
		}
		foreach (int item in worldObjectTypeSets[type])
		{
			if (!worldObjects.ContainsKey(item))
			{
				Debug.LogError((object)"WorldObjectTypeSet contains id which is NOT in WorldObjects! This should never happen!");
			}
			else
			{
				list.Add(worldObjects[item]);
			}
		}
		return list;
	}

	public void Update(MVNetworkGame game)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			if (value.State != MVWorldObjectState.Destroyed)
			{
				if (value.NetworkObject != null)
				{
					value.NetworkObject.Update(game);
				}
				if (value is MVLogicObject)
				{
					((MVLogicObject)value).Update();
				}
			}
			if (value.State == MVWorldObjectState.Destroyed)
			{
				RemoveWorldObjectFromTypeSet(value);
				list.Add(value);
				value.Destroy();
				Object.Destroy((Object)(object)value.GameObject);
			}
		}
		foreach (MVWorldObjectClient item in list)
		{
			if (worldObjects.ContainsKey(item.GroupId))
			{
				((MVGroup)worldObjects[item.GroupId]).RemoveChild(item.Id);
			}
			worldObjects.Remove(item.Id);
		}
		if (updateLOD)
		{
			UpdateLOD();
		}
	}

	private MVWorldObjectClient WorldObjectFactory(WorldObjectType type, Hashtable data)
	{
		switch (type)
		{
		case WorldObjectType.Avatar:
			return new MVAvatar();
		case WorldObjectType.CubeModel:
			return new MVCubeModelInstance();
		case WorldObjectType.PointLight:
			return new MVPointLight();
		case WorldObjectType.SpawnPoint:
			return new MVSpawnPoint();
		case WorldObjectType.CubeModelPrototypeTerrain:
			return new MVCubeModelPrototypeTerrain();
		case WorldObjectType.Group:
			return new MVGroup();
		case WorldObjectType.TriggerBox:
			return new MVTriggerBox();
		case WorldObjectType.SoundEmitter:
			return new MVSoundEmitter();
		case WorldObjectType.Flag:
			return new MVFlag();
		case WorldObjectType.TestLogicCube:
			return new TestLogicCube();
		case WorldObjectType.Battery:
			return new MVBattery();
		case WorldObjectType.ToggleBox:
			return new MVToggleBox();
		case WorldObjectType.Negate:
			return new MVNegate();
		case WorldObjectType.And:
			return new MVAnd();
		case WorldObjectType.Explosives:
			return new MVExplosives();
		case WorldObjectType.TextMsg:
			return new MVTextMsg();
		case WorldObjectType.Fire:
			return new MVFire();
		case WorldObjectType.Smoke:
			return new MVSmoke();
		case WorldObjectType.TimeTrigger:
			return new MVTimeTrigger();
		case WorldObjectType.Teleporter:
			return new MVTeleporter();
		case WorldObjectType.Goal:
			return new MVGoal();
		case WorldObjectType.PickupItemHealthPack:
			return new MVPickupItemHealthPack();
		case WorldObjectType.PickupItemCenterGun:
			return new MVPickupItemCenterGun();
		case WorldObjectType.CubeModelTerrainFineGrained:
			return new MVCubeModelFineGrainedTerrain();
		case WorldObjectType.PressurePlate:
			return new MVPressurePlate();
		default:
			Debug.LogError((object)("WOCM trying to create unknown type: " + type));
			return null;
		}
	}

	public int RegisterWorldObject(WorldObjectType type, int groupId, Hashtable data, Hashtable runTimeData, int ownerActorNr, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		MVWorldObjectClient mVWorldObjectClient = WorldObjectFactory(type, data);
		mVWorldObjectClient.WorldObjectType = type;
		mVWorldObjectClient.GroupId = groupId;
		mVWorldObjectClient.Data = data;
		mVWorldObjectClient.RunTimeData = runTimeData;
		mVWorldObjectClient.RunTimeData = runTimeData;
		mVWorldObjectClient.OwnerActorNr = ownerActorNr;
		mVWorldObjectClient.Position = position;
		mVWorldObjectClient.Rotation = rotation;
		mVWorldObjectClient.Scale = scale;
		mVWorldObjectClient.State = MVWorldObjectState.Created;
		pendingRegisterQueue.Enqueue(mVWorldObjectClient);
		return 0;
	}

	public void RegisterWorldObjectResponse(int id, bool success)
	{
		if (success)
		{
			if (pendingRegisterQueue.Count <= 0)
			{
				Debug.LogError((object)"RegisterWorldObjectResponse, but no object on pendingRegisterQueue");
				return;
			}
			MVWorldObjectClient mVWorldObjectClient = pendingRegisterQueue.Dequeue();
			mVWorldObjectClient.SetId(id);
			mVWorldObjectClient.State = MVWorldObjectState.Synced;
			mVWorldObjectClient.CreateGameObject(local: true);
			AddToWorldObjects(mVWorldObjectClient);
			if (MVGameController.Instance.Game.JoinState == MVJoinState.Playing)
			{
				mVWorldObjectClient.Initialize();
			}
			if (OnWorldObjectRegisterResponse != null)
			{
				OnWorldObjectRegisterResponse(this, new OnWorldObjectRegisterResponseEventArgs(id));
			}
		}
		else
		{
			pendingRegisterQueue.Dequeue();
		}
	}

	public void RegisterWorldObjectProxy(WorldObjectType type, Hashtable data, Hashtable runTimeData, int id, int groupId, int ownerActorNr, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (worldObjects.ContainsKey(id))
		{
			Debug.LogWarning((object)"The worldObject is allready in worldObjects. This probably originates from Wait for group");
			return;
		}
		MVWorldObjectClient mVWorldObjectClient = WorldObjectFactory(type, data);
		mVWorldObjectClient.SetId(id);
		mVWorldObjectClient.SetGroupId(groupId);
		mVWorldObjectClient.WorldObjectType = type;
		mVWorldObjectClient.Data = data;
		mVWorldObjectClient.RunTimeData = runTimeData;
		mVWorldObjectClient.OwnerActorNr = ownerActorNr;
		mVWorldObjectClient.Position = position;
		mVWorldObjectClient.Rotation = rotation;
		mVWorldObjectClient.Scale = scale;
		mVWorldObjectClient.State = MVWorldObjectState.Synced;
		mVWorldObjectClient.CreateGameObject(local: false);
		AddToWorldObjects(mVWorldObjectClient);
		if (MVGameController.Instance.Game.JoinState == MVJoinState.Playing)
		{
			mVWorldObjectClient.Initialize();
		}
	}

	public bool Ungroup(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		pendingUngroupQueue.Enqueue(id);
		return true;
	}

	public bool UngroupResponse(bool success)
	{
		if (success)
		{
			if (pendingUngroupQueue.Count <= 0)
			{
				Debug.LogError((object)"UngroupResponse, but no object on pendingUngroupQueue");
				return false;
			}
			int num = pendingUngroupQueue.Dequeue();
			UngroupExecute(num);
			if (OnUngroupResponse != null)
			{
				OnUngroupResponse(this, new OnUngroupResponseEventArgs(num, success));
			}
			return true;
		}
		if (pendingUngroupQueue.Count <= 0)
		{
			Debug.LogError((object)"UngroupResponse, but no object on pendingUngroupQueue");
			return false;
		}
		int worldObjectID = pendingUngroupQueue.Dequeue();
		if (OnUngroupResponse != null)
		{
			OnUngroupResponse(this, new OnUngroupResponseEventArgs(worldObjectID, success));
		}
		return false;
	}

	public bool UngroupProxy(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (WorldObjects[id].GroupId != -1)
		{
			UngroupExecute(id);
		}
		return true;
	}

	private void UngroupExecute(int id)
	{
		int groupId = worldObjects[id].GroupId;
		ArrayList arrayList = new ArrayList(((MVGroup)worldObjects[id]).Children);
		foreach (MVWorldObjectClient item in arrayList)
		{
			((MVGroup)worldObjects[id]).RemoveChild(item.Id);
			((MVGroup)worldObjects[groupId]).AddChild(item);
			item.GroupId = groupId;
		}
		SetState(id, MVWorldObjectState.Destroyed);
		Object.Destroy((Object)(object)worldObjects[id].GameObject);
	}

	public bool UnregisterWorldObject(int id, bool forceDeletion = false)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		pendingUnregisterQueue.Enqueue(worldObjects[id]);
		return true;
	}

	public bool UnregisterWorldObjectResponse(bool success)
	{
		if (success)
		{
			if (pendingUnregisterQueue.Count <= 0)
			{
				Debug.LogError((object)"UnregisterWorldObjectResponse, but no object on pendingUnregisterQueue");
				return false;
			}
			MVWorldObjectClient mVWorldObjectClient = pendingUnregisterQueue.Dequeue();
			OnUnregisterCleanUpLinks(mVWorldObjectClient);
			SetState(mVWorldObjectClient.Id, MVWorldObjectState.Destroyed);
			Object.Destroy((Object)(object)mVWorldObjectClient.GameObject);
			return true;
		}
		pendingUnregisterQueue.Dequeue().DeleteFailed();
		return true;
	}

	public bool UnregisterWorldObjectProxy(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (WorldObjects[id].GroupId != -1)
		{
			MVGroup mVGroup = (MVGroup)WorldObjects[WorldObjects[id].GroupId];
			mVGroup.RemoveChild(id);
		}
		OnUnregisterCleanUpLinks(WorldObjects[id]);
		SetState(id, MVWorldObjectState.Destroyed);
		Object.Destroy((Object)(object)WorldObjects[id].GameObject);
		return true;
	}

	private void OnUnregisterCleanUpLinks(MVWorldObjectClient wo)
	{
		List<Link> list = new List<Link>();
		foreach (Link inputLinkRef in wo.InputLinkRefs)
		{
			list.Add(inputLinkRef);
		}
		foreach (Link outputLinkRef in wo.OutputLinkRefs)
		{
			list.Add(outputLinkRef);
		}
		foreach (Link item in list)
		{
			MVGameController.Instance.Game.RemoveLink(item);
		}
	}

	public static void SetState(int id, MVWorldObjectState state)
	{
		MVGameController.Instance.WOCM.GetWorldObjectClient(id).State = state;
		if (!(MVGameController.Instance.WOCM.GetWorldObjectClient(id) is MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(id)).Children)
		{
			SetState(child.Id, state);
		}
	}

	public bool LockHierarchyResponse(int id, bool lockObject, bool success)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (lockObject)
		{
			SetOwnerRecursively(id, localPlayerActorNumber);
		}
		else
		{
			SetOwnerRecursively(id, 0);
		}
		if (OnHierarchyLockedResponse != null)
		{
			OnHierarchyLockedResponse(this, new OnHierarchyLockedEventArgs(id, success));
		}
		return true;
	}

	public bool LockHierarchyProxy(int id, int actorNr)
	{
		Debug.Log((object)("LockHierarchyProxy " + id));
		SetOwnerRecursively(id, actorNr);
		return true;
	}

	private void SetOwnerRecursively(int id, int actorNr)
	{
		worldObjects[id].OwnerActorNr = actorNr;
		if ((object)worldObjects[id].GetType() != typeof(MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)worldObjects[id]).Children)
		{
			child.OwnerActorNr = actorNr;
		}
	}

	public bool TransferOwnershipResponse(int id, int ownerActorNr, bool success)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (success)
		{
			WorldObjects[id].OwnerActorNr = ownerActorNr;
			if (ownerActorNr == 0)
			{
				WorldObjects[id].NetworkObject = new MVNetworkListener(WorldObjects[id]);
			}
			else
			{
				WorldObjects[id].NetworkObject = new MVNetworkReporter(WorldObjects[id]);
			}
		}
		else
		{
			Debug.LogWarning((object)"Failed to set ownership...");
		}
		if (OnWorldObjectTransferOwnershipResponse != null)
		{
			OnWorldObjectTransferOwnershipResponse(this, new OnTransferOwnershipResponseEventArgs(id, ownerActorNr, success));
		}
		return true;
	}

	public bool TransferOwnershipProxy(int id, int ownerActorNr)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		WorldObjects[id].OwnerActorNr = ownerActorNr;
		if (ownerActorNr == 0)
		{
			WorldObjects[id].DeSelect();
		}
		else
		{
			WorldObjects[id].Select(Color.blue);
		}
		return true;
	}

	public void OnUpdatePrototypeEvent(int worldInventoryID, byte[] worldInventoryData)
	{
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = WorldInventory.RuntimePrototypes[worldInventoryID];
		runtimePrototypeCubeModel.UpdatePrototype(new BytePacker(worldInventoryData));
	}

	public void OnUpdatePrototypeScaleEvent(int worldInventoryID, float scale)
	{
		worldInventory.Prototypes[worldInventoryID].Scale = scale;
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = WorldInventory.RuntimePrototypes[worldInventoryID];
		runtimePrototypeCubeModel.UpdatePrototypeScale(scale);
	}

	public void OnUpdateWorldObjectDataEvent(int worldObjectID, Hashtable worldObjectData)
	{
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)"Attempt to update WorldObjectData on unknown WorldObject!");
			return;
		}
		worldObjects[worldObjectID].Data = worldObjectData;
		worldObjects[worldObjectID].OnDataUpdate();
	}

	public void OnUpdateWorldObjectRunTimeDataEvent(int worldObjectID, Hashtable delta)
	{
		logger.Log($"Received OnUpdateWorldObjectRunTimeData event. worldObjectId={worldObjectID.ToString()}");
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)"Attempt to updata WorldObjectRunTimeData on unknown WorldObject!");
			return;
		}
		MVWorldObjectClient mVWorldObjectClient = worldObjects[worldObjectID];
		mVWorldObjectClient.RunTimeDataUpdate(delta);
	}

	public void OnUpdateTerrainEvent(int worldObjectID, byte[] terrainData)
	{
	}

	public void DebugLogWorldObjectList()
	{
		Debug.Log((object)"----------");
		Debug.Log((object)"WORLD OBJECT LIST:");
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			Debug.Log((object)("WO Type: " + value.WorldObjectType.ToString() + ", Id: " + value.Id + ", Owner: " + value.OwnerActorNr));
		}
		Debug.Log((object)"----------");
	}

	public void AddPrototype(int id, int itemID, int typeID, string name, Hashtable data, float scale, int actorNrInstigator)
	{
		worldInventory.AddPrototype(id, itemID, typeID, name, data, scale, actorNrInstigator);
		if (actorNrInstigator == localPlayerActorNumber && OnRequestedPrototypeCreated != null)
		{
			OnRequestedPrototypeCreated(this, new OnRequestedPrototypeCreatedEventArgs(id));
		}
	}

	public void RemovePrototype(int id)
	{
		worldInventory.RemovePrototype(id);
	}

	public MVWorldObjectClient GetWorldObjectClient(int id)
	{
		if (worldObjects.ContainsKey(id))
		{
			return worldObjects[id];
		}
		return null;
	}

	public MVWorldObject GetWorldObject(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return null;
		}
		return worldObjects[id];
	}

	public MVWorldObjectClient GetWorldObjectGoId(int goId)
	{
		if (gameObjectIdToWorldObjectIdMap.ContainsKey(goId))
		{
			return worldObjects[gameObjectIdToWorldObjectIdMap[goId]];
		}
		return null;
	}

	public bool GoIdContains(int goId)
	{
		return gameObjectIdToWorldObjectIdMap.ContainsKey(goId);
	}

	public static MVWorldObjectClient GetMVObject(Transform t)
	{
		if (MVGameController.Instance.WOCM.GoIdContains(((Object)((Component)t).gameObject).GetInstanceID()))
		{
			return MVGameController.Instance.WOCM.GetWorldObjectGoId(((Object)((Component)t).gameObject).GetInstanceID());
		}
		if ((Object)(object)t.parent != (Object)null)
		{
			return GetMVObject(t.parent);
		}
		return null;
	}

	public bool Pick(ref VoxelHit hit)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		float num = 0f;
		bool flag = false;
		if (MVGameController.Instance.EditorController.IsDrawPlaneActive())
		{
			Vector3 hit2 = Vector3.zero;
			if (MVGameController.Instance.EditorController.WorldEditorDrawPlane.Pick(ref hit2))
			{
				Vector3 val = hit2 - ray.origin;
				num = val.magnitude;
				flag = true;
			}
		}
		List<VoxelHit> list = CollisionDetection.MVHitAll(ray);
		if (list.Count == 0)
		{
			return false;
		}
		float num2 = float.PositiveInfinity;
		bool flag2 = false;
		foreach (VoxelHit item in list)
		{
			if ((!(item.distance < num) && flag) || !((Component)item.transform).gameObject.active)
			{
				continue;
			}
			Renderer componentInChildren = ((Component)item.transform).gameObject.GetComponentInChildren<Renderer>();
			if ((Object)(object)componentInChildren != (Object)null && componentInChildren.enabled && (((Component)item.transform).gameObject.layer != LayerMask.NameToLayer("Logic") || MVGameController.Instance.EditorController.IsLogicRendered()))
			{
				float num3 = Vector3.Distance(ray.origin, item.point);
				if (num3 < num2)
				{
					num2 = num3;
					hit = item;
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
		}
		return flag2;
	}

	public MVSpawnPoint GetValidSpawnPoint()
	{
		List<MVWorldObjectClient> worldObjectsByType = GetWorldObjectsByType(WorldObjectType.SpawnPoint);
		if (worldObjectsByType.Count > 0)
		{
			int index = Random.Range(0, worldObjectsByType.Count);
			return (MVSpawnPoint)worldObjectsByType[index];
		}
		Debug.LogError((object)"No valid SpawnPoint on planet...");
		return null;
	}

	public Vector3 GetValidAvatarStartPosition(int tries = 0)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (Terrain == null)
		{
			Debug.LogError((object)"No terrain found");
			BytePacker bytePacker = new BytePacker();
			bytePacker.Write(1);
			bytePacker.Write((short)0);
			bytePacker.Write((short)0);
			bytePacker.Write((short)0);
			bytePacker.Write((byte)3);
			bytePacker.Write((byte)1);
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)121] = bytePacker.ToArray();
			return Vector3.zero;
		}
		Vector3 randomCubePos = Terrain.GetRandomCubePos();
		randomCubePos += Vector3.up * 100f;
		if (CollisionDetection.MVHit(new Ray(randomCubePos, Vector3.down), out var voxelHit))
		{
			randomCubePos = voxelHit.point + Vector3.up * 1f;
		}
		else if (tries < 10)
		{
			return GetValidAvatarStartPosition(tries++);
		}
		return randomCubePos;
	}

	public void PreparePlayMode()
	{
	}

	public void EndPlayMode()
	{
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			if (!value.Visible)
			{
				value.Visible = true;
			}
		}
	}

	private void AddToWorldObjects(MVWorldObjectClient wo)
	{
		if (gameObjectIdToWorldObjectIdMap.ContainsKey(((Object)wo.GameObject).GetInstanceID()))
		{
			Debug.LogError((object)"Key already in gameObjectsWorldObjectsMap");
		}
		else
		{
			gameObjectIdToWorldObjectIdMap.Add(((Object)wo.GameObject).GetInstanceID(), wo.Id);
		}
		if (worldObjects.ContainsKey(wo.Id))
		{
			Debug.LogError((object)"Key already in WorldObjects dictionary");
			return;
		}
		worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.Add(wo.Id);
		worldObjects.Add(wo.Id, wo);
		AddWorldObjectToTypeSet(wo);
	}
}
