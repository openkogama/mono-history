using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public abstract class MVWorldObjectClientManager : IWorldObjectManager
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
			if (wo == null)
			{
				Debug.LogError("RemoveFromTypeSet failed. World object is null. This is most likely caused by non recursive delete serverside");
				return;
			}
			if (!worldObjectTypeSets.ContainsKey(wo.WorldObjectType))
			{
				Debug.LogError("RemoveFromTypeSet failed. There is no HashSet defined for type: " + wo.WorldObjectType);
				return;
			}
			if (!worldObjectTypeSets[wo.WorldObjectType].Contains(wo.Id))
			{
				Debug.LogWarning("Could not find wo for type " + wo.WorldObjectType);
			}
			worldObjectTypeSets[wo.WorldObjectType].Remove(wo.Id);
			if (worldObjectTypeSets[wo.WorldObjectType].Count <= 0)
			{
				worldObjectTypeSets.Remove(wo.WorldObjectType);
				RemoveWorldObjectFromTypeWorldObjectTypeMap(wo.GetType());
			}
			gameObjectIdToWorldObjectIdMap.Remove(wo.GameObject.GetInstanceID());
		}

		public bool TryGetWorldObjectTypeFromObjectType(Type type, ref WorldObjectType worldObjectType)
		{
			if (!typeWorldObjectTypeMap.ContainsKey(type))
			{
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
			if (gameObjectIdToWorldObjectIdMap.ContainsKey(wo.GameObject.GetInstanceID()))
			{
				Debug.LogError("Key already in gameObjectsWorldObjectsMap");
			}
			else
			{
				gameObjectIdToWorldObjectIdMap.Add(wo.GameObject.GetInstanceID(), wo.Id);
			}
		}

		private void AddWorldObjectToTypeWorldObjectTypeMap(Type type, WorldObjectType worldObjectType)
		{
			if (typeWorldObjectTypeMap.ContainsKey(type))
			{
				Debug.LogError("Type already contained in typeWorldObjectTypeMap");
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
				Debug.LogError("Type not contained in typeWorldObjectTypeMap");
			}
			else
			{
				typeWorldObjectTypeMap.Remove(type);
			}
		}
	}

	private class WOCMWorldObjectClientRef<T> : WorldObjectClientRef<T> where T : MVWorldObjectClient
	{
		public WOCMWorldObjectClientRef(int woId)
			: base(woId)
		{
		}
	}

	private class WOCMWorldObjectClientRef : WorldObjectClientRef
	{
		public WOCMWorldObjectClientRef(int woId)
			: base(woId)
		{
		}
	}

	protected readonly Dictionary<int, MVWorldObjectClient> worldObjects = new Dictionary<int, MVWorldObjectClient>();

	protected readonly Queue<int> pendingUngroupQueue = new Queue<int>();

	protected readonly WorldObjectMapping worldObjectMapping;

	protected Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>> woDestroyedEventSubscribers = new Dictionary<int, Action<object, WorldObjectDestroyedEventArgs>>();

	protected Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>> woCreatedEventSubscribers = new Dictionary<Type, Action<object, WorldObjectCreatedEventArgs>>();

	private int rootGroupId = -1;

	private Bounds worldBounds = default;

	public EventHandler<OnTransferOwnershipResponseEventArgs> OnWorldObjectTransferOwnershipResponse;

	public EventHandler<OnRequestedPrototypeCreatedEventArgs> OnRequestedPrototypeCreated;

	public EventHandler<OnHierarchyLockedEventArgs> OnHierarchyLockedResponse;

	public EventHandler<OnUngroupResponseEventArgs> OnUngroupResponse;

	public EventHandler<OnTransferWosResponseEventArgs> OnTransferWosResponse;

	public EventHandler<CloneWorldObjectTreeResponseEventArgs> CloneWorldObjectTreeResponse;

	public EventHandler<EventArgs> OnResetWorldDone;

	public MoveableController MoveableController { get; private set; }

	public Bounds WorldBounds => worldBounds;

	public MVAvatarLocal AvatarLocal { get; set; }

	public int Count => worldObjects.Count;

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

	public MVWorldObjectClientManager()
	{
		MoveableController = new MoveableController();
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
			Debug.LogWarning("Failed to get worldObject");
			return false;
		}
		return worldObjects[woID].WorldObjectType == worldObjectType;
	}

	public void GetAllWoIds(int id, HashSet<int> ids)
	{
		MVWorldObjectClient worldObjectClient = GetWorldObjectClient(id);
		ids.Add(worldObjectClient.Id);
		if (worldObjectClient.GetType() != typeof(MVGroup))
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
				Debug.LogError("WorldObjectTypeSet contains id which is NOT in WorldObjects! This should never happen!");
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
			if (worldObjectClient.GameObject.GetComponent<T>() != null)
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

	public WorldObjectClientRef<T> GetSingletonWorldObjectRef<T>() where T : MVWorldObjectClient
	{
		WorldObjectType worldObjectType = WorldObjectType.Battery;
		if (!worldObjectMapping.TryGetWorldObjectTypeFromObjectType(typeof(T), ref worldObjectType))
		{
			return null;
		}
		MVWorldObjectClient singletonWorldObjectByType = GetSingletonWorldObjectByType(worldObjectType);
		if (singletonWorldObjectByType == null)
		{
			return null;
		}
		return new WOCMWorldObjectClientRef<T>(singletonWorldObjectByType.Id);
	}

	private MVWorldObjectClient GetSingletonWorldObjectByType(WorldObjectType worldObjectType)
	{
		List<MVWorldObjectClient> worldObjectsByType = GetWorldObjectsByType(worldObjectType);
		if (worldObjectsByType.Count > 1)
		{
			Debug.LogError($"WorldObjectType {worldObjectType} is not a singleton object. Count is {worldObjectsByType.Count}: ");
			return null;
		}
		if (worldObjectsByType.Count == 0)
		{
			Debug.LogWarning($"Singleton of worldObject of type: {worldObjectType} not found");
			return null;
		}
		return worldObjectsByType[0];
	}

	public static T GetEnabledMonoBehaviourHighestInHierarchy<T>(GameObject gameObject) where T : MonoBehaviour
	{
		T component = gameObject.GetComponent<T>();
		if (component != null && component.enabled)
		{
			return component;
		}
		if (gameObject.transform.parent == null)
		{
			return (T)null;
		}
		return GetEnabledMonoBehaviourHighestInHierarchy<T>(gameObject.transform.parent.gameObject);
	}

	public int GetWoIDWithLocalOwnerHighestInHierarchy(int woID)
	{
		int result = -1;
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
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
				Debug.LogError("WorldObjectTypeSet contains id which is NOT in WorldObjects! This should never happen!");
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
				if (value.CompareWithKoGaMaPackage(mVWorldObjectClient, koGaMaPackageClient, ref insertedByProfileId) && insertedByProfileId == MVGameControllerBase.Game.LocalPlayer.ProfileID)
				{
					Debug.Log("InsertedByProfileID " + insertedByProfileId);
					worldObjectId = value.Id;
					return true;
				}
			}
		}
		return false;
	}

	public MVWorldObjectClient GetWorldObjectClient(int id)
	{
		MVWorldObjectClient value = null;
		worldObjects.TryGetValue(id, out value);
		return value;
	}

	public T GetWorldObjectClient<T>(int id) where T : MVWorldObjectClient
	{
		MVWorldObjectClient value = null;
		worldObjects.TryGetValue(id, out value);
		return (T)value;
	}

	public WorldObjectClientRef<T> GetWorldObjectClientRef<T>(int id) where T : MVWorldObjectClient
	{
		return new WOCMWorldObjectClientRef<T>(id);
	}

	public static WorldObjectClientRef<MVWorldObjectClient> GetWorldObjectClientRefNullRefTyped()
	{
		return new WOCMWorldObjectClientRef<MVWorldObjectClient>(-1);
	}

	public WorldObjectClientRef GetWorldObjectClientRef(int id)
	{
		return new WOCMWorldObjectClientRef(id);
	}

	public static WorldObjectClientRef GetWorldObjectClientRefNullRef()
	{
		return new WOCMWorldObjectClientRef(-1);
	}

	public MVWorldObject GetWorldObject(int id)
	{
		return GetWorldObjectClient(id);
	}

	public bool TryGetWorldObject(int id, out MVWorldObject worldObject)
	{
		bool result = worldObjects.TryGetValue(id, out var value);
		worldObject = value;
		return result;
	}

	public MVWorldObjectClient GetWorldObjectClientRoot(int id)
	{
		MVWorldObjectClient worldObjectClient = GetWorldObjectClient(id);
		if (worldObjectClient.GroupId == -1)
		{
			Debug.LogError("Group id is -1. This is the world root");
			return null;
		}
		if (worldObjectClient.GroupId == rootGroupId)
		{
			return worldObjectClient;
		}
		return GetWorldObjectClientRoot(worldObjectClient.GroupId);
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
		MVWorldObjectClient worldObjectByGoId = MVGameControllerBase.WOCM.GetWorldObjectByGoId(t.gameObject.GetInstanceID());
		if (worldObjectByGoId != null)
		{
			return worldObjectByGoId;
		}
		if (t.parent != null)
		{
			return GetMVObject(t.parent);
		}
		return null;
	}

	public MVSpawnPoint GetValidSpawnPoint()
	{
		List<MVWorldObjectClient> list = ((MVGameControllerBase.Game.TeamManager.TeamCount() <= 1) ? GetWorldObjectsByType(GetSpawnPointTypeForNoneTeam()) : GetWorldObjectsByType(GetSpawnPointTypeForTeam(MVGameControllerBase.Game.LocalPlayer.Team)));
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return (MVSpawnPoint)list[index];
		}
		Debug.LogError("No valid SpawnPoint on planet...");
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
		if (!woDestroyedEventSubscribers.TryGetValue(woID, out var value))
		{
			return;
		}
		if (value == null)
		{
			woDestroyedEventSubscribers.Remove(woID);
			return;
		}
		value = (Action<object, WorldObjectDestroyedEventArgs>)Delegate.Remove(value, woDestroyedEventHandler);
		if (value != null)
		{
			woDestroyedEventSubscribers[woID] = value;
		}
		else
		{
			woDestroyedEventSubscribers.Remove(woID);
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
			Debug.LogWarning("trying to unregister none existing worldobject");
			return false;
		}
		MVGameControllerBase.OperationRequests.UnregisterWorldObject(worldObjectId);
		return true;
	}

	public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
	{
		MVGameControllerBase.OperationRequests.CloneWorldObjectTree(root, localOwner, setAsPreviewItem, cloneToRootGroup);
	}
}
