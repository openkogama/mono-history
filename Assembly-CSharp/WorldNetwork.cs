using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class WorldNetwork : World
{
	private Links links;

	private ObjectLinks objectLinks;

	public MVWorldObjectClientManagerNetwork WorldObjectClientManagerNetwork => worldObjectClientManager;

	public RuntimeEventManagerNetwork RuntimeEventManagerNetwork => runtimeEventManagerNetwork;

	public WorldNetwork()
	{
		worldInventory = new MVWorldInventory();
		links = new Links(OnResetNode);
		worldObjectClientManager = new MVWorldObjectClientManagerNetwork();
		objectLinks = new ObjectLinks();
	}

	public void Update(MVNetworkGame game)
	{
		worldObjectClientManager.Update(game);
		links.Update();
		objectLinks.Update();
	}

	public void CreateGameWorldFromQueryData(BytePacker queryData, int instigatorActorNumber)
	{
		MVWorldObjectClient root = InitializeQueryData(queryData);
		ConstructRuntimeEventManager();
		DeserializeRuntimeEvents(queryData);
		CreateQueryEvent(root, instigatorActorNumber);
	}

	private void ConstructRuntimeEventManager()
	{
		MVCubeModelPrototypeTerrain singletonWorldObject = WorldObjectClientManager.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		MVCubeModelFineGrainedTerrain singletonWorldObject2 = WorldObjectClientManager.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
		runtimeEventManagerNetwork = new RuntimeEventManagerNetwork(singletonWorldObject, singletonWorldObject2);
	}

	public void AddGameQueryDataToGameWorld(BytePacker queryData, int instigatorActorNumber)
	{
		MVWorldObjectClient root = InitializeQueryData(queryData);
		CreateQueryEvent(root, instigatorActorNumber);
	}

	private MVWorldObjectClient InitializeQueryData(BytePacker queryData)
	{
		int koGaMaData = KogamaDataHandler.GetKoGaMaData(queryData, HandleDeserializedWorldData, readRuntimeData: true);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(koGaMaData);
		worldObjectClient?.Initialize();
		return worldObjectClient;
	}

	private void DeserializeRuntimeEvents(BytePacker queryData)
	{
		RuntimeEventManagerNetwork.DeserializeRuntimeEvents(queryData);
	}

	private void CreateQueryEvent(MVWorldObjectClient root, int instigatorActorNumber)
	{
		if (InitializedGameQueryData != null)
		{
			InitializedGameQueryDataEventArgs e = new InitializedGameQueryDataEventArgs(root, instigatorActorNumber);
			InitializedGameQueryData(this, e);
		}
	}

	private void HandleDeserializedWorldData(Dictionary<object, object> data, KogamaDataType dataType)
	{
		switch (dataType)
		{
		case KogamaDataType.Prototypes:
			AddPrototype(data);
			break;
		case KogamaDataType.WorldObjects:
			AddWorldObject(data);
			break;
		case KogamaDataType.Links:
			AddLink(data);
			break;
		case KogamaDataType.ObjectLinks:
			AddObjectLink(data);
			break;
		}
	}

	private void AddPrototype(Dictionary<object, object> data)
	{
		WorldInventory.AddPrototype(data);
	}

	private void AddWorldObject(Dictionary<object, object> data)
	{
		worldObjectClientManager.AddWorldObject(data, worldInventory);
	}

	private void AddLink(Dictionary<object, object> data)
	{
		Link link = new Link();
		link.id = (int)data[LinkDataParameter.Id];
		link.outputWOID = (int)data[LinkDataParameter.OutputWOID];
		link.inputWOID = (int)data[LinkDataParameter.InputWOID];
		AddLink(link);
	}

	private void AddObjectLink(Dictionary<object, object> data)
	{
		ObjectLink objectLink = new ObjectLink();
		objectLink.id = (int)data[ObjectLinkDataParameter.Id];
		objectLink.objectConnectorWOID = (int)data[ObjectLinkDataParameter.ObjectLinkConnectorWOID];
		objectLink.objectWOID = (int)data[ObjectLinkDataParameter.ObjectWOID];
		AddObjectLink(objectLink);
	}

	public void OnCloneWorldObjectTreeEvent(int ownerActorNumber, int previewProfileOwnerId, bool cloneToRootGroup, int originalId, int cloneId, int cloneLinkId, int cloneObjectLinkId)
	{
		CloneBookkeeping cloneBookkeeping = new CloneBookkeeping();
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(originalId);
		cloneBookkeeping.cloneIdIncrement = cloneId;
		cloneBookkeeping.cloneLinkIdIncrement = cloneLinkId;
		cloneBookkeeping.cloneObjectLinkIdIncrement = cloneObjectLinkId;
		MVWorldObjectClient mVWorldObjectClient = worldObjectClientManager.Clone(ownerActorNumber, worldObjectClient, cloneBookkeeping, worldInventory);
		if (previewProfileOwnerId != 0)
		{
			mVWorldObjectClient.PreviewOwnerProfileId = previewProfileOwnerId;
		}
		if (cloneToRootGroup)
		{
			worldObjectClientManager.RootGroup.TransferChild(mVWorldObjectClient.Id);
		}
		CloneLinks(cloneBookkeeping);
		CloneObjectLinks(cloneBookkeeping);
		mVWorldObjectClient.Initialize();
	}

	private void CloneLinks(CloneBookkeeping cloneBookkeeping)
	{
		if (cloneBookkeeping.cloneLinkIdIncrement == -1 && cloneBookkeeping.linkIds.Count > 0)
		{
			Debug.LogError("Found links client side even none was detected serverside");
		}
		foreach (int linkId in cloneBookkeeping.linkIds)
		{
			if (cloneBookkeeping.worldObjectIdsMaps.ContainsKey(links.GetLink(linkId).inputWOID) && cloneBookkeeping.worldObjectIdsMaps.ContainsKey(links.GetLink(linkId).outputWOID))
			{
				int inputWOID = cloneBookkeeping.worldObjectIdsMaps[links.GetLink(linkId).inputWOID];
				int outputWOID = cloneBookkeeping.worldObjectIdsMaps[links.GetLink(linkId).outputWOID];
				Link link = new Link(cloneBookkeeping.cloneLinkIdIncrement, outputWOID, inputWOID, links.GetLink(linkId).isSet);
				AddLink(link);
				cloneBookkeeping.cloneLinkIdIncrement++;
			}
		}
	}

	private void CloneObjectLinks(CloneBookkeeping cloneBookkeeping)
	{
		if (cloneBookkeeping.cloneObjectLinkIdIncrement == -1 && cloneBookkeeping.objectLinkIds.Count > 0)
		{
			Debug.LogError("Found links client side even none was detected serverside");
		}
		foreach (int objectLinkId in cloneBookkeeping.objectLinkIds)
		{
			if (cloneBookkeeping.worldObjectIdsMaps.ContainsKey(objectLinks.GetObjectLink(objectLinkId).objectConnectorWOID) && cloneBookkeeping.worldObjectIdsMaps.ContainsKey(objectLinks.GetObjectLink(objectLinkId).objectWOID))
			{
				int objectWOID = cloneBookkeeping.worldObjectIdsMaps[objectLinks.GetObjectLink(objectLinkId).objectWOID];
				int objectConnectorWOID = cloneBookkeeping.worldObjectIdsMaps[objectLinks.GetObjectLink(objectLinkId).objectConnectorWOID];
				ObjectLink objectLink = new ObjectLink(cloneBookkeeping.cloneObjectLinkIdIncrement, objectConnectorWOID, objectWOID);
				AddObjectLink(objectLink);
				cloneBookkeeping.cloneObjectLinkIdIncrement++;
			}
		}
	}

	public bool OnUnregisterWorldObject(int id)
	{
		if (!worldObjectClientManager.Contains(id))
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(id);
		OnUnregisterCleanUpLinks(worldObjectClient);
		worldObjectClientManager.SetState(worldObjectClient.Id, MVWorldObjectState.Destroyed);
		Object.Destroy(worldObjectClient.GameObject);
		worldObjectClientManager.OnWorldObjectDestroyed(worldObjectClient.Id);
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
			MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(item.outputWOID);
			MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(item.inputWOID);
			links.RemoveLink(item.id, worldObjectClient, worldObjectClient2);
		}
	}

	public void ResetLogicFromId(int worldObjectID)
	{
		Debug.Log("ResetLogicFromId: " + worldObjectID);
		links.ResetChunk(worldObjectID);
	}

	private void OnResetNode(int id)
	{
		worldObjectClientManager.GetWorldObjectClient(id).Reset();
	}

	public void AddLink(Link link)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		links.AddLink(link, worldObjectClient, worldObjectClient2);
	}

	public void RemoveLink(int linkID)
	{
		if (!links.Contains(linkID))
		{
			Debug.LogError("RemoveLink event, but link not registered!");
			return;
		}
		Link link = links.GetLink(linkID);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		links.RemoveLink(linkID, worldObjectClient, worldObjectClient2);
	}

	public bool AddPendingLink(Link link)
	{
		if (!ValidateLink(link))
		{
			return false;
		}
		links.AddPendingLink(link);
		return true;
	}

	public bool RemovePendingLink(int linkID)
	{
		if (!links.Contains(linkID))
		{
			Debug.LogError("Attempt to remove link, but link not registered");
			return false;
		}
		Link link = links.GetLink(linkID);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		links.RemovePendingLink(link.id, worldObjectClient, worldObjectClient2);
		return true;
	}

	public void HandleAddLinkResponse(bool success, int linkID)
	{
		if (success)
		{
			Link link = links.DequeuePendingLink();
			link.id = linkID;
			AddLink(link);
		}
		else
		{
			links.DequeuePendingLink();
		}
	}

	public void HandleRemoveLinkResponse(bool success)
	{
		if (success)
		{
			links.DequeuePendingRemoveLink();
			return;
		}
		Link link = links.DequeuePendingRemoveLink();
		AddLink(link);
	}

	private bool ValidateLink(Link link)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		return links.ValidateLink(link, worldObjectClient, worldObjectClient2);
	}

	public void AddObjectLink(ObjectLink objectLink)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(objectLink.objectConnectorWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(objectLink.objectWOID);
		objectLinks.AddObjectLink(objectLink, worldObjectClient, worldObjectClient2);
	}

	public void RemoveObjectLink(int objectLinkID)
	{
		if (!objectLinks.Contains(objectLinkID))
		{
			Debug.LogError("RemoveLink event, but link not registered!");
			return;
		}
		ObjectLink objectLink = objectLinks.GetObjectLink(objectLinkID);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(objectLink.objectConnectorWOID);
		objectLinks.RemoveObjectLink(objectLink, worldObjectClient);
	}

	public bool AddPendingObjectLink(ObjectLink objectLink)
	{
		if (!ValidateObjectLink(objectLink))
		{
			return false;
		}
		objectLinks.AddPendingObjectLink(objectLink);
		return true;
	}

	public bool RemovePendingObjectLink(int objectLinkID)
	{
		if (!objectLinks.Contains(objectLinkID))
		{
			Debug.LogError("Attempt to remove link, but link not registered");
			return false;
		}
		ObjectLink objectLink = objectLinks.GetObjectLink(objectLinkID);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(objectLink.objectConnectorWOID);
		objectLinks.RemovePendingObjectLink(objectLink, worldObjectClient);
		return true;
	}

	public void HandleAddObjectLinkResponse(bool success, int linkID)
	{
		if (success)
		{
			ObjectLink objectLink = objectLinks.DequeuePendingObjectLink();
			objectLink.id = linkID;
			AddObjectLink(objectLink);
		}
		else
		{
			objectLinks.DequeuePendingObjectLink();
		}
	}

	public void HandleAddObjectObjectLinkResponse(bool success, int objectLinkID)
	{
		if (success)
		{
			ObjectLink objectLink = objectLinks.DequeuePendingObjectLink();
			objectLink.id = objectLinkID;
			AddObjectLink(objectLink);
		}
		else
		{
			objectLinks.DequeuePendingObjectLink();
		}
	}

	public void HandleRemoveObjectLinkResponse(bool success)
	{
		if (success)
		{
			objectLinks.DequeuePendingRemoveObjectLink();
			return;
		}
		ObjectLink objectLink = objectLinks.DequeuePendingRemoveObjectLink();
		AddObjectLink(objectLink);
	}

	private bool ValidateObjectLink(ObjectLink objectLink)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(objectLink.objectConnectorWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(objectLink.objectWOID);
		return objectLinks.ValidateObjectLink(objectLink, worldObjectClient, worldObjectClient2);
	}
}
