using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public abstract class MVWorldObjectClientManager
{
	protected class WorldObjectMapping
	{
		private readonly Dictionary<WorldObjectType, HashSet<int>> worldObjectTypeSets = new Dictionary<WorldObjectType, HashSet<int>>();

		private readonly Dictionary<int, int> gameObjectIdToWorldObjectIdMap = new Dictionary<int, int>();

		private readonly Dictionary<Type, WorldObjectType> typeWorldObjectTypeMap = new Dictionary<Type, WorldObjectType>();

		public void AddWorldObjectToTypeSet(MVWorldObjectClient wo)
		{
			if (!worldObjectTypeSets.ContainsKey(wo.WorldObjectType))
			{
				worldObjectTypeSets.Add(wo.WorldObjectType, new HashSet<int>());
				AddWorldObjectToTypeWorldObjectTypeMap(wo.GetType(), wo.WorldObjectType);
			}
			worldObjectTypeSets[wo.WorldObjectType].Add(wo.Id);
			AddToGameObjectIDMap(wo);
		}

		public HashSet<int> GetWorldObjectTypeSet(WorldObjectType worldObjectType)
		{
			if (!worldObjectTypeSets.ContainsKey(worldObjectType))
			{
				return null;
			}
			return worldObjectTypeSets[worldObjectType];
		}

		public void RemoveWorldObjectFromTypeSet(MVWorldObjectClient wo)
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
				RemoveWorldObjectFromTypeWorldObjectTypeMap(wo.GetType());
			}
			gameObjectIdToWorldObjectIdMap.Remove(((Object)wo.GameObject).GetInstanceID());
		}

		public bool TryGetWorldObjectTypeFromObjectType(Type type, ref WorldObjectType worldObjectType)
		{
			if (!typeWorldObjectTypeMap.ContainsKey(type))
			{
				Debug.LogWarning((object)"Could not get worldObject type from type");
				return false;
			}
			worldObjectType = typeWorldObjectTypeMap[type];
			return true;
		}

		public bool TryGetWorldObjectIDFromGameObjectID(int goId, out int woID)
		{
			return gameObjectIdToWorldObjectIdMap.TryGetValue(goId, out woID);
		}

		private void AddToGameObjectIDMap(MVWorldObjectClient wo)
		{
			if (gameObjectIdToWorldObjectIdMap.ContainsKey(((Object)wo.GameObject).GetInstanceID()))
			{
				Debug.LogError((object)"Key already in gameObjectsWorldObjectsMap");
			}
			else
			{
				gameObjectIdToWorldObjectIdMap.Add(((Object)wo.GameObject).GetInstanceID(), wo.Id);
			}
		}

		private void AddWorldObjectToTypeWorldObjectTypeMap(Type type, WorldObjectType worldObjectType)
		{
			if (typeWorldObjectTypeMap.ContainsKey(type))
			{
				Debug.LogError((object)"Type already contained in typeWorldObjectTypeMap");
			}
			else
			{
				typeWorldObjectTypeMap.Add(type, worldObjectType);
			}
		}

		private void RemoveWorldObjectFromTypeWorldObjectTypeMap(Type type)
		{
			if (!typeWorldObjectTypeMap.ContainsKey(type))
			{
				Debug.LogError((object)"Type not contained in typeWorldObjectTypeMap");
			}
			else
			{
				typeWorldObjectTypeMap.Remove(type);
			}
		}
	}

	protected class WorldObjectLOD
	{
		private struct WorldObjectsIdsLodBookkeeping(List<int> worldObjectsIdsLod)
		{
			public int currentPosition = 0;

			public List<int> worldObjectsIdsLod = worldObjectsIdsLod;

			public MVWorldObjectClient currentWorldObject = null;
		}

		private WorldObjectsIdsLodBookkeeping worldObjectsIdsLodBookkeeping = new WorldObjectsIdsLodBookkeeping(new List<int>());

		private MVWorldObjectClientManager worldObjectClientManager;

		public WorldObjectLOD(MVWorldObjectClientManager worldObjectClientManager)
		{
			this.worldObjectClientManager = worldObjectClientManager;
		}

		public void AddWorldObjectToLOD(int woID)
		{
			worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.Add(woID);
		}

		public void UpdateLOD()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)MVGameController.Instance.Game.CameraController != (Object)null))
			{
				return;
			}
			worldObjectClientManager.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().ChangeLODTerrain();
			Vector3 position = ((Component)((Component)MVGameController.Instance.Game.CameraController).camera).transform.position;
			float num = 1000f;
			int num2 = Mathf.Max(1, Mathf.RoundToInt(num * Time.deltaTime));
			for (int i = 0; i < num2; i++)
			{
				if (worldObjectsIdsLodBookkeeping.currentPosition >= worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.Count)
				{
					worldObjectsIdsLodBookkeeping.currentPosition = 0;
				}
				if (worldObjectClientManager.worldObjects.TryGetValue(worldObjectsIdsLodBookkeeping.worldObjectsIdsLod[worldObjectsIdsLodBookkeeping.currentPosition], out worldObjectsIdsLodBookkeeping.currentWorldObject))
				{
					worldObjectsIdsLodBookkeeping.currentWorldObject.ChangeLOD(Vector3.Distance(worldObjectsIdsLodBookkeeping.currentWorldObject.WorldPosition, position));
				}
				else
				{
					worldObjectsIdsLodBookkeeping.worldObjectsIdsLod.RemoveAt(worldObjectsIdsLodBookkeeping.currentPosition);
				}
				worldObjectsIdsLodBookkeeping.currentPosition++;
			}
		}
	}

	protected readonly Dictionary<int, MVWorldObjectClient> worldObjects = new Dictionary<int, MVWorldObjectClient>();

	protected readonly Queue<int> pendingUngroupQueue = new Queue<int>();

	protected readonly WorldObjectLOD worldObjectLOD;

	protected readonly WorldObjectMapping worldObjectMapping;

	protected Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>> woDestroyedEventSubscribers = new Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>>();

	protected Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>> woCreatedEventSubscribers = new Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>>();

	private int rootGroupId = -1;

	private SharedWorldObjectGameplayFunctions sharedWorldObjectGameplayFunctions;

	private Bounds worldBounds = default;

	public EventHandler<OnTransferOwnershipResponseEventArgs> OnWorldObjectTransferOwnershipResponse;

	public EventHandler<OnRequestedPrototypeCreatedEventArgs> OnRequestedPrototypeCreated;

	public EventHandler<OnHierarchyLockedEventArgs> OnHierarchyLockedResponse;

	public EventHandler<OnUngroupResponseEventArgs> OnUngroupResponse;

	public EventHandler<OnTransferWosResponseEventArgs> OnTransferWosResponse;

	public EventHandler<CloneWorldObjectTreeResponseEventArgs> CloneWorldObjectTreeResponse;

	public MoveableController MoveableController { get; private set; }

	public Bounds WorldBounds
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return worldBounds;
		}
	}

	public MVAvatarLocal AvatarLocal { get; set; }

	public WaterPlaneManager WaterPlaneManager { get; set; }

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

	public SharedWorldObjectGameplayFunctions SharedWorldObjectGameplayFunctions => sharedWorldObjectGameplayFunctions;

	public MVWorldObjectClientManager()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		MoveableController = new MoveableController();
		sharedWorldObjectGameplayFunctions = new SharedWorldObjectGameplayFunctions();
		worldObjectLOD = new WorldObjectLOD(this);
		worldObjectMapping = new WorldObjectMapping();
	}

	public bool Contains(int woID)
	{
		return worldObjects.ContainsKey(woID);
	}

	public bool IsType(int woID, WorldObjectType worldObjectType)
	{
		if (!worldObjects.ContainsKey(woID))
		{
			Debug.LogWarning((object)"Failed to get worldObject");
			return false;
		}
		return worldObjects[woID].WorldObjectType == worldObjectType;
	}

	public void GetAllWoIds(int id, HashSet<int> ids)
	{
		MVWorldObjectClient worldObjectClient = GetWorldObjectClient(id);
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

	public List<MVWorldObjectClient> GetWorldObjectsByType(WorldObjectType type)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		HashSet<int> worldObjectTypeSet = worldObjectMapping.GetWorldObjectTypeSet(type);
		if (worldObjectTypeSet == null)
		{
			return list;
		}
		foreach (int item in worldObjectTypeSet)
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

	public int GetWoIDHighestInHierarchyWithComponent<T>(int woId) where T : Component
	{
		int result = -1;
		do
		{
			MVWorldObjectClient worldObjectClient = GetWorldObjectClient(woId);
			if ((Object)(object)worldObjectClient.GameObject.GetComponent<T>() != (Object)null)
			{
				result = worldObjectClient.Id;
			}
			woId = worldObjectClient.GroupId;
		}
		while (woId != -1);
		return result;
	}

	public T GetSingletonWorldObject<T>() where T : MVWorldObjectClient
	{
		WorldObjectType worldObjectType = WorldObjectType.Battery;
		if (!worldObjectMapping.TryGetWorldObjectTypeFromObjectType(typeof(T), ref worldObjectType))
		{
			return (T)null;
		}
		MVWorldObjectClient singletonWorldObjectByType = GetSingletonWorldObjectByType(worldObjectType);
		if (singletonWorldObjectByType == null)
		{
			return (T)null;
		}
		return (T)singletonWorldObjectByType;
	}

	private MVWorldObjectClient GetSingletonWorldObjectByType(WorldObjectType worldObjectType)
	{
		List<MVWorldObjectClient> worldObjectsByType = GetWorldObjectsByType(worldObjectType);
		if (worldObjectsByType.Count > 1)
		{
			Debug.LogError((object)$"WorldObjectType {worldObjectType} is not a singleton object. Count is {worldObjectsByType.Count}: ");
			return null;
		}
		if (worldObjectsByType.Count == 0)
		{
			Debug.LogWarning((object)$"Singleton of worldObject of type: {worldObjectType} not found");
			return null;
		}
		return worldObjectsByType[0];
	}

	public static T GetEnabledMonoBehaviourHighestInHierarchy<T>(GameObject gameObject) where T : MonoBehaviour
	{
		T component = gameObject.GetComponent<T>();
		if ((Object)(object)component != (Object)null && ((Behaviour)component).enabled)
		{
			return component;
		}
		if ((Object)(object)gameObject.transform.parent == (Object)null)
		{
			return (T)(object)null;
		}
		return GetEnabledMonoBehaviourHighestInHierarchy<T>(((Component)gameObject.transform.parent).gameObject);
	}

	public int GetWoIDWithLocalOwnerHighestInHierarchy(int woID)
	{
		int result = -1;
		int actorNr = MVGameController.Instance.Game.LocalPlayer.ActorNr;
		do
		{
			MVWorldObjectClient worldObjectClient = GetWorldObjectClient(woID);
			if (worldObjectClient.OwnerActorNr == actorNr)
			{
				result = worldObjectClient.Id;
			}
			woID = worldObjectClient.GroupId;
		}
		while (woID != -1);
		return result;
	}

	public List<MVWorldObjectClient> GetBlueprintWorldObjectsByType(Type type)
	{
		WorldObjectType worldObjectType = WorldObjectType.Blueprint;
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		HashSet<int> worldObjectTypeSet = worldObjectMapping.GetWorldObjectTypeSet(worldObjectType);
		if (worldObjectTypeSet == null)
		{
			return list;
		}
		foreach (int item in worldObjectTypeSet)
		{
			if (!worldObjects.ContainsKey(item))
			{
				Debug.LogError((object)"WorldObjectTypeSet contains id which is NOT in WorldObjects! This should never happen!");
			}
			else if (type.Equals(worldObjects[item].GetType()))
			{
				list.Add(worldObjects[item]);
			}
		}
		return list;
	}

	public bool GetUnmodifiedWorldObject(KoGaMaPackageClient koGaMaPackageClient, ref int worldObjectId)
	{
		MVWorldObjectClient mVWorldObjectClient = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			if (value.GroupId == rootGroupId && value.ItemId == mVWorldObjectClient.ItemId && value.WorldObjectType == mVWorldObjectClient.WorldObjectType)
			{
				int insertedByProfileId = 0;
				if (value.CompareWithKoGaMaPackage(mVWorldObjectClient, koGaMaPackageClient, ref insertedByProfileId) && insertedByProfileId == MVGameController.Instance.Game.LocalPlayer.ProfileID)
				{
					Debug.Log((object)("InsertedByProfileID " + insertedByProfileId));
					worldObjectId = value.Id;
					return true;
				}
			}
		}
		return false;
	}

	public MVWorldObjectClient GetWorldObjectClient(int id)
	{
		worldObjects.TryGetValue(id, out var value);
		return value;
	}

	public MVWorldObjectClient GetWorldObjectClientWhere(Func<MVWorldObjectClient, bool> predicate)
	{
		return worldObjects.Values.FirstOrDefault(predicate);
	}

	public IEnumerable<MVWorldObjectClient> GetWorldObjectClientsWhere(Func<MVWorldObjectClient, bool> predicate)
	{
		return worldObjects.Values.Where(predicate);
	}

	public MVWorldObjectClient GetWorldObjectByGoId(int goId)
	{
		if (worldObjectMapping.TryGetWorldObjectIDFromGameObjectID(goId, out var woID))
		{
			return worldObjects[woID];
		}
		return null;
	}

	public static MVWorldObjectClient GetMVObject(Transform t)
	{
		MVWorldObjectClient worldObjectByGoId = MVGameController.Instance.WOCM.GetWorldObjectByGoId(((Object)((Component)t).gameObject).GetInstanceID());
		if (worldObjectByGoId != null)
		{
			return worldObjectByGoId;
		}
		if ((Object)(object)t.parent != (Object)null)
		{
			return GetMVObject(t.parent);
		}
		return null;
	}

	public bool Pick(ref VoxelHit hit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		float num = 0f;
		bool flag = false;
		if (MVGameController.Instance.EditController != null && MVGameController.Instance.EditController.IsDrawPlaneActive)
		{
			Vector3 hit2 = Vector3.zero;
			if (MVGameController.Instance.EditController.WorldEditorDrawPlane.Pick(ref hit2))
			{
				Vector3 val = hit2 - ray.origin;
				num = val.magnitude;
				flag = true;
			}
		}
		List<VoxelHit> list = CollisionDetection.MVHitAll(ray, float.PositiveInfinity, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		float num2 = float.PositiveInfinity;
		bool result = false;
		foreach (VoxelHit item in list)
		{
			if ((item.distance < num || !flag) && ((Component)item.transform).gameObject.active && (((Component)item.transform).gameObject.layer != LayerMask.NameToLayer("Logic") || MVGameController.Instance.EditController.IsLogicRendered() || IsHitPickup(item)))
			{
				float num3 = Vector3.Distance(ray.origin, item.point);
				if (num3 < num2)
				{
					num2 = num3;
					hit = item;
					result = true;
				}
			}
		}
		return result;
	}

	private bool IsHitPickup(VoxelHit hit)
	{
		Transform parent = hit.transform.parent;
		return (Object)(object)((Component)parent).GetComponent<PickupItemObjectScript>() != (Object)null;
	}

	public MVSpawnPoint GetValidSpawnPoint()
	{
		List<MVWorldObjectClient> list = ((MVGameController.Instance.Game.TeamManager.TeamCount() <= 1) ? GetWorldObjectsByType(GetSpawnPointTypeForNoneTeam()) : GetWorldObjectsByType(GetSpawnPointTypeForTeam(MVGameController.Instance.Game.LocalPlayer.Team)));
		if (list.Count > 0)
		{
			int index = Random.Range(0, list.Count);
			return (MVSpawnPoint)list[index];
		}
		Debug.LogError((object)"No valid SpawnPoint on planet...");
		return null;
	}

	private WorldObjectType GetSpawnPointTypeForTeam(MVTeam team)
	{
		return team switch
		{
			MVTeam.Blue => WorldObjectType.SpawnPointBlue, 
			MVTeam.Red => WorldObjectType.SpawnPointRed, 
			MVTeam.Green => WorldObjectType.SpawnPointGreen, 
			MVTeam.Yellow => WorldObjectType.SpawnPointYellow, 
			_ => GetSpawnPointTypeForNoneTeam(), 
		};
	}

	private WorldObjectType GetSpawnPointTypeForNoneTeam()
	{
		if (GetWorldObjectsByType(WorldObjectType.SpawnPointBlue).Count > 0)
		{
			return WorldObjectType.SpawnPointBlue;
		}
		if (GetWorldObjectsByType(WorldObjectType.SpawnPointRed).Count > 0)
		{
			return WorldObjectType.SpawnPointRed;
		}
		if (GetWorldObjectsByType(WorldObjectType.SpawnPointGreen).Count > 0)
		{
			return WorldObjectType.SpawnPointGreen;
		}
		if (GetWorldObjectsByType(WorldObjectType.SpawnPointYellow).Count > 0)
		{
			return WorldObjectType.SpawnPointYellow;
		}
		return WorldObjectType.SpawnPoint;
	}

	public void SubscribeWODestroyedEvent(int woID, Action<object, WorldObjectDestroyedEventArgs> woDestroyedEventHandler)
	{
		if (worldObjects.TryGetValue(woID, out var _))
		{
			if (woDestroyedEventSubscribers.TryGetValue(woID, out var _))
			{
				Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>> dictionary2;
				Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>> dictionary = (dictionary2 = woDestroyedEventSubscribers);
				int key2;
				int key = (key2 = woID);
				Action<object, WorldObjectDestroyedEventArgs> a = dictionary2[key2];
				dictionary[key] = (Action<object, WorldObjectDestroyedEventArgs>)Delegate.Combine(a, woDestroyedEventHandler);
			}
			else
			{
				woDestroyedEventSubscribers[woID] = woDestroyedEventHandler;
			}
		}
	}

	public void UnsubscribeWODestroyedEvent(int woID, Action<object, WorldObjectDestroyedEventArgs> woDestroyedEventHandler)
	{
		if (woDestroyedEventSubscribers.TryGetValue(woID, out var value))
		{
			value = (Action<object, WorldObjectDestroyedEventArgs>)Delegate.Remove(value, woDestroyedEventHandler);
		}
	}

	public void SubscribeWOCreatedEvent(Type type, Action<object, WorldObjectCreatedEventArgs> woCreatedEventHandler)
	{
		if (woCreatedEventSubscribers.TryGetValue(type, out var _))
		{
			Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>> dictionary2;
			Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>> dictionary = (dictionary2 = woCreatedEventSubscribers);
			Type key2;
			Type key = (key2 = type);
			Action<object, WorldObjectCreatedEventArgs> a = dictionary2[key2];
			dictionary[key] = (Action<object, WorldObjectCreatedEventArgs>)Delegate.Combine(a, woCreatedEventHandler);
		}
		else
		{
			woCreatedEventSubscribers[type] = woCreatedEventHandler;
		}
	}

	public void UnsubscribeWOCreatedEvent(Type type, Action<object, WorldObjectCreatedEventArgs> woCreatedEventHandler)
	{
		if (woCreatedEventSubscribers.TryGetValue(type, out var value))
		{
			value = (Action<object, WorldObjectCreatedEventArgs>)Delegate.Remove(value, woCreatedEventHandler);
		}
	}

	public bool UnregisterWorldObject(int worldObjectId)
	{
		if (!worldObjects.ContainsKey(worldObjectId))
		{
			Debug.LogWarning((object)"trying to unregister none existing worldobject");
			return false;
		}
		MVGameController.Instance.Game.UnregisterWorldObject(worldObjectId);
		return true;
	}

	public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
	{
		MVGameController.Instance.Game.CloneWorldObjectTree(root, localOwner, setAsPreviewItem, cloneToRootGroup);
	}
}
