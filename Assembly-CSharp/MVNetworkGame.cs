using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ExitGames.Client.Photon;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkGame : IPhotonPeerListener
{
	public delegate void OnPlayerListChangedDelegate();

	public delegate void OnReceivedChatMessageDelegate(string sender, string message);

	private const string appName = "MVGameServer";

	public const int numPrototypesToReturnPerBatch = 25;

	public const int numWOToReturnPerBatch = 50;

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(MVNetworkGame));

	private LitePeer peer;

	private MVConnState connState;

	private MVJoinState joinState;

	private string planetName = "TestPlanet";

	private string gameName = string.Empty;

	private string ip;

	private int port;

	public int numPrototypesToFetch;

	public int numPrototypesFetched;

	public int prototypeBatchQueryID = -1;

	public int numWorldObjectsToFetch;

	public int numWorldObjectsFetched;

	public int woBatchQueryID = -1;

	private bool editorMode;

	private int[] linkTable;

	private string username;

	private string password;

	private MVGUIManager guiManager;

	private MVNetworkGameStateListener networkGameStateListener;

	public OnPlayerListChangedDelegate onPlayerListChanged;

	public OnReceivedChatMessageDelegate OnReceivedChatMessage;

	public MVConnState ConnState
	{
		get
		{
			return connState;
		}
		set
		{
			connState = value;
		}
	}

	public MVJoinState JoinState => joinState;

	public MVNetworkGameStateListener NetworkGameStateListener => networkGameStateListener;

	public LitePeer Peer => peer;

	public bool EditorMode => editorMode;

	public string PlanetName => planetName;

	public string GameName => gameName;

	public string Ip => ip;

	public int Port => port;

	public MVNetworkGame(string planetName, string gameName, string ip, int port, bool editorMode, MVGUIManager guiManager)
	{
		this.planetName = planetName;
		this.gameName = gameName;
		this.ip = ip;
		this.port = port;
		this.editorMode = editorMode;
		peer = new LitePeer(this, useTcp: false);
		peer.DebugOut = DebugLevel.WARNING;
		this.guiManager = guiManager;
		numPrototypesFetched = 0;
		networkGameStateListener = new MVNetworkGameStateListener();
	}

	public void Update()
	{
		try
		{
			if (peer != null)
			{
				peer.Service();
				if (joinState == MVJoinState.Playing)
				{
					MVGameController.Instance.WOCM.Update(this);
				}
				networkGameStateListener.Update(this);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Exception in update loop: " + ex.ToString()));
			Debug.LogError((object)("JoinState: " + joinState));
			Debug.Log((object)ex.StackTrace);
			throw ex;
		}
	}

	private void GotoNextJoinState()
	{
		switch (joinState)
		{
		case MVJoinState.Joining:
			joinState = MVJoinState.FetchingItemTypes;
			RequestDBQuery(DBQuery.RequestItemTypes, new Hashtable());
			break;
		case MVJoinState.FetchingItemTypes:
			joinState = MVJoinState.FetchingOwnershipTypes;
			RequestDBQuery(DBQuery.RequestPlanetOwnershipTypes, new Hashtable());
			break;
		case MVJoinState.FetchingOwnershipTypes:
		{
			joinState = MVJoinState.FetchingInventory;
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)0, MVGameController.Instance.WOCM.LocalPlayer.ProfileID);
			RequestLargeDBQuery(DBQuery.RequestInventory, hashtable, 10);
			break;
		}
		case MVJoinState.FetchingInventory:
			joinState = MVJoinState.FetchingPrototypes;
			peer.OpCustom(121, new Hashtable(), sendReliable: true);
			break;
		case MVJoinState.FetchingPrototypes:
			joinState = MVJoinState.FetchingUserList;
			peer.OpCustom(109, new Hashtable(), sendReliable: true);
			break;
		case MVJoinState.FetchingUserList:
			joinState = MVJoinState.FetchingFriends;
			peer.OpCustom(137, new Hashtable(), sendReliable: true);
			break;
		case MVJoinState.FetchingFriends:
			joinState = MVJoinState.FetchingWorldObjects;
			peer.OpCustom(110, new Hashtable(), sendReliable: true);
			break;
		case MVJoinState.FetchingWorldObjects:
			MVGameController.Instance.WOCM.BuildObjectHierarchies();
			joinState = MVJoinState.FetchingLinks;
			RequestLinks();
			break;
		case MVJoinState.FetchingLinks:
			joinState = MVJoinState.CreatingAvatar;
			CreateLocalAvatar();
			break;
		case MVJoinState.CreatingAvatar:
			joinState = MVJoinState.InitializingWorld;
			InitializeWorld();
			break;
		case MVJoinState.InitializingWorld:
			joinState = MVJoinState.SettingActorReady;
			SetActorReady();
			break;
		case MVJoinState.SettingActorReady:
			joinState = MVJoinState.Playing;
			break;
		case MVJoinState.Playing:
			break;
		case MVJoinState.BuildingGroupHierarchies:
			break;
		}
	}

	public bool Join(string username, string password)
	{
		connState = MVConnState.Connecting;
		try
		{
			if (!peer.Connect(ip + ":" + port, "MVGameServer"))
			{
				Debug.LogError((object)"Not able to connect. Internet connection available?");
				return false;
			}
			this.username = username;
			this.password = password;
		}
		catch (SecurityException ex)
		{
			Debug.LogError((object)("Security Exception (check policy file): " + ex));
			return false;
		}
		catch (Exception ex2)
		{
			Debug.LogError((object)("Unknown exception during connect: '" + ex2.ToString() + "', Message: '" + ex2.Message + "'"));
			return false;
		}
		return true;
	}

	public void Leave()
	{
		if (connState == MVConnState.Joined)
		{
			connState = MVConnState.Leaving;
			joinState = MVJoinState.Leaving;
			peer.OpLeave("MVGameServer");
		}
		else
		{
			connState = MVConnState.Disconnected;
			joinState = MVJoinState.Leaving;
			peer.Disconnect();
		}
	}

	public void PublishPlanet(byte[] pngImageAsByteArray)
	{
		if (joinState == MVJoinState.Playing && editorMode)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)136, pngImageAsByteArray);
			peer.OpCustom(129, hashtable, sendReliable: true);
		}
	}

	public void SetActorReady()
	{
		peer.OpCustom(105, new Hashtable(), sendReliable: true);
	}

	public void InitializeWorld()
	{
		IEnumerable<MVWorldObjectClient> enumerable = from k in MVGameController.Instance.WOCM.WorldObjects.Keys
			orderby k
			select MVGameController.Instance.WOCM.WorldObjects[k];
		foreach (MVWorldObjectClient item in enumerable)
		{
			item.Initialize();
		}
		int num = 0;
		for (int num2 = 0; num2 < linkTable.Length / 4; num2++)
		{
			Link link = new Link();
			link.id = linkTable[num++];
			link.outputWOID = linkTable[num++];
			link.inputWOID = linkTable[num++];
			link.isSet = linkTable[num++] == 1;
			MVGameController.Instance.WOCM.AddLink(link);
		}
		GotoNextJoinState();
	}

	public void ResetWorld()
	{
		foreach (MVWorldObjectClient value in MVGameController.Instance.WOCM.WorldObjects.Values)
		{
			if (value is MVLogicObject)
			{
				value.ResetLogic();
			}
		}
	}

	public void RegisterPrototype(MVItem item)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)105, item.itemID);
		hashtable.Add((byte)106, item.itemTypeID);
		hashtable.Add((byte)107, item.name);
		peer.OpCustom(118, hashtable, sendReliable: true);
	}

	public void RegisterLocalPrototype(string name, int typeID, byte[] objectData, float scale)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)113, name);
		hashtable.Add((byte)106, typeID);
		hashtable.Add((byte)114, objectData);
		hashtable.Add((byte)99, scale);
		peer.OpCustom(119, hashtable, sendReliable: true);
	}

	public void AutoRegisterLocalPrototype(int woId, int worldInventoryID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)112, worldInventoryID);
		hashtable.Add((byte)81, woId);
		peer.OpCustom(149, hashtable, sendReliable: true);
	}

	public void UpdatePrototype(int worldInventoryID, byte[] prototypeData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)112, worldInventoryID);
		hashtable.Add((byte)114, prototypeData);
		peer.OpCustom(123, hashtable, sendReliable: true);
	}

	public void UpdateGroup(int woId, byte[] prototypeData)
	{
	}

	public void UpdatePrototypeScale(int worldInventoryID, float scale)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)112, worldInventoryID);
		hashtable.Add((byte)99, scale);
		peer.OpCustom(124, hashtable, sendReliable: true);
	}

	public void AddPrototypeToInventory(int worldInventoryID, int slotIndex, byte[] itemTextureData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)112, worldInventoryID);
		hashtable.Add((byte)110, slotIndex);
		hashtable.Add((byte)109, itemTextureData);
		peer.OpCustom(130, hashtable, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void RemoveItemFromInventory(int itemID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)105, itemID);
		peer.OpCustom(131, hashtable, sendReliable: true);
	}

	public void UpdateInventorySlots()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)111, MVGameController.Instance.WOCM.PlayerRepository.itemIDToInventorySlotIndex);
		peer.OpCustom(132, hashtable, sendReliable: true);
	}

	public void AddItemToQuickSlot(int worldObjectID, int slotIndex)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)110, slotIndex);
		peer.OpCustom(133, hashtable, sendReliable: true);
	}

	public void RemoveItemFromQuickSlot(int worldObjectID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		peer.OpCustom(134, hashtable, sendReliable: true);
	}

	public void UpdateQuickSlot(int worldObjectID, int slotIndex)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)110, slotIndex);
		peer.OpCustom(135, hashtable, sendReliable: true);
	}

	public void RequestQuickSlots()
	{
		peer.OpCustom(136, new Hashtable(), sendReliable: true);
	}

	public void SendChatMsg(string chatMsg)
	{
		if (chatMsg.Length > 256)
		{
			Debug.LogWarning((object)"ChatMsg too long. Truncated to 256 chars!");
			chatMsg = chatMsg.Substring(0, 256);
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)138, chatMsg);
		peer.OpCustom(147, hashtable, sendReliable: true);
	}

	public void UpdateTerrain(int worldObjectID, byte[] terrainData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)126, terrainData);
		peer.OpCustom(125, hashtable, sendReliable: true);
	}

	public void UpdateWorldObjectData(int worldObjectID, Hashtable worldObjectData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)78, worldObjectData);
		peer.OpCustom(114, hashtable, sendReliable: true);
	}

	public void UpdateWorldObjectRunTimeData(int worldObjectID, Hashtable worldObjectRunTimeData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)145, worldObjectRunTimeData);
		peer.OpCustom(153, hashtable, sendReliable: true);
	}

	public void RegisterWorldObject(WorldObjectType type, int groupId, Hashtable woData, Hashtable runTimeData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.WOCM.RegisterWorldObject(type, groupId, woData, runTimeData, MVGameController.Instance.WOCM.LocalPlayerActorNumber, position, rotation, scale);
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)77, type);
		hashtable.Add((byte)82, groupId);
		hashtable.Add((byte)78, woData);
		hashtable.Add((byte)145, runTimeData);
		hashtable.Add((byte)80, localOwner);
		hashtable.Add((byte)104, transferOwnershipToServerOnLeave);
		hashtable.Add((byte)89, position.x);
		hashtable.Add((byte)90, position.y);
		hashtable.Add((byte)91, position.z);
		hashtable.Add((byte)92, rotation.x);
		hashtable.Add((byte)93, rotation.y);
		hashtable.Add((byte)94, rotation.z);
		hashtable.Add((byte)95, rotation.w);
		hashtable.Add((byte)96, scale.x);
		hashtable.Add((byte)97, scale.y);
		hashtable.Add((byte)98, scale.z);
		peer.OpCustom(111, hashtable, sendReliable: true);
	}

	public void UnregisterWorldObject(int worldObjectID)
	{
		if (MVGameController.Instance.WOCM.UnregisterWorldObject(worldObjectID))
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)81, worldObjectID);
			peer.OpCustom(112, hashtable, sendReliable: true);
		}
	}

	public void Ungroup(int worldObjectID)
	{
		if (!MVGameController.Instance.WOCM.Ungroup(worldObjectID))
		{
			Debug.LogWarning((object)"Ungroup failed");
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		peer.OpCustom(144, hashtable, sendReliable: true);
	}

	public void ReportCaptureFlag()
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState != AvatarState.Editing)
		{
			peer.OpCustom(150, new Hashtable(), sendReliable: true);
		}
	}

	public void SetActorProperty(Hashtable properties)
	{
		peer.OpSetPropertiesOfActor(MVGameController.Instance.WOCM.LocalPlayer.ActorNr, properties, broadcast: true, 0);
	}

	public void ResetLogicChunk(int worldObjectID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		peer.OpCustom(152, hashtable, sendReliable: true);
	}

	public void OnUnregisterWorldObjectResponse(bool success)
	{
		if (!MVGameController.Instance.WOCM.UnregisterWorldObjectResponse(success))
		{
			Debug.LogWarning((object)"OnUnregisterWorldObjectResponse failed!");
		}
	}

	public void UpdateWorldObject(int id, Vector3 position, Quaternion rotation, TransformPackageType packageType)
	{
		if (joinState == MVJoinState.Playing)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)81, id);
			hashtable.Add((byte)100, peer.ServerTimeInMilliSeconds);
			hashtable.Add((byte)89, position.x);
			hashtable.Add((byte)90, position.y);
			hashtable.Add((byte)91, position.z);
			hashtable.Add((byte)92, rotation.x);
			hashtable.Add((byte)93, rotation.y);
			hashtable.Add((byte)94, rotation.z);
			hashtable.Add((byte)95, rotation.w);
			hashtable.Add((byte)101, (byte)packageType);
			peer.OpCustom(113, hashtable, sendReliable: false);
		}
	}

	public void UpdateLocalAvatarCam()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		MVCameraController weCamera = MVGameController.Instance.WOCM.WeCamera;
		Vector3 forward = ((Component)weCamera).transform.forward;
		Vector3 position = ((Component)weCamera).transform.position;
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)149, position.x);
		hashtable.Add((byte)150, position.y);
		hashtable.Add((byte)151, position.z);
		hashtable.Add((byte)152, forward.x);
		hashtable.Add((byte)153, forward.y);
		hashtable.Add((byte)154, forward.z);
		peer.OpCustom(156, hashtable, sendReliable: true);
	}

	public void UpdateNetworkInput(int id, NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		if (joinState == MVJoinState.Playing)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)81, id);
			hashtable.Add((byte)100, peer.ServerTimeInMilliSeconds);
			hashtable.Add((byte)102, (byte)actionCode);
			hashtable.Add((byte)103, (byte)keyCode);
			peer.OpCustom(115, hashtable, sendReliable: true);
		}
	}

	public void TransferOwnership(int worldObjectID, int ownerActorNr)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)79, ownerActorNr);
		peer.OpCustom(116, hashtable, sendReliable: true);
	}

	public void LockHierarchy(int worldObjectID, bool lockHierarchy)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, worldObjectID);
		hashtable.Add((byte)135, lockHierarchy);
		peer.OpCustom(145, hashtable, sendReliable: true);
	}

	public void RequestFriendShipByName(string name)
	{
		if (name != MVGameController.Instance.WOCM.LocalPlayer.Username)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)71, name);
			peer.OpCustom(138, hashtable, sendReliable: true);
		}
		else
		{
			guiManager.ShowMessageBox("Cant request friendship from yourself!");
		}
	}

	public void RequestFriendShipByID(int id)
	{
		if (id != MVGameController.Instance.WOCM.LocalPlayer.ProfileID)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)124, id);
			peer.OpCustom(139, hashtable, sendReliable: true);
		}
		else
		{
			guiManager.ShowMessageBox("Cant request friendship from yourself!");
		}
	}

	public void OnSyncAvatarStatusEvent(int actorNr, Hashtable data)
	{
	}

	public void OnResetLogicChunkEvent(int worldObjectID)
	{
		MVGameController.Instance.WOCM.ResetLogicFromId(worldObjectID);
	}

	public void OnPickupItemStateChangeEvent(PickupItemState state, int worldObjectID, int instigatorActorNr)
	{
		Debug.Log((object)("OnPickupItemStateChangeEvent, state: " + state.ToString() + ", instigator: " + instigatorActorNr));
		if (MVGameController.Instance.WOCM.WorldObjects.ContainsKey(worldObjectID))
		{
			if (!(MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVPickupItemBase))
			{
				Debug.LogError((object)"PickUpItemStateChangeEvent failed, since WOID is not derived from MVPickupItemBase");
				return;
			}
			MVPickupItemBase mVPickupItemBase = (MVPickupItemBase)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVPickupItemBase.HandleStateChange(state, instigatorActorNr);
		}
		else
		{
			Debug.LogWarning((object)("OnPickupItemStateChangeEvent failed, since WorldObjectID does not exist. WOID: " + worldObjectID));
		}
	}

	public void OnUpdateLineOfFire(int actorNr, Vector3 camOrigin, Vector3 camDir)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.WOCM.Players[actorNr].Avatar.Avatar.SetLineOfFire(camOrigin, camDir);
	}

	public List<CommonOverlapArg> GetWOIdsWithinRadius(float radius, Vector3 worldPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(worldPos, radius);
		List<CommonOverlapArg> list = new List<CommonOverlapArg>();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < array.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)array[i]).transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg item = new CommonOverlapArg(mVObject);
				list.Add(item);
				hashSet.Add(mVObject.Id);
			}
		}
		return list;
	}

	public void OnRemoveCubesWithinRadiusEvent(float radius, Vector3 worldPos, float damage, DamageFallOffType damageFallOffType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		RemoveCubes.HandleRemoveCubes(GetWOIdsWithinRadius(radius, worldPos), radius, worldPos, damage, damageFallOffType, MVGameController.Instance.WOCM.FineGrainedTerrain);
	}

	private void OnRequestFriendshipResponse(int returnCode)
	{
		switch (returnCode)
		{
		case 0:
			break;
		case -1:
			guiManager.ShowMessageBox("Undefined fail during friend request");
			break;
		case -2:
			guiManager.ShowMessageBox("User does not exist");
			break;
		case -3:
			guiManager.ShowMessageBox("You already have a pending request with that user");
			break;
		case -4:
			guiManager.ShowMessageBox("You are already friends with that user");
			break;
		case -5:
			guiManager.ShowMessageBox("You have blocked that user");
			break;
		case -6:
			guiManager.ShowMessageBox("User has sent you request! Accept?");
			break;
		case -7:
			guiManager.ShowMessageBox("That user has blocked you");
			break;
		}
	}

	public void RequestAcceptFriendShip(int friendID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)123, friendID);
		peer.OpCustom(140, hashtable, sendReliable: true);
	}

	public void RequestRejectFriendShip(int friendID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)123, friendID);
		peer.OpCustom(141, hashtable, sendReliable: true);
	}

	public void RequestWoUniquePrototype(int woId)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)81, woId);
		peer.OpCustom(149, hashtable, sendReliable: true);
	}

	private void JoinGame()
	{
		connState = MVConnState.Joining;
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)71, username);
		hashtable.Add((byte)72, password);
		hashtable.Add((byte)4, gameName);
		hashtable.Add((byte)70, planetName);
		hashtable.Add((byte)74, editorMode);
		peer.OpCustom(90, hashtable, sendReliable: true);
	}

	public void AddLink(Link link)
	{
		if (link.inputWOID <= 0 || link.outputWOID <= 0)
		{
			Debug.LogError((object)"Attempt to add link, but link not added to input/output WO's");
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)129, link.outputWOID);
		hashtable.Add((byte)128, link.inputWOID);
		peer.OpCustom(126, hashtable, sendReliable: true);
		MVGameController.Instance.WOCM.AddPendingLink(link);
	}

	public void RemoveLink(Link link)
	{
		if (!MVGameController.Instance.WOCM.Links.ContainsKey(link.id))
		{
			Debug.LogError((object)"Attempt to remove link, but link not registered");
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)130, link.id);
		peer.OpCustom(127, hashtable, sendReliable: true);
		MVGameController.Instance.WOCM.RemovePendingLink(link);
	}

	public void RequestLinks()
	{
		peer.OpCustom(128, new Hashtable(), sendReliable: true);
	}

	public void RequestRemoveCubesWithinRadius(int[] woIds, float radius, Vector3 position, float damage, DamageFallOffType damageFallOffType)
	{
		if (radius <= 0f)
		{
			Debug.LogError((object)"Radius can't be zero or less!");
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)147, woIds);
		hashtable.Add((byte)148, radius);
		hashtable.Add((byte)89, position.x);
		hashtable.Add((byte)90, position.y);
		hashtable.Add((byte)91, position.z);
		hashtable.Add((byte)155, damage);
		hashtable.Add((byte)156, (int)damageFallOffType);
		peer.OpCustom(155, hashtable, sendReliable: true);
	}

	public void TriggerBoxEnter(MVWorldObjectClient wo)
	{
		if (joinState == MVJoinState.Playing)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)81, wo.Id);
			peer.OpCustom(142, hashtable, sendReliable: true);
		}
	}

	public void TriggerBoxExit(MVWorldObjectClient wo)
	{
		if (joinState == MVJoinState.Playing)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)81, wo.Id);
			peer.OpCustom(143, hashtable, sendReliable: true);
		}
	}

	public void UploadPlanetScreenshot(byte[] textureData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)136, textureData);
		peer.OpCustom(148, hashtable, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void PurchaseItem(int itemID, int prototypeID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)105, itemID);
		hashtable.Add((byte)112, prototypeID);
		peer.OpCustom(154, hashtable, sendReliable: true);
	}

	private void OnJoinResponse(Hashtable returnValues)
	{
		int profileID = (int)returnValues[(byte)73];
		int num = (int)returnValues[(byte)9];
		int planetOwnershipTypeID = (int)returnValues[(byte)76];
		MVGameController.Instance.WOCM.LocalPlayerActorNumber = num;
		AddPlayer(username, profileID, num, planetOwnershipTypeID);
		MVGameController.Instance.WOCM.LocalPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		if (MVGameController.Instance.WOCM.LocalPlayer.ProfileID == -1)
		{
			Debug.Log((object)"Anonymous log-in...");
		}
		else if (MVGameController.Instance.WOCM.LocalPlayer.PlanetOwnershipTypeID == 1)
		{
			logger.Log("WELCOME TO YOUR PLANET, MEMBER!");
		}
		else if (MVGameController.Instance.WOCM.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			logger.Log("WELCOME TO YOUR PLANET, OWNER");
		}
		else
		{
			logger.Log("You are NOT an owner of this planet!");
		}
		MVGameStateType gameStateType = (MVGameStateType)(int)returnValues[(byte)139];
		int startTime = (int)returnValues[(byte)141];
		int duration = (int)returnValues[(byte)140];
		MVGameStateReason reason = (MVGameStateReason)(int)returnValues[(byte)142];
		networkGameStateListener.ChangeState(this, gameStateType, startTime, duration, reason, 0);
		Application.LoadLevel("LoadPlanet");
		GotoNextJoinState();
	}

	private void OnRequestUserListResponse(Hashtable userList)
	{
		if (userList != null)
		{
			foreach (int key in userList.Keys)
			{
				logger.Log("User in UserList: " + (userList[key] as Hashtable)[(byte)71]);
				if (key != MVGameController.Instance.WOCM.LocalPlayerActorNumber)
				{
					string text = (string)(userList[key] as Hashtable)[(byte)71];
					int num2 = (int)(userList[key] as Hashtable)[(byte)73];
					Hashtable hashtable = (Hashtable)(userList[key] as Hashtable)[(byte)143];
					AddPlayer((string)(userList[key] as Hashtable)[(byte)71], (int)(userList[key] as Hashtable)[(byte)73], key);
					logger.Log("AVATAR HEALTH: " + (float)hashtable["health"]);
					MVGameController.Instance.WOCM.Players[key].InitAvatarStatus = hashtable;
				}
			}
		}
		else
		{
			Debug.LogWarning((object)"UserList is null");
		}
		GotoNextJoinState();
	}

	private void OnRequestFriendsResponse(Hashtable friendsList)
	{
		if (friendsList != null)
		{
			foreach (int key in friendsList.Keys)
			{
				Hashtable hashtable = (Hashtable)friendsList[key];
				int profileID = (int)hashtable[(byte)0];
				int friendProfileID = (int)hashtable[(byte)27];
				FriendStatus status = (FriendStatus)(int)hashtable[(byte)29];
				MVGameController.Instance.WOCM.Friends.AddFriend(key, profileID, friendProfileID, status);
			}
		}
		else
		{
			Debug.LogWarning((object)"Friendslist is null");
		}
		GotoNextJoinState();
	}

	private void OnRequestPrototypesResponse(int numPrototypes, int queryID)
	{
		numPrototypesToFetch = numPrototypes;
		numPrototypesFetched = 0;
		prototypeBatchQueryID = queryID;
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)116, prototypeBatchQueryID);
		hashtable.Add((byte)119, 25);
		peer.OpCustom(122, hashtable, sendReliable: true);
	}

	private void OnGetNextPrototypeBatchResponse(Hashtable returnValues)
	{
		if (joinState == MVJoinState.FetchingPrototypes)
		{
			Hashtable hashtable = (Hashtable)returnValues[(byte)118];
			foreach (object key in hashtable.Keys)
			{
				Hashtable hashtable2 = (Hashtable)hashtable[key];
				int id = (int)hashtable2[(byte)112];
				int itemID = (int)hashtable2[(byte)105];
				int typeID = (int)hashtable2[(byte)106];
				string name = (string)hashtable2[(byte)113];
				Hashtable data = (Hashtable)hashtable2[(byte)114];
				float scale = (float)hashtable2[(byte)99];
				MVGameController.Instance.WOCM.WorldInventory.AddPrototype(id, itemID, typeID, name, data, scale, -1);
				numPrototypesFetched++;
			}
			if (numPrototypesFetched >= numPrototypesToFetch)
			{
				GotoNextJoinState();
				return;
			}
			Hashtable hashtable3 = new Hashtable();
			hashtable3.Add((byte)116, prototypeBatchQueryID);
			hashtable3.Add((byte)119, 25);
			peer.OpCustom(122, hashtable3, sendReliable: true);
		}
		else
		{
			Debug.LogError((object)"PrototypeBatch'es should only be used during init");
		}
	}

	private void OnRequestWorldObjectsResponse(int numWorldObjects, int queryID)
	{
		if (joinState == MVJoinState.FetchingWorldObjects)
		{
			numWorldObjectsToFetch = numWorldObjects;
			numWorldObjectsFetched = 0;
			woBatchQueryID = queryID;
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)83, woBatchQueryID);
			hashtable.Add((byte)86, 50);
			peer.OpCustom(117, hashtable, sendReliable: true);
		}
		else
		{
			Debug.LogError((object)"OnRequestWorldObjectsResponse: gameState is NOT MVGameState.FetchingWorldObjects!");
		}
	}

	private void OnGetNextWOBatchResponse(Hashtable returnValues)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if (joinState == MVJoinState.FetchingWorldObjects)
		{
			Hashtable hashtable = (Hashtable)returnValues[(byte)85];
			foreach (object key in hashtable.Keys)
			{
				Hashtable hashtable2 = (Hashtable)hashtable[key];
				WorldObjectType type = (WorldObjectType)(int)hashtable2[(byte)77];
				Hashtable data = (Hashtable)hashtable2[(byte)78];
				Hashtable runTimeData = (Hashtable)hashtable2[(byte)145];
				int id = (int)hashtable2[(byte)81];
				int groupId = (int)hashtable2[(byte)82];
				int ownerActorNr = (int)hashtable2[(byte)79];
				Vector3 position = new Vector3((float)hashtable2[(byte)89], (float)hashtable2[(byte)90], (float)hashtable2[(byte)91]);
				Quaternion rotation = new Quaternion((float)hashtable2[(byte)92], (float)hashtable2[(byte)93], (float)hashtable2[(byte)94], (float)hashtable2[(byte)95]);
				Vector3 scale = new Vector3((float)hashtable2[(byte)96], (float)hashtable2[(byte)97], (float)hashtable2[(byte)98]);
				MVGameController.Instance.WOCM.RegisterWorldObjectProxy(type, data, runTimeData, id, groupId, ownerActorNr, position, rotation, scale);
				numWorldObjectsFetched++;
			}
			if (numWorldObjectsFetched >= numWorldObjectsToFetch)
			{
				GotoNextJoinState();
				return;
			}
			Hashtable hashtable3 = new Hashtable();
			hashtable3.Add((byte)83, woBatchQueryID);
			hashtable3.Add((byte)86, 50);
			peer.OpCustom(117, hashtable3, sendReliable: true);
		}
		else
		{
			Debug.LogError((object)"This (WOBatch'es) should only be used during init, not while playing...");
		}
	}

	private void OnRegisterWorldObjectResponse(int id, bool success)
	{
		MVGameController.Instance.WOCM.RegisterWorldObjectResponse(id, success);
		if (joinState == MVJoinState.CreatingAvatar)
		{
			MVGameController.Instance.WOCM.LocalPlayer.Avatar = (MVAvatar)MVGameController.Instance.WOCM.WorldObjects[id];
			GotoNextJoinState();
		}
	}

	private void OnTransferOwnershipResponse(Hashtable returnValues, int returnCode)
	{
		int id = (int)returnValues[(byte)81];
		int ownerActorNr = (int)returnValues[(byte)79];
		if (returnCode == 0)
		{
			MVGameController.Instance.WOCM.TransferOwnershipResponse(id, ownerActorNr, success: true);
		}
		else
		{
			MVGameController.Instance.WOCM.TransferOwnershipResponse(id, ownerActorNr, success: false);
		}
	}

	private void OnLockHierarchyResponse(Hashtable returnValues, int returnCode)
	{
		int id = (int)returnValues[(byte)81];
		bool lockObject = (bool)returnValues[(byte)135];
		if (returnCode == 0)
		{
		}
		MVGameController.Instance.WOCM.LockHierarchyResponse(id, lockObject, returnCode == 0);
	}

	private void OnRequestWoUniquePrototypeFailed(Hashtable returnValues)
	{
		Debug.LogWarning((object)"OnRequestWoUniquePrototypeFailed");
		int woId = (int)returnValues[(byte)81];
		MVGameController.Instance.WOCM.WorldInventory.UnpendRuntimePrototype(woId);
	}

	private void OnRequestLinksResponse(int[] linkTable)
	{
		this.linkTable = (int[])linkTable.Clone();
		GotoNextJoinState();
	}

	private void OnPurchaseItemResponse(int returnCode)
	{
		string message = string.Empty;
		switch (returnCode)
		{
		case 0:
			message = "Item purchased";
			break;
		case -1:
			message = "Undefined fail!";
			break;
		case -2:
			message = "You don't have enough\nsilver to buy the item";
			break;
		case -3:
			message = "You cannot purchase the item,\nsince you are the owner";
			break;
		case -4:
			message = "The item to purchase\nwas not found";
			break;
		case -5:
			message = "The item is already in your inventory";
			break;
		}
		MVGUIMessageBox.New(message);
	}

	private void OnLockHierarchyEvent(Hashtable eventData)
	{
		Debug.Log((object)"OnLockHierarchyEvent");
		MVGameController.Instance.WOCM.LockHierarchyProxy((int)eventData[(byte)81], (int)eventData[(byte)79]);
	}

	private void OnUngroupResponse(bool success)
	{
		MVGameController.Instance.WOCM.UngroupResponse(success);
	}

	private void OnUngroupEvent(Hashtable eventData)
	{
		MVGameController.Instance.WOCM.UngroupProxy((int)eventData[(byte)81]);
	}

	private void OnRegisterWorldObjectEvent(Hashtable eventData)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)eventData[(byte)79];
		if (num == MVGameController.Instance.WOCM.LocalPlayer.ActorNr)
		{
			Debug.LogError((object)"Attempt to register world object, but object is owned by local player. Server-side error: Use operation response instead");
			return;
		}
		WorldObjectType type = (WorldObjectType)(int)eventData[(byte)77];
		Hashtable data = (Hashtable)eventData[(byte)78];
		Hashtable runTimeData = (Hashtable)eventData[(byte)145];
		int id = (int)eventData[(byte)81];
		int groupId = (int)eventData[(byte)82];
		Vector3 position = new Vector3((float)eventData[(byte)89], (float)eventData[(byte)90], (float)eventData[(byte)91]);
		Quaternion rotation = new Quaternion((float)eventData[(byte)92], (float)eventData[(byte)93], (float)eventData[(byte)94], (float)eventData[(byte)95]);
		Vector3 scale = new Vector3((float)eventData[(byte)96], (float)eventData[(byte)97], (float)eventData[(byte)98]);
		MVGameController.Instance.WOCM.RegisterWorldObjectProxy(type, data, runTimeData, id, groupId, num, position, rotation, scale);
	}

	private void OnUnregisterWorldObjectEvent(int worldObjectID)
	{
		MVGameController.Instance.WOCM.UnregisterWorldObjectProxy(worldObjectID);
	}

	private void OnUpdateWorldObjectEvent(Hashtable photonEvent)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (joinState == MVJoinState.Playing)
		{
			int key = (int)photonEvent[(byte)81];
			if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(key))
			{
				Debug.LogError((object)"Attempt to update world object, but object not registered in world");
			}
			else if (MVGameController.Instance.WOCM.WorldObjects[key].State != MVWorldObjectState.Destroyed)
			{
				NetworkTransformPackage networkTransformPackage = new NetworkTransformPackage();
				networkTransformPackage.position = new Vector3((float)photonEvent[(byte)89], (float)photonEvent[(byte)90], (float)photonEvent[(byte)91]);
				networkTransformPackage.rotation = new Quaternion((float)photonEvent[(byte)92], (float)photonEvent[(byte)93], (float)photonEvent[(byte)94], (float)photonEvent[(byte)95]);
				networkTransformPackage.timestamp = (int)photonEvent[(byte)100];
				networkTransformPackage.packageType = (TransformPackageType)(byte)photonEvent[(byte)101];
				(MVGameController.Instance.WOCM.WorldObjects[key].NetworkObject as MVNetworkListener).AddTransformPackage(networkTransformPackage);
			}
			else
			{
				Debug.LogWarning((object)"Attempt to update world object, but object in destroyed state");
			}
		}
	}

	private void OnUpdateNetworkInputEvent(Hashtable photonEvent)
	{
		int key = (int)photonEvent[(byte)81];
		NetworkInputActionCodes actionCode = (NetworkInputActionCodes)(byte)Enum.ToObject(typeof(NetworkInputActionCodes), (byte)photonEvent[(byte)102]);
		NetworkInputKeyCodes keyCode = (NetworkInputKeyCodes)(byte)photonEvent[(byte)103];
		int timestamp = (int)photonEvent[(byte)100];
		if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(key))
		{
			Debug.LogError((object)"Attempt to update network input on world object that is not in list");
			return;
		}
		if (!(MVGameController.Instance.WOCM.WorldObjects[key].NetworkObject is MVNetworkListener))
		{
			Debug.LogError((object)"Attempt to update network input, but NetworkObject is not a listener");
		}
		NetworkInputPackage networkInputPackage = new NetworkInputPackage();
		networkInputPackage.actionCode = actionCode;
		networkInputPackage.keyCode = keyCode;
		networkInputPackage.timestamp = timestamp;
		(MVGameController.Instance.WOCM.WorldObjects[key].NetworkObject as MVNetworkListener).AddNetworkInputPackage(networkInputPackage);
	}

	private void OnTransferOwnershipEvent(int worldObjectID, int ownerActorNr)
	{
		MVGameController.Instance.WOCM.TransferOwnershipProxy(worldObjectID, ownerActorNr);
	}

	private void OnRegisterPrototypeEvent(int worldInventoryID, int itemID, int worldInventoryTypeID, string worldInventoryName, Hashtable worldInventoryData, float scale, int actorNr)
	{
		MVGameController.Instance.WOCM.AddPrototype(worldInventoryID, itemID, worldInventoryTypeID, worldInventoryName, worldInventoryData, scale, actorNr);
	}

	private void OnUnregisterPrototypeEvent(int worldInventoryID)
	{
		MVGameController.Instance.WOCM.RemovePrototype(worldInventoryID);
	}

	private void OnFriendRequestEvent(int friendID, int profileID, int friendProfileID)
	{
		guiManager.ShowMessageBox("Friend request FriendID: " + friendID + " Profile: " + profileID + " FriendProfile: " + friendProfileID);
		MVGameController.Instance.WOCM.Friends.AddFriend(friendID, profileID, friendProfileID, FriendStatus.Pending);
	}

	private void OnFriendUpdateEvent(int friendID, int profileID, FriendStatus status)
	{
		guiManager.ShowMessageBox("Friendship changed FriendID: " + friendID + " ProfileID: " + profileID + " status: " + status.ToString());
		MVGameController.Instance.WOCM.Friends.UpdateFriend(friendID, profileID, status);
	}

	private void OnAddLinkEvent(int fromID, int toID, int linkID)
	{
		Link link = new Link();
		link.outputWOID = fromID;
		link.inputWOID = toID;
		link.id = linkID;
		MVGameController.Instance.WOCM.AddLink(link);
	}

	private void OnRemoveLinkEvent(int linkID)
	{
		if (!MVGameController.Instance.WOCM.Links.ContainsKey(linkID))
		{
			Debug.LogError((object)"RemoveLink event, but link not registered!");
			return;
		}
		Link link = MVGameController.Instance.WOCM.Links[linkID];
		MVGameController.Instance.WOCM.RemoveLink(link);
	}

	private void OnTriggerBoxEnterEvent(int actorNr, int worldObjectID)
	{
		if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)("OnTriggerBoxEnterEvent received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVTriggerBox.OnEnter(MVGameController.Instance.WOCM.Players[actorNr]);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxExitEvent(int actorNr, int worldObjectID)
	{
		if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)("OnTriggerBoxExitEvent received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVTriggerBox.OnExit(MVGameController.Instance.WOCM.Players[actorNr]);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxStayBegin(int worldObjectID, int actorNr)
	{
		if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVTriggerBox.OnStayBegin(actorNr);
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVPressurePlate)
		{
			MVPressurePlate mVPressurePlate = (MVPressurePlate)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVPressurePlate.OnStayBegin(actorNr);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxStayEnd(int worldObjectID)
	{
		if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError((object)("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVTriggerBox.OnStayEnd();
		}
		else if (MVGameController.Instance.WOCM.WorldObjects[worldObjectID] is MVPressurePlate)
		{
			MVPressurePlate mVPressurePlate = (MVPressurePlate)MVGameController.Instance.WOCM.WorldObjects[worldObjectID];
			mVPressurePlate.OnStayEnd();
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnAddItemToInventoryEvent(int actorNr, int itemID, int itemTypeID, string itemName, byte[] itemData, int slotIndex, int worldInventoryID)
	{
		if (MVGameController.Instance.WOCM.PlayerRepository.PlayerInventory.ContainsKey(itemID))
		{
			Debug.LogError((object)"Attempt to add item to inventory, but itemID already exists in inventory");
		}
		else if (actorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = itemID;
			mVItem.itemTypeID = itemTypeID;
			mVItem.name = itemName;
			mVItem.data = itemData;
			Debug.Log((object)("Data length: " + mVItem.data.Length));
			MVGameController.Instance.WOCM.PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
			MVGameController.Instance.WOCM.PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, slotIndex);
			MVGameController.Instance.WOCM.PlayerRepository.NotifyPlayerInventoryChange();
		}
		else
		{
			MVPrototype prototype = MVGameController.Instance.WOCM.WorldInventory.GetPrototype(worldInventoryID);
			if (prototype == null)
			{
				Debug.LogError((object)"Attempt to fetch prototype, but prototype does not exist!");
			}
			else
			{
				prototype.ItemID = itemID;
			}
		}
	}

	private void OnRemoveItemFromInventory(int itemID)
	{
		if (!MVGameController.Instance.WOCM.PlayerRepository.PlayerInventory.ContainsKey(itemID))
		{
			Debug.LogError((object)"Attempt to remove item to inventory, but itemID not in inventory");
		}
		else
		{
			MVGameController.Instance.WOCM.PlayerRepository.RemoveItem(itemID);
		}
	}

	private void OnChatMsgEvent(int actorNr, string chatMsg)
	{
		string sender = MVGameController.Instance.WOCM.Players[actorNr].Username;
		if (OnReceivedChatMessage != null)
		{
			OnReceivedChatMessage(sender, chatMsg);
		}
	}

	private void OnWoUniquePrototypeEvent(int woId, int worldInventoryId)
	{
		MVGameController.Instance.WOCM.WorldInventory.OnReplaceWoPrototype(woId, worldInventoryId);
	}

	private void OnGameStateChange(MVGameStateType gameStateType, int startTime, int duration, MVGameStateReason reason, int actorNr)
	{
		Debug.Log((object)string.Concat(new object[4] { "OnGameStateChange EVENT: ", gameStateType, ", reason: ", reason }));
		networkGameStateListener.ChangeState(this, gameStateType, startTime, duration, reason, actorNr);
	}

	private void AddPlayer(string username, int profileID, int actorNr, int planetOwnershipTypeID = 0)
	{
		if (MVGameController.Instance.WOCM.Players.ContainsKey(actorNr))
		{
			Debug.LogWarning((object)"Duplicate player");
			return;
		}
		MVPlayer mVPlayer = new MVPlayer();
		mVPlayer.ActorNr = actorNr;
		mVPlayer.ProfileID = profileID;
		mVPlayer.Username = username;
		mVPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		MVGameController.Instance.WOCM.Players.Add(actorNr, mVPlayer);
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	private void CreateLocalAvatar()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		MVSpawnPoint validSpawnPoint = MVGameController.Instance.WOCM.GetValidSpawnPoint();
		if (validSpawnPoint == null)
		{
			Debug.LogError((object)"No spawn-point found on planet!");
			RegisterWorldObject(WorldObjectType.Avatar, -1, new Hashtable(), new Hashtable(), MVGameController.Instance.WOCM.GetValidAvatarStartPosition(), Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: false);
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 100f);
		hashtable.Add("animation", "Idle");
		hashtable.Add("centerSlot", string.Empty);
		hashtable.Add("rightSlot", string.Empty);
		hashtable.Add("leftSlot", string.Empty);
		hashtable.Add("aboveSlot", string.Empty);
		hashtable.Add("isFiring", false);
		RegisterWorldObject(WorldObjectType.Avatar, -1, new Hashtable(), hashtable, validSpawnPoint.GameObject.transform.localPosition, validSpawnPoint.GameObject.transform.localRotation, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: false);
	}

	public void RequestDBQuery(DBQuery query, Hashtable inData)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)62, (byte)query);
		hashtable.Add((byte)64, inData);
		peer.OpCustom(106, hashtable, sendReliable: true);
	}

	public void OnDBQueryResponse(Hashtable outData)
	{
		if (outData == null)
		{
			Debug.LogError((object)"OnDBQueryResponse: outData is null");
		}
		else if (joinState == MVJoinState.FetchingItemTypes)
		{
			foreach (object key in outData.Keys)
			{
				MVGameController.Instance.WOCM.PlayerRepository.ItemTypes.Add((int)key, (string)outData[(int)key]);
			}
		}
		else if (joinState == MVJoinState.FetchingOwnershipTypes)
		{
			foreach (object key2 in outData.Keys)
			{
				MVGameController.Instance.WOCM.PlayerRepository.PlanetOwnershipTypes.Add((int)key2, (string)outData[(int)key2]);
			}
		}
		if (joinState != MVJoinState.Playing)
		{
			GotoNextJoinState();
		}
	}

	public void OnDBQueryFailed(DBReasonCode reason)
	{
		Debug.Log((object)("DBQuery failed during GameState '" + joinState.ToString() + "'. Reason: " + reason));
		if (joinState != MVJoinState.Playing)
		{
			GotoNextJoinState();
		}
	}

	public void RequestLargeDBQuery(DBQuery query, Hashtable inData, int numRowsPerReturn)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)62, (byte)query);
		hashtable.Add((byte)64, inData);
		hashtable.Add((byte)68, numRowsPerReturn);
		peer.OpCustom(107, hashtable, sendReliable: true);
	}

	public void OnLargeDBQueryResponse(int largeDBQueryID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)67, largeDBQueryID);
		peer.OpCustom(108, hashtable, sendReliable: true);
	}

	public void OnGetNextResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		if (joinState != MVJoinState.FetchingInventory)
		{
			return;
		}
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.itemTypeID = (int)((Hashtable)outData[key])[(byte)17];
			mVItem.name = (string)((Hashtable)outData[key])[(byte)12];
			mVItem.data = (byte[])((Hashtable)outData[key])[(byte)13];
			MVGameController.Instance.WOCM.PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
			int num2 = (int)((Hashtable)outData[key])[(byte)23];
			MVGameController.Instance.WOCM.PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
		}
		if (isDone)
		{
			GotoNextJoinState();
		}
		else
		{
			OnLargeDBQueryResponse(largeQueryId);
		}
		MVGameController.Instance.WOCM.PlayerRepository.NotifyPlayerInventoryChange();
	}

	public void OnAddPrototypeToInventoryResponse(int returnCode, int price, int itemID, int prototypeID)
	{
		bool flag = false;
		string text = string.Empty;
		switch (returnCode)
		{
		case 0:
			text = "Successfully added model\nto your inventory";
			break;
		case -1:
			text = "Undefined error during\nadd to inventory";
			break;
		case -5:
			text = "Failed to add to inventory";
			break;
		case -4:
			text = "Failed to create item";
			break;
		case -3:
			text = "You are not the creator\nof this model.\n\nYou can only add models\n to your inventory,\nif they are created by you";
			break;
		case -6:
			text = "You are not the creator\nof this model.\n\nYou need to buy the model\nTo add it to your Inventory\nPrice: " + price + " silver";
			flag = true;
			break;
		case -7:
			text = "The item is already in\nyour inventory";
			break;
		case -2:
			text = "Prototype not found";
			break;
		}
		if (!flag)
		{
			guiManager.ShowMessageBox(text);
		}
		else
		{
			guiManager.ShowPurchaseItemBox(text, price, itemID, prototypeID);
		}
	}

	public void OnAddItemToQuickSlotResponse(bool success)
	{
	}

	public void OnRemoveItemFromQuickSlotResponse(bool success)
	{
	}

	public void OnUpdateQuickSlotResponse(bool success)
	{
	}

	public void OnRequestQuickSlotResponse(Hashtable returnValues, bool success)
	{
		GotoNextJoinState();
	}

	public void OperationResult(byte opCode, int returnCode, Hashtable returnValues, short invocID)
	{
		logger.Log($"OperationResult OpCode: {(MVOperationCodes)opCode} Return Code: {returnCode}.");
		if (returnCode != 0)
		{
		}
		switch (opCode)
		{
		case 90:
			connState = MVConnState.Joined;
			switch (returnCode)
			{
			case 0:
				OnJoinResponse(returnValues);
				return;
			case -4:
				guiManager.ShowMessageBox("Planet '" + planetName + "'\n recently disposed on server");
				break;
			case -3:
				guiManager.ShowMessageBox("You are not authorized to join\nplanet '" + planetName + "' in edit mode");
				break;
			case -2:
			{
				string text = "The planet '" + planetName + "' was not found\n";
				if (!editorMode)
				{
					text += "among Published Planets";
				}
				guiManager.ShowMessageBox(text);
				break;
			}
			case -6:
				guiManager.ShowMessageBox("The planet '" + planetName + " failed\nduring load");
				break;
			case -5:
				guiManager.ShowMessageBox("The profile has already joined the planet '" + planetName + "'");
				break;
			}
			peer.Disconnect();
			break;
		case 91:
			connState = MVConnState.Disconnecting;
			peer.Disconnect();
			break;
		case 129:
			switch (returnCode)
			{
			case 0:
				guiManager.ShowMessageBox("Successfully published planet");
				break;
			case -1:
				guiManager.ShowMessageBox("Undefined fail duing Publish...");
				break;
			case -2:
				guiManager.ShowMessageBox("You are not authorized to\nPublish this planet");
				break;
			default:
				guiManager.ShowMessageBox("Unhandled returnCode duing PublishPlanet...");
				break;
			}
			break;
		case 95:
			peer.DeriveSharedKey((byte[])returnValues[(byte)17]);
			break;
		case 105:
			if (joinState == MVJoinState.SettingActorReady)
			{
				GotoNextJoinState();
			}
			else
			{
				Debug.LogError((object)"SetActorReady returned, but we're not in SettingActorReadyState");
			}
			break;
		case 109:
			OnRequestUserListResponse((Hashtable)returnValues[(byte)75]);
			break;
		case 137:
			OnRequestFriendsResponse((Hashtable)returnValues[(byte)122]);
			break;
		case 110:
			OnRequestWorldObjectsResponse((int)returnValues[(byte)88], (int)returnValues[(byte)83]);
			break;
		case 117:
			OnGetNextWOBatchResponse(returnValues);
			break;
		case 111:
			OnRegisterWorldObjectResponse((int)returnValues[(byte)81], returnCode == 0);
			break;
		case 112:
			OnUnregisterWorldObjectResponse(returnCode == 0);
			break;
		case 113:
			break;
		case 153:
			if (returnCode != 0)
			{
				Debug.LogError((object)"UpdateWorldObjectRunTimeData FAILED on server!");
			}
			break;
		case 116:
			OnTransferOwnershipResponse(returnValues, returnCode);
			break;
		case 121:
			OnRequestPrototypesResponse((int)returnValues[(byte)115], (int)returnValues[(byte)116]);
			break;
		case 122:
			OnGetNextPrototypeBatchResponse(returnValues);
			break;
		case 106:
			if ((byte)returnValues[(byte)65] == 0)
			{
				OnDBQueryResponse((Hashtable)returnValues[(byte)63]);
			}
			else
			{
				OnDBQueryFailed((DBReasonCode)(byte)returnValues[(byte)66]);
			}
			break;
		case 107:
			if (returnCode == 0)
			{
				OnLargeDBQueryResponse((int)returnValues[(byte)67]);
				break;
			}
			Debug.LogError((object)"LargeDBQuery response failed...");
			if (joinState != MVJoinState.Playing)
			{
				GotoNextJoinState();
			}
			break;
		case 108:
			OnGetNextResultSetResponse((Hashtable)returnValues[(byte)63], (int)returnValues[(byte)67], !(bool)returnValues[(byte)69]);
			break;
		case 130:
			OnAddPrototypeToInventoryResponse(returnCode, (int)returnValues[(byte)144], (int)returnValues[(byte)105], (int)returnValues[(byte)112]);
			break;
		case 133:
			OnAddItemToQuickSlotResponse(returnCode == 0);
			break;
		case 134:
			OnRemoveItemFromQuickSlotResponse(returnCode == 0);
			break;
		case 135:
			OnUpdateQuickSlotResponse(returnCode == 0);
			break;
		case 136:
			OnRequestQuickSlotResponse(returnValues, returnCode == 0);
			break;
		case 138:
			OnRequestFriendshipResponse(returnCode);
			break;
		case 139:
			OnRequestFriendshipResponse(returnCode);
			break;
		case 144:
			OnUngroupResponse(returnCode == 0);
			break;
		case 145:
			OnLockHierarchyResponse(returnValues, returnCode);
			break;
		case 149:
			if (returnCode != 0)
			{
				OnRequestWoUniquePrototypeFailed(returnValues);
			}
			break;
		case 126:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"AddLink FAILED");
				MVGameController.Instance.WOCM.HandleAddLinkResponse(success: false, (int)returnValues[(byte)130]);
			}
			else
			{
				MVGameController.Instance.WOCM.HandleAddLinkResponse(success: true, (int)returnValues[(byte)130]);
			}
			break;
		case 127:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"RemoveLink FAILED");
				MVGameController.Instance.WOCM.HandleRemoveLinkResponse(success: false);
			}
			else
			{
				MVGameController.Instance.WOCM.HandleRemoveLinkResponse(success: true);
			}
			break;
		case 128:
			OnRequestLinksResponse((int[])returnValues[(byte)131]);
			break;
		case 154:
			OnPurchaseItemResponse(returnCode);
			break;
		case 92:
		case 93:
		case 94:
		case 96:
		case 97:
		case 98:
		case 99:
		case 100:
		case 101:
		case 102:
		case 103:
		case 104:
		case 114:
		case 115:
		case 118:
		case 119:
		case 120:
		case 123:
		case 124:
		case 125:
		case 131:
		case 132:
		case 140:
		case 141:
		case 142:
		case 143:
		case 146:
		case 147:
		case 148:
		case 150:
		case 151:
		case 152:
			break;
		}
	}

	public void EventAction(byte eventCode, Hashtable photonEvent)
	{
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
		switch (eventCode)
		{
		case 90:
		{
			string text2 = (string)photonEvent[(byte)71];
			int profileID3 = (int)photonEvent[(byte)73];
			int num2 = (int)photonEvent[(byte)9];
			if (num2 != MVGameController.Instance.WOCM.LocalPlayerActorNumber)
			{
				AddPlayer(text2, profileID3, num2);
			}
			break;
		}
		case 91:
		{
			int num3 = (int)photonEvent[(byte)9];
			if (num3 != MVGameController.Instance.WOCM.LocalPlayer.ActorNr)
			{
				MVGameController.Instance.WOCM.Players.Remove(num3);
			}
			else
			{
				Debug.Log((object)"Local player leave event");
			}
			break;
		}
		case 93:
			OnRegisterWorldObjectEvent(photonEvent);
			break;
		case 94:
			logger.Log("UnregisterWorldObject event...");
			OnUnregisterWorldObjectEvent((int)photonEvent[(byte)81]);
			break;
		case 95:
			OnUpdateWorldObjectEvent(photonEvent);
			break;
		case 98:
			OnUpdateNetworkInputEvent(photonEvent);
			break;
		case 97:
			OnTransferOwnershipEvent((int)photonEvent[(byte)81], (int)photonEvent[(byte)79]);
			break;
		case 99:
			OnRegisterPrototypeEvent((int)photonEvent[(byte)112], (int)photonEvent[(byte)105], (int)photonEvent[(byte)106], (string)photonEvent[(byte)113], (Hashtable)photonEvent[(byte)114], (float)photonEvent[(byte)99], (int)photonEvent[(byte)9]);
			break;
		case 100:
			OnUnregisterPrototypeEvent((int)photonEvent[(byte)112]);
			break;
		case 101:
			MVGameController.Instance.WOCM.OnUpdatePrototypeEvent((int)photonEvent[(byte)112], (byte[])photonEvent[(byte)114]);
			break;
		case 102:
			MVGameController.Instance.WOCM.OnUpdatePrototypeScaleEvent((int)photonEvent[(byte)112], (float)photonEvent[(byte)99]);
			break;
		case 96:
			MVGameController.Instance.WOCM.OnUpdateWorldObjectDataEvent((int)photonEvent[(byte)81], (Hashtable)photonEvent[(byte)78]);
			break;
		case 123:
			if ((int)photonEvent[(byte)9] != MVGameController.Instance.WOCM.LocalPlayer.ActorNr)
			{
				MVGameController.Instance.WOCM.OnUpdateWorldObjectRunTimeDataEvent((int)photonEvent[(byte)81], (Hashtable)photonEvent[(byte)145]);
			}
			break;
		case 103:
			MVGameController.Instance.WOCM.OnUpdateTerrainEvent((int)photonEvent[(byte)81], (byte[])photonEvent[(byte)126]);
			break;
		case 104:
			logger.Log("Add link event...");
			OnAddLinkEvent((int)photonEvent[(byte)129], (int)photonEvent[(byte)128], (int)photonEvent[(byte)130]);
			break;
		case 105:
			logger.Log("Remove link event...");
			OnRemoveLinkEvent((int)photonEvent[(byte)130]);
			break;
		case 106:
			logger.Log("Add item to inventory event...");
			OnAddItemToInventoryEvent((int)photonEvent[(byte)9], (int)photonEvent[(byte)105], (int)photonEvent[(byte)106], (string)photonEvent[(byte)107], (byte[])photonEvent[(byte)108], (int)photonEvent[(byte)110], (int)photonEvent[(byte)112]);
			break;
		case 107:
			logger.Log("Remove item from inventory event...");
			OnRemoveItemFromInventory((int)photonEvent[(byte)105]);
			break;
		case 108:
		{
			int friendID2 = (int)photonEvent[(byte)123];
			int profileID2 = (int)photonEvent[(byte)73];
			int friendProfileID = (int)photonEvent[(byte)124];
			OnFriendRequestEvent(friendID2, profileID2, friendProfileID);
			break;
		}
		case 109:
		{
			int friendID = (int)photonEvent[(byte)123];
			int profileID = (int)photonEvent[(byte)73];
			FriendStatus status = (FriendStatus)(int)photonEvent[(byte)125];
			OnFriendUpdateEvent(friendID, profileID, status);
			break;
		}
		case 110:
		{
			int worldObjectID4 = (int)photonEvent[(byte)81];
			int actorNr3 = (int)photonEvent[(byte)9];
			OnTriggerBoxEnterEvent(actorNr3, worldObjectID4);
			break;
		}
		case 111:
		{
			int worldObjectID3 = (int)photonEvent[(byte)81];
			int actorNr2 = (int)photonEvent[(byte)9];
			OnTriggerBoxExitEvent(actorNr2, worldObjectID3);
			break;
		}
		case 112:
		{
			int worldObjectID2 = (int)photonEvent[(byte)81];
			int actorNr = (int)photonEvent[(byte)9];
			OnTriggerBoxStayBegin(worldObjectID2, actorNr);
			break;
		}
		case 113:
		{
			int worldObjectID = (int)photonEvent[(byte)81];
			OnTriggerBoxStayEnd(worldObjectID);
			break;
		}
		case 114:
			Debug.Log((object)"Ungroup event...");
			OnUngroupEvent(photonEvent);
			break;
		case 116:
			Debug.Log((object)"LockHierarchy event...");
			OnLockHierarchyEvent(photonEvent);
			break;
		case 118:
			OnChatMsgEvent((int)photonEvent[(byte)9], (string)photonEvent[(byte)138]);
			break;
		case 119:
			Debug.Log((object)"WoUniquePrototypeEvent");
			OnWoUniquePrototypeEvent((int)photonEvent[(byte)81], (int)photonEvent[(byte)112]);
			break;
		case 120:
			OnGameStateChange((MVGameStateType)(int)photonEvent[(byte)139], (int)photonEvent[(byte)141], (int)photonEvent[(byte)140], (MVGameStateReason)(int)photonEvent[(byte)142], (int)photonEvent[(byte)9]);
			break;
		case 92:
		{
			int num = (int)photonEvent[(byte)10];
			Hashtable hashtable = (Hashtable)photonEvent[(byte)12];
			Debug.Log((object)("ACTOR-NR: " + num));
			Debug.Log((object)MVGameController.Instance.WOCM.Players[num].Username);
			{
				foreach (string key in hashtable.Keys)
				{
					Debug.Log((object)(key + ": " + hashtable[key]));
				}
				break;
			}
		}
		case 121:
			OnSyncAvatarStatusEvent((int)photonEvent[(byte)9], (Hashtable)photonEvent[(byte)143]);
			break;
		case 122:
			OnResetLogicChunkEvent((int)photonEvent[(byte)81]);
			break;
		case 124:
			OnPickupItemStateChangeEvent((PickupItemState)(int)photonEvent[(byte)146], (int)photonEvent[(byte)81], (int)photonEvent[(byte)9]);
			break;
		case 125:
			OnRemoveCubesWithinRadiusEvent((float)photonEvent[(byte)148], new Vector3((float)photonEvent[(byte)89], (float)photonEvent[(byte)90], (float)photonEvent[(byte)91]), (float)photonEvent[(byte)155], (DamageFallOffType)(int)photonEvent[(byte)156]);
			break;
		case 126:
			OnUpdateLineOfFire(camOrigin: new Vector3((float)photonEvent[(byte)149], (float)photonEvent[(byte)150], (float)photonEvent[(byte)151]), camDir: new Vector3((float)photonEvent[(byte)152], (float)photonEvent[(byte)153], (float)photonEvent[(byte)154]), actorNr: (int)photonEvent[(byte)9]);
			break;
		case 115:
		case 117:
			break;
		}
	}

	public void PeerStatusCallback(StatusCode returnCode)
	{
		switch (returnCode)
		{
		case StatusCode.Connect:
			JoinGame();
			break;
		case StatusCode.Disconnect:
			connState = MVConnState.Disconnected;
			break;
		case StatusCode.Exception:
			connState = MVConnState.Exception;
			break;
		case StatusCode.SendError:
			connState = MVConnState.SendError;
			break;
		case StatusCode.TimeoutDisconnect:
			connState = MVConnState.TimeoutDisconnect;
			break;
		default:
			Debug.LogWarning((object)("Unhandled PeerStatusCallback, returnCode: " + returnCode));
			break;
		}
	}

	public void DebugReturn(DebugLevel level, string debug)
	{
		Debug.Log((object)("Photon DebugReturn: " + debug));
	}
}
