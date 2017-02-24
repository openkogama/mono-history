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
		links = new Links();
		worldObjectClientManager = new MVWorldObjectClientManagerNetwork();
		objectLinks = new ObjectLinks();
	}

	public void Update(MVNetworkGame game)
	{
		links.Update();
		objectLinks.Update();
	}

	public void CreateGameWorldFromQueryData(BytePacker queryData, int instigatorActorNumber)
	{
		CullingApiWrapper.Init(10000, MVGameControllerBase.CameraController.MainCamera, CullingApiWrapper.baseDistance, MVGameControllerBase.CameraController.MainCamera.transform);
		KoGaMaDataHandler.GetKoGaMaDataAsync(queryData, HandleDeserializedWorldData, readRuntimeData: true, (int rootId) =>
		{
			OnGameDataDeserialized(queryData, instigatorActorNumber, rootId);
		});
	}

	private void OnGameDataDeserialized(BytePacker queryData, int instigatorActorNumber, int rootId)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(rootId);
		worldObjectClient?.Initialize();
		ConstructRuntimeEventManager();
		DeserializeRuntimeEvents(queryData);
		CreateQueryEvent(worldObjectClient, instigatorActorNumber);
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
		int koGaMaData = KoGaMaDataHandler.GetKoGaMaData(queryData, HandleDeserializedWorldData, readRuntimeData: true);
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

	public MVWorldObjectClient OnCloneWorldObjectTreeEvent(int ownerActorNumber, int previewProfileOwnerId, bool cloneToRootGroup, int originalId, int cloneId, int cloneLinkId, int cloneObjectLinkId)
	{
		CloneBookkeeping cloneBookkeeping = new CloneBookkeeping();
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(originalId);
		cloneBookkeeping.cloneIdIncrement = cloneId;
		Debug.Log("cloneBookkeeping.cloneLinkIdIncrement " + cloneBookkeeping.cloneLinkIdIncrement);
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
		return mVWorldObjectClient;
	}

	private void CloneLinks(CloneBookkeeping cloneBookkeeping)
	{
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
		foreach (int objectLinkId in cloneBookkeeping.objectLinkIds)
		{
			if (cloneBookkeeping.worldObjectIdsMaps.ContainsKey(objectLinks.GetObjectLink(objectLinkId).objectConnectorWOID) && cloneBookkeeping.worldObjectIdsMaps.ContainsKey(objectLinks.GetObjectLink(objectLinkId).objectWOID))
			{
				int objectWOID = cloneBookkeeping.worldObjectIdsMaps[objectLinks.GetObjectLink(objectLinkId).objectWOID];
				int objectConnectorWOID = cloneBookkeeping.worldObjectIdsMaps[objectLinks.GetObjectLink(objectLinkId).objectConnectorWOID];
				ObjectLink objectLink = new ObjectLink(cloneBookkeeping.cloneObjectLinkIdIncrement, objectConnectorWOID, objectWOID, objectLinks.GetObjectLink(objectLinkId).isSet);
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
		worldObjectClientManager.DestroyWO(id);
		return true;
	}

	public void AddLink(Link link)
	{
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		links.AddLink(link, worldObjectClient, worldObjectClient2);
	}

	public Link RemoveLink(int linkID)
	{
		if (!links.Contains(linkID))
		{
			Debug.LogError("RemoveLink event, but link not registered!");
			return null;
		}
		Link link = links.GetLink(linkID);
		MVWorldObjectClient worldObjectClient = worldObjectClientManager.GetWorldObjectClient(link.outputWOID);
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(link.inputWOID);
		links.RemoveLink(linkID, worldObjectClient, worldObjectClient2);
		return link;
	}

	public bool LinksContains(int linkID)
	{
		return links.Contains(linkID);
	}

	public bool ObjectLinksContains(int linkID)
	{
		return objectLinks.Contains(linkID);
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
		MVWorldObjectClient worldObjectClient2 = worldObjectClientManager.GetWorldObjectClient(objectLink.objectWOID);
		objectLinks.RemoveObjectLink(objectLink, worldObjectClient, worldObjectClient2);
	}
}
