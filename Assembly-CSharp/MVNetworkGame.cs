using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVNetworkGame : IPhotonPeerListener
{
	private class GameDataQueryManager
	{
		private class GameDataQuery
		{
			private BytePacker bp;

			private int instigatorActorNumber;

			public QueryType QueryType { get; private set; }

			public int InstigatorActorNumber => instigatorActorNumber;

			public GameDataQuery(BytePacker bp, int instigatorActorNumber, QueryType queryType)
			{
				this.bp = bp;
				this.instigatorActorNumber = instigatorActorNumber;
				QueryType = queryType;
			}

			public override string ToString()
			{
				return $"[GameDataQuery: InstigatorActorNumber={InstigatorActorNumber}, BPLength={bp.Length}]";
			}

			public void AddGameDataQuery(GameDataQuery gameDataQuery)
			{
				bp.Position = bp.Length;
				bp.Write(gameDataQuery.GetBytePacker().ToArray());
			}

			public BytePacker GetBytePacker()
			{
				bp.Position = 0;
				return bp;
			}
		}

		private readonly Dictionary<int, GameDataQuery> gameDataQueries = new Dictionary<int, GameDataQuery>();

		public void HandleDataBatch(int instigator, int queryId, QueryType queryType, bool queryDataLeft, BytePacker bp)
		{
			GameDataQuery gameDataQuery = new GameDataQuery(bp, instigator, queryType);
			if (queryId != -1)
			{
				OnGetGameBatch(queryId, gameDataQuery);
				if (queryDataLeft)
				{
					Debug.Log((object)"GameQueryDataLeft: Request next batch");
					MVGameController.Instance.Game.RequestGetNextGameBatch(queryId);
				}
				else
				{
					Debug.Log((object)"No data left, wait for done event");
				}
			}
			else
			{
				InitializeGameQueryData(gameDataQuery);
			}
		}

		public void OnGameQueryReady(int queryId)
		{
			GameDataQuery gameDataQuery = gameDataQueries[queryId];
			InitializeGameQueryData(gameDataQuery);
		}

		private void OnGetGameBatch(int queryId, GameDataQuery gameDataQuery)
		{
			Debug.Log((object)("OnGetGameBatch: " + gameDataQuery.ToString()));
			if (gameDataQueries.ContainsKey(queryId))
			{
				gameDataQueries[queryId].AddGameDataQuery(gameDataQuery);
			}
			else
			{
				gameDataQueries.Add(queryId, gameDataQuery);
			}
		}

		private void InitializeGameQueryData(GameDataQuery gameDataQuery)
		{
			Debug.Log((object)$"GameDataQuery {gameDataQuery.QueryType}");
			switch (gameDataQuery.QueryType)
			{
			case QueryType.GameWorld:
				MVGameController.Instance.Game.worldNetwork.AddGameQueryDataToGameWorld(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
				break;
			case QueryType.Item:
				if (MVGameController.Instance.Game.ReceivedItemFromQuery != null)
				{
					ReceivedItemFromQueryEventArgs e = new ReceivedItemFromQueryEventArgs(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
					MVGameController.Instance.Game.ReceivedItemFromQuery(this, e);
				}
				break;
			}
		}
	}

	public delegate void OnPlayerListChangedDelegate();

	public delegate void OnGetNextResultSetResponseDelegate(Hashtable outData, int largeQueryId, bool isDone);

	public delegate void OnReceivedChatMessageDelegate(MVPlayer sender, string message);

	public delegate void OnReceivedGameMsgDelegate(MVGameMsgType type, Hashtable gameMsgData);

	public delegate void OnReceivedXPDelegate(int amount);

	public delegate void OnMarketPlaceActionCompleteDelegate(bool success);

	private const string appName = "MVGameServer";

	public const int numInventoryItemsPerBatch = 10;

	private const float ExpirationStateCheckPeriod = 1f;

	private const float ExpirationDataFetchCheckPeriod = 15f;

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(MVNetworkGame));

	private LitePeer peer;

	private MVConnState connState;

	private MVJoinState _joinState;

	private MVItemBusinessLogic itemBusinessLogic = new MVItemBusinessLogic();

	private bool isPublished;

	private OperationResponsePendingManager operationResponsePendingManager;

	private GameDataQueryManager gameDataQueryManager = new GameDataQueryManager();

	private ItemCategories itemCategories;

	private MVNetworkGameStateListener networkGameStateListener;

	private int lastFrameServerTimeUpdate = -1;

	private int lastFrameLocalTimeUpdate = -1;

	private int serverTimeInMilliseconds;

	private int localTimeInMilliseconds;

	private DateTime dbTimeBase;

	private int localTimeInMillisecondsOnDBTimeSync;

	public Action<int, Hashtable> PurchaseProductResponseHandler;

	public Action<bool> OnSetAvatarAccessoryResponse;

	public OnPlayerListChangedDelegate onPlayerListChanged;

	public OnGetNextResultSetResponseDelegate OnGetNextResultSetResponse;

	public OnReceivedChatMessageDelegate OnReceivedChatMessage;

	public OnReceivedGameMsgDelegate OnReceivedGameMsg;

	public OnReceivedXPDelegate OnReceivedXP;

	public OnMarketPlaceActionCompleteDelegate OnMarketPlaceActionComplete;

	private float lastExpirationStateCheck;

	private float lastDataFetchCheck;

	private HashSet<int> ownedRequestedIDs = new HashSet<int>();

	private HashSet<int> ownedRequestFailedIDs = new HashSet<int>();

	private HashSet<int> expiredStreamingAssetIDs = new HashSet<int>();

	private MVLocalObjectController playerController;

	private PlayerRepository playerRepository;

	private ShopRepository shopRepository;

	private ShopRepository avatarShopRepository;

	private Dictionary<int, MVPlayer> players;

	private MVTeamManager teamManager = new MVTeamManager();

	private FriendList friends;

	private MVMaterialRepository materialRepository;

	private int localPlayerActorNumber = -1;

	private WorldNetwork worldNetwork;

	public MVItemBusinessLogic ItemBusinessLogic => itemBusinessLogic;

	public ItemCategories ItemCategories => itemCategories;

	public bool TouristChatAllowed { get; private set; }

	public bool IsDebugMode { get; private set; }

	public MVConnState ConnState
	{
		get
		{
			return connState;
		}
		set
		{
			if (connState != MVConnState.HandlingException || (value != MVConnState.Exception && value != MVConnState.SendError && value != MVConnState.TimeoutDisconnect))
			{
				connState = value;
			}
		}
	}

	public bool IsPlaying => MVGameController.Instance.GameMode == MVGameMode.Play || (MVGameController.Instance.GameMode == MVGameMode.Edit && MVGameController.Instance.EditorController != null && MVGameController.Instance.EditorController.PlayInEditor);

	public MVJoinState JoinState
	{
		get
		{
			return _joinState;
		}
		private set
		{
			_joinState = value;
		}
	}

	public MVNetworkGameStateListener NetworkGameStateListener => networkGameStateListener;

	public LitePeer Peer => peer;

	public MVGameMode GameMode => MVGameController.Instance.GameMode;

	public int ServerTimeInMilliSeconds
	{
		get
		{
			if (lastFrameServerTimeUpdate != Time.frameCount)
			{
				serverTimeInMilliseconds = Peer.ServerTimeInMilliSeconds;
				lastFrameServerTimeUpdate = Time.frameCount;
			}
			return serverTimeInMilliseconds;
		}
	}

	public int LocalTimeInMilliSeconds
	{
		get
		{
			if (lastFrameLocalTimeUpdate != Time.frameCount)
			{
				localTimeInMilliseconds = SupportClass.GetTickCount();
				lastFrameLocalTimeUpdate = Time.frameCount;
			}
			return localTimeInMilliseconds;
		}
	}

	public DateTime DBTime => dbTimeBase.AddMilliseconds(LocalTimeInMilliSeconds - localTimeInMillisecondsOnDBTimeSync);

	public Dictionary<int, StreamingAssetInfo> StreamingAssetInfoMap { get; private set; }

	public StreamingAssetShopInventory StreamingAssetShopInventory { get; private set; }

	public StreamingAssetInventory StreamingAssetInventory { get; private set; }

	public InventoryExpirationChecker StreamingAssetExpirationChecker { get; private set; }

	public MVPlayer LocalPlayer
	{
		get
		{
			players.TryGetValue(localPlayerActorNumber, out var value);
			return value;
		}
	}

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

	public MVMaterialRepository MaterialRepository => materialRepository;

	public PlayerRepository PlayerRepository => playerRepository;

	public ShopRepository ShopRepository => shopRepository;

	public ShopRepository AvatarShopRepository => avatarShopRepository;

	public MVTeamManager TeamManager => teamManager;

	public AssetBundleMgr AssetBundleMgr { get; private set; }

	public MVGameModeChangeNotifier GameStateController { get; private set; }

	public MVCameraController CameraController { get; set; }

	public FriendList Friends => friends;

	public MVLocalObjectController PlayerController => playerController;

	public Dictionary<int, MVPlayer> Players => players;

	public MVWorldObjectClientManager WorldObjectClientManager
	{
		get
		{
			if (worldNetwork == null)
			{
				return null;
			}
			return worldNetwork.WorldObjectClientManager;
		}
	}

	public World World => worldNetwork;

	public event EventHandler<ScreenshotUploadedEventArgs> ScreenshotUploaded;

	public event EventHandler<ProductsExpiringEventArgs> StreamingAssetsExpired;

	public event EventHandler<ReceivedItemFromQueryEventArgs> ReceivedItemFromQuery;

	public MVNetworkGame()
	{
		peer = new LitePeer(this, ConnectionProtocol.Udp);
		peer.DisconnectTimeout = 60000;
		peer.DebugOut = DebugLevel.WARNING;
		operationResponsePendingManager = new OperationResponsePendingManager(peer);
		networkGameStateListener = new MVNetworkGameStateListener();
		networkGameStateListener.OnGameStateChanged += networkGameStateListener_OnGameStateChanged;
		StreamingAssetExpirationChecker = new InventoryExpirationChecker();
		StreamingAssetInfoMap = new Dictionary<int, StreamingAssetInfo>();
		StreamingAssetShopInventory = new StreamingAssetShopInventory();
		StreamingAssetInventory = new StreamingAssetInventory(StreamingAssetExpirationChecker);
	}

	private void networkGameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		if (JoinState == MVJoinState.Playing)
		{
			MVGameStateType currentGameState = networkGameStateListener.CurrentGameState;
			if (currentGameState == MVGameStateType.Round)
			{
				LocalPlayer.Reset();
				worldNetwork.WorldObjectClientManagerNetwork.ResetWorld();
			}
		}
	}

	public void Update()
	{
		if (Application.isEditor)
		{
			UpdateGame();
			return;
		}
		try
		{
			UpdateGame();
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Exception in update loop: " + ex.ToString()));
			Debug.LogError((object)("JoinState: " + JoinState));
			Debug.Log((object)ex.StackTrace);
			throw ex;
		}
	}

	public void Cleanup()
	{
		if (worldNetwork != null && worldNetwork.WorldObjectClientManagerNetwork != null)
		{
			worldNetwork.WorldObjectClientManagerNetwork.Cleanup();
		}
	}

	private void UpdateGame()
	{
		if (peer != null)
		{
			peer.Service();
			if (JoinState == MVJoinState.Playing)
			{
				CheckStreamingAsssetExpiration();
				worldNetwork.Update(this);
				AssetBundleMgr.Update();
			}
			networkGameStateListener.Update(this);
		}
	}

	private void GotoNextJoinState()
	{
		switch (JoinState)
		{
		case MVJoinState.Joining:
			Debug.Log((object)("Profile ID " + LocalPlayer.Username));
			JoinState = MVJoinState.SynchronizingGameTime;
			GetCreditStatus();
			break;
		case MVJoinState.SynchronizingGameTime:
			peer.OpCustom(68, new Dictionary<byte, object>(), sendReliable: true);
			JoinState = MVJoinState.FetchingCreditStatus;
			break;
		case MVJoinState.FetchingCreditStatus:
		{
			JoinState = MVJoinState.FetchingMaterials;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, MVGameController.Instance.ProfileID);
			peer.OpCustom(50, dictionary, sendReliable: true);
			break;
		}
		case MVJoinState.FetchingMaterials:
			JoinState = MVJoinState.FetchingItemTypes;
			RequestDBQuery(DBQuery.RequestItemCategories, new Hashtable());
			break;
		case MVJoinState.FetchingItemTypes:
			JoinState = MVJoinState.FetchingOwnershipTypes;
			RequestDBQuery(DBQuery.RequestPlanetOwnershipTypes, new Hashtable());
			break;
		case MVJoinState.FetchingOwnershipTypes:
			JoinState = MVJoinState.FetchingStreamingAssets;
			RequestStreamingAssetList(StreamingAssetType.AmbientAudio, StreamingAssetType.AvatarAccessory);
			break;
		case MVJoinState.FetchingStreamingAssets:
			JoinState = MVJoinState.FetchingStreamingAssetInventory;
			if (GameMode == MVGameMode.Edit)
			{
				RequestStreamingAssetInventory(StreamingAssetType.AmbientAudio);
			}
			else if (GameMode == MVGameMode.CharacterEditor)
			{
				RequestStreamingAssetInventory(StreamingAssetType.AvatarAccessory);
			}
			else
			{
				GotoNextJoinState();
			}
			break;
		case MVJoinState.FetchingStreamingAssetInventory:
			if (GameMode == MVGameMode.Edit)
			{
				JoinState = MVJoinState.FetchingInventory;
				OnGetNextResultSetResponse = OnInventoryResultSetResponse;
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add((byte)0, LocalPlayer.ProfileID);
				RequestLargeDBQuery(DBQuery.RequestInventory, hashtable2, 10);
			}
			else if (GameMode == MVGameMode.CharacterEditor)
			{
				JoinState = MVJoinState.FetchingAvatarShopInventory;
				OnGetNextResultSetResponse = OnAvatarShopInventoryResultSetResponse;
				Hashtable inData = new Hashtable();
				RequestLargeDBQuery(DBQuery.RequestAvatarShopInventory, inData, 25);
			}
			else
			{
				FetchGameSnapshot();
			}
			break;
		case MVJoinState.FetchingInventory:
			if (GameMode == MVGameMode.Edit)
			{
				JoinState = MVJoinState.FetchingShopInventory;
				OnGetNextResultSetResponse = OnShopInventoryResultSetResponse;
				Hashtable hashtable = new Hashtable();
				hashtable.Add((byte)0, LocalPlayer.ProfileID);
				RequestLargeDBQuery(DBQuery.RequestClientShopInventory, hashtable, 10);
			}
			else
			{
				FetchGameSnapshot();
			}
			break;
		case MVJoinState.FetchingAvatarShopInventory:
			FetchGameSnapshot();
			break;
		case MVJoinState.FetchingShopInventory:
			JoinState = MVJoinState.FetchingBuiltInItems;
			peer.OpCustom(67, new Dictionary<byte, object>(), sendReliable: true);
			break;
		case MVJoinState.FetchingBuiltInItems:
			FetchGameSnapshot();
			break;
		case MVJoinState.FetchingGameSnapShot:
			JoinState = MVJoinState.FetchingFriends;
			peer.OpCustom(20, new Dictionary<byte, object>(), sendReliable: true);
			break;
		case MVJoinState.FetchingFriends:
			JoinState = MVJoinState.FetchingTeamList;
			RequestTeamList();
			break;
		case MVJoinState.FetchingTeamList:
			JoinState = MVJoinState.SelectingTeam;
			SelectTeamFromJoinFlow();
			break;
		case MVJoinState.SelectingTeam:
			JoinState = MVJoinState.SettingTeam;
			SetTeamFromJoinFlow();
			break;
		case MVJoinState.SettingTeam:
			JoinState = MVJoinState.SettingActorReady;
			WorldObjectClientManager.AvatarLocal.Respawn();
			CameraController.GetCamera<JetPackCamera>().SetCameraToAvatarEulerHack();
			SetActorReady();
			break;
		case MVJoinState.SettingActorReady:
			JoinState = MVJoinState.FetchingActiveAvatar;
			GotoNextJoinState();
			break;
		case MVJoinState.FetchingActiveAvatar:
			JoinState = MVJoinState.Playing;
			if (GameMode == MVGameMode.CharacterEditor)
			{
				peer.OpCustom(65, new Dictionary<byte, object>(), sendReliable: true);
			}
			else
			{
				GotoNextJoinState();
			}
			break;
		case MVJoinState.Playing:
			break;
		case MVJoinState.CreatingAvatar:
		case MVJoinState.Leaving:
			break;
		}
	}

	public bool Join()
	{
		ConnState = MVConnState.Connecting;
		try
		{
			if (!peer.Connect(MVGameController.Instance.GameSessionData.Ip, "MVGameServer"))
			{
				Debug.LogError((object)"Not able to connect. Internet connection available?");
				return false;
			}
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
		if (ConnState == MVConnState.Joined)
		{
			ConnState = MVConnState.Leaving;
			JoinState = MVJoinState.Leaving;
			peer.OpLeave();
		}
		else
		{
			ConnState = MVConnState.Disconnected;
			JoinState = MVJoinState.Leaving;
			peer.Disconnect();
		}
	}

	private void CheckStreamingAsssetExpiration()
	{
		bool flag = lastExpirationStateCheck <= Time.realtimeSinceStartup / 1f;
		bool flag2 = lastDataFetchCheck <= Time.realtimeSinceStartup / 15f;
		if (flag)
		{
			lastExpirationStateCheck++;
			StreamingAssetExpirationChecker.CheckExpiration();
		}
		HashSet<int> iDs = StreamingAssetInventory.GetIDs();
		HashSet<int> hashSet = new HashSet<int>(iDs);
		HashSet<int> hashSet2 = null;
		HashSet<int> hashSet3 = null;
		HashSet<int> hashSet4 = null;
		MVBody body = WorldObjectClientManager.AvatarLocal.Body;
		if (flag2 || StreamingAssetExpirationChecker.ContainsExpired())
		{
			lastDataFetchCheck++;
			if (GameMode != MVGameMode.CharacterEditor && body.AccessoriesLoaded)
			{
				hashSet2 = body.GetAccessoryIDs();
				hashSet.UnionWith(hashSet2);
				hashSet3 = new HashSet<int>(hashSet2);
				hashSet3.ExceptWith(iDs);
				hashSet3.ExceptWith(ownedRequestedIDs);
			}
			hashSet4 = StreamingAssetExpirationChecker.GetExpiringInfoIDs(StreamingAssetInfo.ExpiringFetchThreshold);
			if (hashSet3 != null && hashSet3.Count != 0)
			{
				hashSet3.IntersectWith(hashSet4);
				hashSet3.ExceptWith(ownedRequestFailedIDs);
				if (hashSet3.Count != 0)
				{
					RequestStreamingAssetInventoryItems(hashSet3.ToArray());
					ownedRequestedIDs.UnionWith(hashSet3);
				}
			}
			if (hashSet4.Count == 0)
			{
			}
		}
		if (hashSet4 == null || hashSet4.Count == 0 || ownedRequestedIDs.Count != 0 || StreamingAssetsExpired == null)
		{
			return;
		}
		List<InventoryExpirationInfo> list = new List<InventoryExpirationInfo>();
		foreach (int item in hashSet4)
		{
			InventoryExpirationInfo expirationInfo = StreamingAssetExpirationChecker.GetExpirationInfo(item);
			if (expirationInfo.IsExpired && !expiredStreamingAssetIDs.Contains(expirationInfo.InventoryID))
			{
				expiredStreamingAssetIDs.Add(expirationInfo.InventoryID);
				list.Add(expirationInfo);
				expirationInfo.Renewed += InventoryExpirationInfo_Renewed;
			}
		}
		if (0 < list.Count)
		{
			Debug.Log((object)("Expired assets " + list.BuildString(eachEntryNewLine: false)));
			ProductsExpiringEventArgs e = new ProductsExpiringEventArgs(list);
			StreamingAssetsExpired(this, e);
		}
	}

	private void InventoryExpirationInfo_Renewed(object sender, ProductRenewedEventArgs e)
	{
		expiredStreamingAssetIDs.Remove(e.InvetoryID);
	}

	private void FetchGameSnapshot()
	{
		JoinState = MVJoinState.FetchingGameSnapShot;
		Hashtable hashtable = new Hashtable();
		hashtable.Add("ownerUserName", LocalPlayer.Username);
		hashtable.Add("actorNr", LocalPlayer.ActorNr);
		Hashtable value = hashtable;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(16, value);
		peer.OpCustom(73, dictionary, sendReliable: true);
	}

	public void PublishPlanet()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null || MVGameController.Instance.Game == null || JoinState != MVJoinState.Playing || GameMode != MVGameMode.Edit)
		{
			return;
		}
		if (IsOperationPending(MVOperationCodes.PublishPlanet))
		{
			Debug.LogWarning((object)"Publish planet operation is pending. Aborting publish");
		}
		else if (!isPublished)
		{
			if (GenerateTextureData.IsCreatingScreenShot)
			{
				Debug.LogWarning((object)"Texture is already being generated. Aborting publish");
			}
			else
			{
				GeneratePlanetScreenShot(PublishPlanet);
			}
		}
		else
		{
			PublishPlanet(new byte[0], ImageType.Planet, MVGameController.Instance.PlanetID);
		}
	}

	public void UploadGameScreenShot()
	{
		if (GenerateTextureData.IsCreatingScreenShot)
		{
			Debug.LogWarning((object)"Texture is already being generated. Aborting UploadGameScreenShot");
		}
		else if (IsOperationPending(MVOperationCodes.UploadScreenshot))
		{
			Debug.LogWarning((object)"UploadScreenshot operation is pending. Aborting UploadGameScreenShot");
		}
		else
		{
			GeneratePlanetScreenShot(UploadScreenshot);
		}
	}

	private static void GeneratePlanetScreenShot(Action<byte[], ImageType, int> callback)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("GenerateTexture");
		GenerateTextureData generateTextureData = val.AddComponent<GenerateTextureData>();
		generateTextureData.GenerateTextureDataCameraView(callback, ImageType.Planet, MVGameController.Instance.PlanetID);
	}

	public bool IsOperationPending(MVOperationCodes operationCode)
	{
		return operationResponsePendingManager.IsOperationPending(operationCode);
	}

	private void PublishPlanet(byte[] pngImageAsByteArray, ImageType image, int imageId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(62, pngImageAsByteArray);
		operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.PublishPlanet, dictionary);
	}

	public void GetCreditStatus()
	{
		peer.OpCustom(66, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void SetActorReady()
	{
		peer.OpCustom(0, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void AutoRegisterLocalPrototype(int woId, int worldInventoryID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(45, worldInventoryID);
		dictionary.Add(20, woId);
		peer.OpCustom(32, dictionary, sendReliable: true);
	}

	public void UpdatePrototype(int worldInventoryID, byte[] prototypeData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(45, worldInventoryID);
		dictionary.Add(47, prototypeData);
		peer.OpCustom(12, dictionary, sendReliable: true);
	}

	public void UpdatePrototypeScale(int worldInventoryID, float scale)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(45, worldInventoryID);
		dictionary.Add(32, scale);
		peer.OpCustom(13, dictionary, sendReliable: true);
	}

	public void AddWorldObjectToInventory(int worldObjectID, byte[] itemTextureData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(42, itemTextureData);
		peer.OpCustom(61, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void AddWorldObjectToInventorDev(int worldObjectID, byte[] itemTextureData, string itemName, int itemCategory, bool overWrite)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(156, itemName);
		dictionary.Add(155, itemCategory);
		dictionary.Add(157, overWrite);
		dictionary.Add(42, itemTextureData);
		peer.OpCustom(62, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void RemoveItemFromInventory(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(18, dictionary, sendReliable: true);
	}

	public void UpdateInventorySlots()
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(44, PlayerRepository.itemIDToInventorySlotIndex);
		peer.OpCustom(19, dictionary, sendReliable: true);
	}

	public void SendChatMsg(string chatMsg)
	{
		if (!(chatMsg == string.Empty))
		{
			if (chatMsg.Length > 256)
			{
				Debug.LogWarning((object)"ChatMsg too long. Truncated to 256 chars!");
				chatMsg = chatMsg.Substring(0, 256);
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(63, chatMsg);
			peer.OpCustom(30, dictionary, sendReliable: true);
		}
	}

	public void SendClientLog(string logString, string stackTrace, LogType type, Dictionary<string, object> extraSentryData)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(150, logString);
		dictionary.Add(151, stackTrace);
		dictionary.Add(152, (byte)type);
		dictionary.Add(158, extraSentryData);
		peer.OpCustom(77, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void UpdateWorldObjectData(int worldObjectID, Hashtable worldObjectData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(16, worldObjectData);
		peer.OpCustom(7, dictionary, sendReliable: true);
	}

	public void UpdateWorldObjectDataPartial(int worldObjectID, string keyPath, object value)
	{
		string[] array = keyPath.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			Debug.LogError((object)"Trying to update WO with an empty path key");
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add(array[^1], value);
		int num = array.Length - 2;
		while (0 <= num)
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2[array[num]] = hashtable;
			hashtable = hashtable2;
			num--;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(16, hashtable);
		Debug.Log((object)dictionary.BuildStringRecursive("Update wo " + worldObjectID + " partial data: "));
		peer.OpCustom(8, dictionary, sendReliable: true);
	}

	public void UpdateWorldObjectDataPartial(int worldObjectID, Hashtable woData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(16, woData);
		peer.OpCustom(8, dictionary, sendReliable: true);
	}

	public void RemoveWorldObjectDataPartial(int worldObjectID, string keyPath)
	{
		string[] array = keyPath.Split(new char[1] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			Debug.LogError((object)"Trying to remoe WO data with an empty path key");
		}
		Hashtable hashtable = new Hashtable();
		Hashtable hashtable2 = hashtable;
		for (int i = 0; i < array.Length; i++)
		{
			if (i < array.Length - 1)
			{
				Hashtable hashtable3 = new Hashtable();
				hashtable2[array[i]] = hashtable3;
				hashtable2 = hashtable3;
			}
			else
			{
				hashtable2[array[i]] = string.Empty;
			}
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(17, hashtable);
		peer.OpCustom(9, dictionary, sendReliable: true);
	}

	public void RemoveWorldObjectDataPartial(int worldObjectID, Hashtable woDataToRemove)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(17, woDataToRemove);
		peer.OpCustom(9, dictionary, sendReliable: true);
	}

	public void WorldObjectRPC(int worldObjectID, Hashtable dataPackage)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(83, dataPackage);
		peer.OpCustom(40, dictionary, sendReliable: true);
	}

	public void UpdateWorldObjectRunTimeData(int worldObjectID, Hashtable worldObjectRunTimeData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(70, worldObjectRunTimeData);
		peer.OpCustom(36, dictionary, sendReliable: true);
	}

	public void RegisterWorldObject(WorldObjectType type, int groupId, Hashtable woData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(15, type);
		dictionary.Add(21, groupId);
		dictionary.Add(16, woData);
		dictionary.Add(18, localOwner ? LocalPlayerActorNumber : 0);
		dictionary.Add(37, transferOwnershipToServerOnLeave);
		dictionary.Add(22, position.x);
		dictionary.Add(23, position.y);
		dictionary.Add(24, position.z);
		dictionary.Add(25, rotation.x);
		dictionary.Add(26, rotation.y);
		dictionary.Add(27, rotation.z);
		dictionary.Add(28, rotation.w);
		dictionary.Add(29, scale.x);
		dictionary.Add(30, scale.y);
		dictionary.Add(31, scale.z);
		peer.OpCustom(4, dictionary, sendReliable: true);
	}

	public void RequestBuiltInItem(BuiltInItem builtInItem, int groupId, Hashtable customData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(116, builtInItem);
		dictionary.Add(21, groupId);
		dictionary.Add(245, customData);
		dictionary.Add(18, localOwner ? LocalPlayerActorNumber : 0);
		dictionary.Add(37, transferOwnershipToServerOnLeave);
		dictionary.Add(22, position.x);
		dictionary.Add(23, position.y);
		dictionary.Add(24, position.z);
		dictionary.Add(25, rotation.x);
		dictionary.Add(26, rotation.y);
		dictionary.Add(27, rotation.z);
		dictionary.Add(28, rotation.w);
		dictionary.Add(29, scale.x);
		dictionary.Add(30, scale.y);
		dictionary.Add(31, scale.z);
		peer.OpCustom(59, dictionary, sendReliable: true);
	}

	public void AddItemToWorld(int itemId, int groupId, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave, bool isPreviewItem)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemId);
		dictionary.Add(21, groupId);
		dictionary.Add(18, localOwner ? LocalPlayerActorNumber : 0);
		dictionary.Add(37, transferOwnershipToServerOnLeave);
		dictionary.Add(22, position.x);
		dictionary.Add(23, position.y);
		dictionary.Add(24, position.z);
		dictionary.Add(25, rotation.x);
		dictionary.Add(26, rotation.y);
		dictionary.Add(27, rotation.z);
		dictionary.Add(28, rotation.w);
		dictionary.Add(29, scale.x);
		dictionary.Add(30, scale.y);
		dictionary.Add(31, scale.z);
		dictionary.Add(126, isPreviewItem);
		peer.OpCustom(60, dictionary, sendReliable: true);
	}

	public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, root.Id);
		dictionary.Add(18, localOwner ? LocalPlayerActorNumber : 0);
		dictionary.Add(102, cloneToRootGroup);
		dictionary.Add(128, setAsPreviewItem);
		peer.OpCustom(49, dictionary, sendReliable: true);
	}

	public void AddPlanetToPlanet(int planetId, int subtreeId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(87, planetId);
		dictionary.Add(20, subtreeId);
		peer.OpCustom(51, dictionary, sendReliable: true);
	}

	public void UnregisterWorldObject(int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(5, dictionary, sendReliable: true);
	}

	public void Ungroup(int worldObjectID)
	{
		if (!worldNetwork.WorldObjectClientManagerNetwork.Ungroup(worldObjectID))
		{
			Debug.LogWarning((object)"Ungroup failed");
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(27, dictionary, sendReliable: true);
	}

	public void ReportCaptureFlag()
	{
		peer.OpCustom(33, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void SetActorProperty(Hashtable properties)
	{
		peer.OpSetPropertiesOfActor(LocalPlayer.ActorNr, properties, broadcast: true, 0);
	}

	public void ResetLogicChunk(int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(35, dictionary, sendReliable: true);
	}

	public void OnUnregisterWorldObjectResponse(int worldObjectID)
	{
		if (!worldNetwork.OnUnregisterWorldObject(worldObjectID))
		{
			Debug.LogWarning((object)"OnUnregisterWorldObjectResponse failed!");
		}
	}

	public void UpdateWorldObject(int id, Vector3 position, Quaternion rotation, TransformPackageType packageType)
	{
		if (JoinState == MVJoinState.Playing)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, id);
			dictionary.Add(33, ServerTimeInMilliSeconds);
			dictionary.Add(22, position.x);
			dictionary.Add(23, position.y);
			dictionary.Add(24, position.z);
			dictionary.Add(25, rotation.x);
			dictionary.Add(26, rotation.y);
			dictionary.Add(27, rotation.z);
			dictionary.Add(28, rotation.w);
			dictionary.Add(34, (byte)packageType);
			peer.OpCustom(6, dictionary, sendReliable: false);
		}
	}

	public void UpdateLineOfFire(int worldObjectIDPickupOwner, Vector3 camDir, Vector3 camOrigin)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectIDPickupOwner);
		dictionary.Add(74, camOrigin.x);
		dictionary.Add(75, camOrigin.y);
		dictionary.Add(76, camOrigin.z);
		dictionary.Add(77, camDir.x);
		dictionary.Add(78, camDir.y);
		dictionary.Add(79, camDir.z);
		peer.OpCustom(39, dictionary, sendReliable: false);
	}

	public void UpdateNetworkInput(int id, NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		if (JoinState == MVJoinState.Playing)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, id);
			dictionary.Add(33, ServerTimeInMilliSeconds);
			dictionary.Add(35, (byte)actionCode);
			dictionary.Add(36, (byte)keyCode);
			peer.OpCustom(10, dictionary, sendReliable: true);
		}
	}

	public void TransferWorldObjectsToGroup(int groupId, int[] worldObjects)
	{
		if (Enumerable.Contains(worldObjects, groupId))
		{
			Debug.LogError((object)("Trying to transfer WO " + groupId + " to itself !"));
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, groupId);
		dictionary.Add(72, worldObjects);
		peer.OpCustom(48, dictionary, sendReliable: true);
	}

	public void TransferOwnership(int worldObjectID, int ownerActorNr, Transform t)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(18, ownerActorNr);
		if ((Object)(object)t == (Object)null)
		{
			dictionary.Add(84, false);
		}
		else
		{
			dictionary.Add(84, true);
			dictionary.Add(22, t.localPosition.x);
			dictionary.Add(23, t.localPosition.y);
			dictionary.Add(24, t.localPosition.z);
			dictionary.Add(25, t.localRotation.x);
			dictionary.Add(26, t.localRotation.y);
			dictionary.Add(27, t.localRotation.z);
			dictionary.Add(28, t.localRotation.w);
		}
		peer.OpCustom(11, dictionary, sendReliable: true);
	}

	public void PostGameMsg(MVGameMsgType gameMsgType, Hashtable gameMsgData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(88, (int)gameMsgType);
		dictionary.Add(89, gameMsgData);
		peer.OpCustom(42, dictionary, sendReliable: true);
	}

	public void AddTeamScore(MVTeam team, int score)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)1, score);
		SetTeamData(team, hashtable);
	}

	public void SetTeamData(MVTeam team, Hashtable teamData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(90, (int)team);
		dictionary.Add(92, teamData);
		peer.OpCustom(47, dictionary, sendReliable: true);
	}

	public void LockHierarchy(int worldObjectID, bool lockHierarchy)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(61, lockHierarchy);
		peer.OpCustom(28, dictionary, sendReliable: true);
	}

	public void RequestFriendShipByName(string name)
	{
		if (name != LocalPlayer.Username)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(9, name);
			peer.OpCustom(21, dictionary, sendReliable: true);
		}
		else
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.RequestFriendshipFromSelfMessage).Show();
		}
	}

	public void RequestFriendShipByID(int id)
	{
		if (id != LocalPlayer.ProfileID)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(51, id);
			peer.OpCustom(22, dictionary, sendReliable: true);
		}
		else
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.RequestFriendshipFromSelfMessage).Show();
		}
	}

	public void OnSyncAvatarStatusEvent(int actorNr, Hashtable data)
	{
	}

	public void OnResetLogicChunkEvent(int worldObjectID)
	{
		worldNetwork.ResetLogicFromId(worldObjectID);
	}

	public void OnPickupItemStateChangeEvent(PickupItemState state, int worldObjectID, int instigatorActorNr)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) != null)
		{
			if (!(WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVPickupItemBase))
			{
				Debug.LogError((object)"PickUpItemStateChangeEvent failed, since WOID is not derived from MVPickupItemBase");
				return;
			}
			MVPickupItemBase mVPickupItemBase = (MVPickupItemBase)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVPickupItemBase.HandleStateChange(state);
		}
		else
		{
			Debug.LogWarning((object)("OnPickupItemStateChangeEvent failed, since WorldObjectID does not exist. WOID: " + worldObjectID));
		}
	}

	public void OnUpdateLineOfFire(int worldObjectID, Vector3 camOrigin, Vector3 camDir)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		MVPickupOwner component = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)"Pickup owner not found");
		}
		else
		{
			component.SetLineOfFire(camOrigin, camDir);
		}
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
		RemoveCubes.HandleRemoveCubes(GetWOIdsWithinRadius(radius, worldPos), radius, worldPos, damage, damageFallOffType, WorldObjectClientManager.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>(), MaterialRepository.GetMaterialPhysicalProperties);
	}

	private void OnRequestFriendshipResponse(int returnCode)
	{
		string text = string.Empty;
		switch (returnCode)
		{
		case -1:
			text = "Undefined fail during friend request";
			break;
		case -2:
			text = "User does not exist";
			break;
		case -3:
			text = "You already have a pending request with that user";
			break;
		case -4:
			text = "You are already friends with that user";
			break;
		case -5:
			text = "You have blocked that user";
			break;
		case -6:
			text = "User has sent you request! Accept?";
			break;
		case -7:
			text = "That user has blocked you";
			break;
		}
		if (!(text != string.Empty))
		{
		}
	}

	public void OnPostGameMsg(MVGameMsgType gameMsgType, Hashtable gameMsgData)
	{
		if (OnReceivedGameMsg != null)
		{
			OnReceivedGameMsg(gameMsgType, gameMsgData);
		}
	}

	public void RequestAcceptFriendShip(int friendID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(50, friendID);
		peer.OpCustom(23, dictionary, sendReliable: true);
	}

	public void RequestRejectFriendShip(int friendID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(50, friendID);
		peer.OpCustom(24, dictionary, sendReliable: true);
	}

	public void RequestWoUniquePrototype(int woId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, woId);
		peer.OpCustom(32, dictionary, sendReliable: true);
	}

	private void JoinGame()
	{
		ConnState = MVConnState.Joining;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		Debug.Log((object)("JoinGame " + MVGameController.Instance.PlanetID));
		if (GameMode == MVGameMode.CharacterEditor)
		{
			Debug.Log((object)"Setting planetId to -1 as GameMode CharacterEditor is not using a planet");
		}
		dictionary.Add(11, MVGameController.Instance.ProfileID);
		dictionary.Add(byte.MaxValue, MVGameController.Instance.GameSessionData.PlanetName);
		dictionary.Add(87, MVGameController.Instance.PlanetID);
		dictionary.Add(117, GameMode);
		dictionary.Add(159, MVGameController.Instance.GameSessionData.Language);
		peer.OpCustom(byte.MaxValue, dictionary, sendReliable: true);
	}

	public bool AddLink(Link link)
	{
		if (!worldNetwork.AddPendingLink(link))
		{
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(55, link.outputWOID);
		dictionary.Add(54, link.inputWOID);
		peer.OpCustom(14, dictionary, sendReliable: true);
		return true;
	}

	public void AddObjectLink(ObjectLink link)
	{
		if (link.objectConnectorWOID <= 0 || link.objectWOID <= 0)
		{
			Debug.LogError((object)"Attempt to add objectLink, but link not connected...");
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(55, link.objectConnectorWOID);
		dictionary.Add(54, link.objectWOID);
		peer.OpCustom(44, dictionary, sendReliable: true);
		worldNetwork.AddPendingObjectLink(link);
	}

	public void RemoveLink(int linkID)
	{
		if (worldNetwork.RemovePendingLink(linkID))
		{
			Debug.Log((object)"RemoveLink");
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(56, linkID);
			peer.OpCustom(15, dictionary, sendReliable: true);
		}
	}

	public void RemoveObjectLink(int objectLinkID)
	{
		if (worldNetwork.RemovePendingObjectLink(objectLinkID))
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(56, objectLinkID);
			peer.OpCustom(45, dictionary, sendReliable: true);
		}
	}

	public void RequestTeamList()
	{
		peer.OpCustom(46, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void RequestRemoveCubesWithinRadius(int[] woIds, float radius, Vector3 position, float damage, DamageFallOffType damageFallOffType)
	{
		if (radius <= 0f)
		{
			Debug.LogError((object)"Radius can't be zero or less!");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(72, woIds);
		dictionary.Add(73, radius);
		dictionary.Add(22, position.x);
		dictionary.Add(23, position.y);
		dictionary.Add(24, position.z);
		dictionary.Add(80, damage);
		dictionary.Add(81, (int)damageFallOffType);
		peer.OpCustom(38, dictionary, sendReliable: true);
	}

	public void TriggerBoxEnter(int triggerBoxOwnerId, int triggerInstigatorId)
	{
		if (JoinState == MVJoinState.Playing)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
			peer.OpCustom(25, dictionary, sendReliable: true);
		}
	}

	public void TriggerBoxExit(int triggerBoxOwnerId, int triggerInstigatorId)
	{
		if (JoinState == MVJoinState.Playing)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, new int[2] { triggerBoxOwnerId, triggerInstigatorId });
			peer.OpCustom(26, dictionary, sendReliable: true);
		}
	}

	public void UploadScreenshot(byte[] textureData, ImageType imageType, int imageId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(119, textureData);
		dictionary.Add(120, imageId);
		dictionary.Add(118, (byte)imageType);
		operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.UploadScreenshot, dictionary);
	}

	public void PurchaseItem(int itemID, int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(37, dictionary, sendReliable: true);
	}

	private void PurchaseProduct(MVProductType productTypeID, Hashtable productData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(95, (int)productTypeID);
		dictionary.Add(96, productData);
		peer.OpCustom(52, dictionary, sendReliable: true);
	}

	public void OnPurchaseProductResponse(int returnCode, Hashtable purchaseResponseData)
	{
		if (PurchaseProductResponseHandler != null)
		{
			PurchaseProductResponseHandler(returnCode, purchaseResponseData);
		}
	}

	public void PurchaseStreamingAsset(StreamingAssetType assetType, int streamingAssetID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)106] = streamingAssetID;
		hashtable[(byte)107] = assetType;
		PurchaseProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void UnlockMaterial(int materialID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)104, materialID);
		PurchaseProduct(MVProductType.MaterialUnlock, hashtable);
	}

	public void UnlockClientShopInventoryItem(int itemId)
	{
		Debug.Log((object)("Purchase item with id: " + itemId));
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)9, itemId);
		PurchaseProduct(MVProductType.Item, hashtable);
	}

	public void PurchaseAvatar(int avatarId)
	{
		PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Combine(PurchaseProductResponseHandler, new Action<int, Hashtable>(OnProductPurchaseAvatarResponse));
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)127, avatarId);
		PurchaseProduct(MVProductType.Avatar, hashtable);
	}

	public void PurchaseRespawnNow()
	{
		Debug.Log((object)"Purchase respawn now");
		Hashtable productData = new Hashtable();
		PurchaseProduct(MVProductType.RespawnNow, productData);
	}

	private void RentProduct(MVProductType productTypeID, Hashtable productData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(95, (int)productTypeID);
		dictionary.Add(97, productData);
		peer.OpCustom(53, dictionary, sendReliable: true);
	}

	public void RentStreamingAsset(StreamingAssetType assetType, int assetID, int streamingAssetInventoryID = 0)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = assetType;
		hashtable[(byte)106] = assetID;
		hashtable[(byte)112] = streamingAssetInventoryID;
		RentProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void PurchaseAvatarAccessory(int streamingAssetID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		PurchaseProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void PurchaseAvatarAccessory(int streamingAssetID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		hashtable[(byte)20] = bodyWoID;
		hashtable[(byte)114] = slot;
		hashtable[(byte)115] = offset;
		PurchaseProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void UpdateAvatarAccessoryOffset(int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[127] = bodyWoID;
		dictionary[114] = (int)slot;
		dictionary[115] = offset;
		peer.OpCustom(78, dictionary, sendReliable: true);
	}

	public void RentAvatarAccessory(int streamingAssetID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		hashtable[(byte)20] = bodyWoID;
		hashtable[(byte)114] = slot;
		hashtable[(byte)115] = offset;
		RentProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void RentAvatarAccessory(int streamingAssetID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		RentProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		hashtable[(byte)112] = streamingAssetInventoryID;
		RentProduct(MVProductType.StreamingAsset, hashtable);
	}

	public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		hashtable[(byte)106] = streamingAssetID;
		hashtable[(byte)112] = streamingAssetInventoryID;
		hashtable[(byte)20] = bodyWoID;
		hashtable[(byte)114] = slot;
		hashtable[(byte)115] = offset;
		RentProduct(MVProductType.StreamingAsset, hashtable);
	}

	private void OnProductPurchaseAvatarResponse(int returnCode, Hashtable purchaseResponseData)
	{
		PurchaseProductResponseHandler = (Action<int, Hashtable>)Delegate.Remove(PurchaseProductResponseHandler, new Action<int, Hashtable>(OnProductPurchaseAvatarResponse));
		Debug.Log((object)("Avatar purchase response: " + (MVPurchaseReturnCode)returnCode));
		if (returnCode != 0)
		{
			WorldNetwork worldNetwork = this.worldNetwork;
			worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		}
	}

	private void ExpireProduct(MVProductType productTypeID, int productInventoryID, Hashtable expireProductData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[95] = (int)productTypeID;
		dictionary[138] = productInventoryID;
		dictionary[98] = expireProductData;
		peer.OpCustom(54, dictionary, sendReliable: true);
	}

	public void ExpireStreamingAsset(StreamingAssetType assetType, int streamingAssetInventoryID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)107, assetType);
		Hashtable expireProductData = hashtable;
		ExpireProduct(MVProductType.StreamingAsset, streamingAssetInventoryID, expireProductData);
	}

	public void ExpireAvatarAccessory(int avatarAccessoryInventoryID, int bodyWoID = 0)
	{
		Debug.LogWarning((object)("Expire accessory " + avatarAccessoryInventoryID + " on body " + bodyWoID));
		Hashtable hashtable = new Hashtable();
		hashtable[(byte)107] = StreamingAssetType.AvatarAccessory;
		if (bodyWoID != 0)
		{
			hashtable[(byte)20] = bodyWoID;
		}
		ExpireProduct(MVProductType.StreamingAsset, avatarAccessoryInventoryID, hashtable);
	}

	private void OnExpireProductResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		MVProductType mVProductType = (MVProductType)(int)returnValues[95];
		int num = (int)returnValues[138];
		if (returnCode != 0)
		{
			Debug.LogError((object)string.Concat(new object[5] { "Expiring product ", mVProductType, " with inventoryID ", num, " failed on server" }));
		}
		else if (mVProductType == MVProductType.StreamingAsset)
		{
			InventoryExpirationInfo expirationInfo = StreamingAssetExpirationChecker.GetExpirationInfo(num);
			if (expirationInfo != null)
			{
				expirationInfo.ExpirePermanently();
				StreamingAssetInventory.Remove(num);
			}
			else
			{
				Debug.LogError((object)("Cant find expiration infor for expired streaming asset with inventoryID " + num));
			}
		}
	}

	public void SetAvatarAccessorySlot(int avatarBodyWoID, int accessoryInventoryID, AvatarAccessorySlot slot, float slotOffset)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[127] = avatarBodyWoID;
		dictionary[112] = accessoryInventoryID;
		dictionary[114] = slot;
		dictionary[115] = slotOffset;
		peer.OpCustom(72, dictionary, sendReliable: true);
	}

	public void ResetAvatar(int AvatarID)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
		Debug.Log((object)"Reset ActiveAvatar  called");
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(127, AvatarID);
		peer.OpCustom(64, dictionary, sendReliable: true);
	}

	private void InitializedResetAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
		if (e.RootWO != null)
		{
			Debug.Log((object)("Reset avatar has been added to world. WorldObjectId is: " + e.RootWO));
			(MVGameController.Instance.IngameController as CharacterEditorController).SubstituteAvatar(e.RootWO.Id);
		}
	}

	private void InitializedPurchasedAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected Obj, but got Unknown
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		if (e.RootWO != null)
		{
			Debug.Log((object)("Purchased avatar has been added to world. WorldObjectId is: " + e.RootWO));
			(MVGameController.Instance.IngameController as CharacterEditorController).AddNewAvatar(e.RootWO.Id);
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(e.RootWO.Id);
			GameObject bodyCloneGO = (GameObject)Object.Instantiate((Object)(object)worldObjectClient.GameObject);
			Action<Texture2D> screenShotDataTexHandler = (Texture2D pngData) =>
			{
				UploadScreenshot(pngData.EncodeToPNG(), ImageType.Avatar, LocalPlayer.ProfileID);
			};
			AvatarScreenshotGenerator.Generate(bodyCloneGO, screenShotDataTexHandler);
		}
	}

	public void AddXP(int amount)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(85, amount);
		peer.OpCustom(41, dictionary, sendReliable: true);
		if (OnReceivedXP != null)
		{
			OnReceivedXP(amount);
		}
	}

	public void SetActiveAvatar(int AvatarID)
	{
		Debug.Log((object)"SetActiveAvatar  called");
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(127, AvatarID);
		peer.OpCustom(63, dictionary, sendReliable: true);
	}

	public void AddCloneToWorldObjects(MVWorldObjectClient wo)
	{
		worldNetwork.WorldObjectClientManagerNetwork.AddToWorldObjects(wo);
	}

	private Dictionary<byte, object> GetAttachWorldObjectToSeatData(VehicleSeatBase seatBase)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = ((Component)seatBase).gameObject.transform.localPosition;
		Quaternion localRotation = ((Component)seatBase).gameObject.transform.localRotation;
		int seatID = seatBase.SeatID;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(22, localPosition.x);
		dictionary.Add(23, localPosition.y);
		dictionary.Add(24, localPosition.z);
		dictionary.Add(25, localRotation.x);
		dictionary.Add(26, localRotation.y);
		dictionary.Add(27, localRotation.z);
		dictionary.Add(28, localRotation.w);
		dictionary.Add(143, (byte)seatID);
		dictionary.Add(144, (byte)seatBase.SeatType);
		return dictionary;
	}

	public void AttachWorldObjectToSeat(int seatOwnerWoID, int worldObjectID, VehicleSeatBase seatBase)
	{
		Dictionary<byte, object> attachWorldObjectToSeatData = GetAttachWorldObjectToSeatData(seatBase);
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)4, seatOwnerWoID);
		hashtable.Add((byte)0, worldObjectID);
		attachWorldObjectToSeatData.Add(72, hashtable);
		peer.OpCustom(74, attachWorldObjectToSeatData, sendReliable: true);
	}

	public void SpawnVehicleWithDriver(int worldObjectSpawnerVehicleID, int worldObjectID, VehicleSeatBase seatBase)
	{
		Dictionary<byte, object> attachWorldObjectToSeatData = GetAttachWorldObjectToSeatData(seatBase);
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)1, worldObjectSpawnerVehicleID);
		hashtable.Add((byte)0, worldObjectID);
		attachWorldObjectToSeatData.Add(72, hashtable);
		peer.OpCustom(76, attachWorldObjectToSeatData, sendReliable: true);
	}

	public void DetachWorldObjectFromVehicle(int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(75, dictionary, sendReliable: true);
	}

	private void OnJoinResponse(Dictionary<byte, object> returnValues)
	{
		InitializeManagers();
		int actorNr = (int)returnValues[254];
		int planetOwnershipTypeID = (int)returnValues[14];
		LocalPlayerActorNumber = actorNr;
		Debug.Log((object)("Username " + (string)returnValues[9]));
		AddPlayer((string)returnValues[9], MVGameController.Instance.ProfileID, actorNr, 0, MVTeam.None, planetOwnershipTypeID);
		LocalPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		if (LocalPlayer.ProfileID == -1)
		{
			Debug.Log((object)"Anonymous log-in...");
		}
		TouristChatAllowed = (bool)returnValues[148];
		isPublished = (bool)returnValues[82];
		bool showErrorPopupClient = (IsDebugMode = (bool)returnValues[149]);
		bool enableSentry = (bool)returnValues[154];
		DebugLogHandler.Setup(showErrorPopupClient, enableSentry);
		MVGameController.Instance.LoadLevel("Joined", MVGameController.LoadMode.Overwrite, OnJoinedLevelLoaded);
		string rootUrl = (string)returnValues[105];
		if (!AssetBundleMgr.Initialize(rootUrl, 4, 3, 1f))
		{
			Debug.LogError((object)"Failed to initialized AssetBundleMgr!");
		}
	}

	private void InitializeManagers()
	{
		worldNetwork = new WorldNetwork();
		playerController = new MVLocalObjectController(worldNetwork.WorldObjectClientManagerNetwork);
		materialRepository = new MVMaterialRepository();
		playerRepository = new PlayerRepository();
		shopRepository = new ShopRepository();
		avatarShopRepository = new ShopRepository();
		players = new Dictionary<int, MVPlayer>();
		friends = new FriendList();
		AssetBundleMgr = new AssetBundleMgr();
		GameStateController = new MVGameModeChangeNotifier();
	}

	public void OnJoinedLevelLoaded()
	{
		Debug.Log((object)"LevelLoadStarted");
		GotoNextJoinState();
	}

	private void OnRequestMaterialsResponse(Hashtable materialList)
	{
		foreach (byte key in materialList.Keys)
		{
			Hashtable hashtable = (Hashtable)materialList[key];
			string name = (string)hashtable[(byte)52];
			string description = (string)hashtable[(byte)53];
			string path = (string)hashtable[(byte)54];
			int materialSound = (int)hashtable[(byte)55];
			int modifierPackageType = (int)hashtable[(byte)56];
			string text = (string)hashtable[(byte)57];
			int priceGold = (int)hashtable[(byte)58];
			int priceSilver = (int)hashtable[(byte)59];
			bool isUnlocked = (bool)hashtable[(byte)60];
			float[] physicalProperties = (float[])hashtable[(byte)116];
			if (text.Length == 0)
			{
				MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, priceSilver, isUnlocked, physicalProperties);
			}
			else
			{
				MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, priceSilver, isUnlocked, physicalProperties, Type.GetType(text));
			}
		}
		GotoNextJoinState();
	}

	private void CreatePlayersFromUserList(Hashtable userList)
	{
		if (userList != null)
		{
			foreach (int key in userList.Keys)
			{
				logger.Log("User in UserList: " + (userList[key] as Hashtable)[(byte)9]);
				if (key != LocalPlayerActorNumber)
				{
					string username = (string)(userList[key] as Hashtable)[(byte)9];
					int profileID = (int)(userList[key] as Hashtable)[(byte)11];
					int value = (int)(userList[key] as Hashtable)[(byte)90];
					int score = (int)(userList[key] as Hashtable)[(byte)142];
					AddPlayer(username, profileID, key, score, (MVTeam)(int)Enum.ToObject(typeof(MVTeam), value));
				}
			}
			return;
		}
		Debug.LogWarning((object)"UserList is null");
	}

	private void OnGetBuiltInItemBusinessData(Dictionary<byte, object> returnValues)
	{
		Hashtable hashtable = (Hashtable)returnValues[133];
		foreach (DictionaryEntry item in hashtable)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = (int)item.Key;
			Hashtable hashtable2 = (Hashtable)item.Value;
			mVItem.itemCategoryID = (int)hashtable2[(byte)117];
			mVItem.itemTypeID = (int)hashtable2[(byte)15];
			mVItem.name = (string)hashtable2[(byte)10];
			mVItem.resellable = (bool)hashtable2[(byte)105];
			Debug.Log((object)("Built in Item.ItemId " + mVItem.itemID));
			itemBusinessLogic.AddItem(mVItem);
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
				int friendProfileID = (int)hashtable[(byte)26];
				FriendStatus status = (FriendStatus)(int)hashtable[(byte)28];
				Friends.AddFriend(key, profileID, friendProfileID, status);
			}
		}
		else
		{
			Debug.LogWarning((object)"Friendslist is null");
		}
		GotoNextJoinState();
	}

	private void WOCM_InitializedGameQueryDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		Debug.LogWarning((object)"This could be incapsulated better");
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
		if (e.RootWO is MVGroup)
		{
			WorldObjectClientManager.RootGroup = (MVGroup)e.RootWO;
		}
		else
		{
			Debug.LogError((object)"RootGroup is not found!");
		}
		if (MVGameController.Instance.OnPostGameInit != null)
		{
			MVGameController.Instance.OnPostGameInit();
		}
		MVGameController.Instance.LoadLevel("PlanetInitialized", MVGameController.LoadMode.Additive, GotoNextJoinState);
	}

	private void InitializedAvatarBodyDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedAvatarBodyDataHandler));
		if (e.RootWO != null)
		{
			int id = e.RootWO.Id;
			int[] worldObjects = new int[1] { id };
			int id2 = WorldObjectClientManager.AvatarLocal.Id;
			MVWorldObjectClientManager worldObjectClientManager = WorldObjectClientManager;
			worldObjectClientManager.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Combine(worldObjectClientManager.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(TransferBodyResponseHandler));
			LockHierarchy(id, lockHierarchy: true);
			TransferWorldObjectsToGroup(id2, worldObjects);
		}
	}

	private void TransferBodyResponseHandler(object sender, OnTransferWosResponseEventArgs e)
	{
		MVWorldObjectClientManager worldObjectClientManager = WorldObjectClientManager;
		worldObjectClientManager.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Remove(worldObjectClientManager.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(TransferBodyResponseHandler));
		Debug.Log((object)"TransferWosResponseHandler");
		if (!e.success)
		{
			Debug.LogError((object)"Body transfer failed!");
		}
		GotoNextJoinState();
	}

	private void OnTransferOwnershipResponse(Dictionary<byte, object> returnValues, int returnCode)
	{
		int id = (int)returnValues[20];
		int ownerActorNr = (int)returnValues[18];
		if (returnCode == 0)
		{
			worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipResponse(id, ownerActorNr, success: true);
		}
		else
		{
			worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipResponse(id, ownerActorNr, success: false);
		}
	}

	private void OnLockHierarchyResponse(Dictionary<byte, object> returnValues, int returnCode)
	{
		int id = (int)returnValues[20];
		bool lockObject = (bool)returnValues[61];
		if (returnCode == 0)
		{
		}
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyResponse(id, lockObject, returnCode == 0);
	}

	private void OnRequestWoUniquePrototypeFailed(Dictionary<byte, object> returnValues)
	{
		Debug.LogWarning((object)"OnRequestWoUniquePrototypeFailed");
		int woId = (int)returnValues[20];
		worldNetwork.WorldInventory.UnpendRuntimePrototype(woId);
	}

	private void OnRequestTeamListResponse(Hashtable teamList)
	{
		Hashtable hashtable = (Hashtable)teamList[0];
		Hashtable hashtable2 = (Hashtable)teamList[1];
		Hashtable hashtable3 = (Hashtable)teamList[2];
		Hashtable hashtable4 = (Hashtable)teamList[3];
		if ((bool)hashtable[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Blue);
			TeamManager.SetScore(MVTeam.Blue, (int)hashtable[(byte)1]);
		}
		if ((bool)hashtable2[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Red);
			TeamManager.SetScore(MVTeam.Red, (int)hashtable2[(byte)1]);
		}
		if ((bool)hashtable3[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Green);
			TeamManager.SetScore(MVTeam.Green, (int)hashtable3[(byte)1]);
		}
		if ((bool)hashtable4[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Yellow);
			TeamManager.SetScore(MVTeam.Yellow, (int)hashtable4[(byte)1]);
		}
		GotoNextJoinState();
	}

	private void OnPurchaseItemResponse(int returnCode)
	{
		TextSlotIndex messageIndex = TextSlotIndex.Empty;
		switch (returnCode)
		{
		case 0:
			messageIndex = TextSlotIndex.ItemPurchased;
			break;
		case -1:
			messageIndex = TextSlotIndex.UndefinedFail;
			break;
		case -2:
			messageIndex = TextSlotIndex.NotEnoughSilver;
			break;
		case -3:
			messageIndex = TextSlotIndex.PurchaseFailOwner;
			break;
		case -4:
			messageIndex = TextSlotIndex.PurchaseFailNotFound;
			break;
		case -5:
			messageIndex = TextSlotIndex.PurchaseFailInInventory;
			break;
		}
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(messageIndex).Show();
	}

	private void OnUngroupResponse(bool success)
	{
		worldNetwork.WorldObjectClientManagerNetwork.UngroupResponse(success);
	}

	private void OnLockHierarchyEvent(EventData eventData)
	{
		worldNetwork.WorldObjectClientManagerNetwork.LockHierarchyProxy((int)eventData[20], (int)eventData[18]);
	}

	private void OnUngroupEvent(EventData eventData)
	{
		worldNetwork.WorldObjectClientManagerNetwork.UngroupProxy((int)eventData[20]);
	}

	private void OnUnregisterWorldObjectEvent(int worldObjectID)
	{
		worldNetwork.OnUnregisterWorldObject(worldObjectID);
	}

	private void OnUpdateWorldObjectEvent(EventData photonEvent)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (JoinState != MVJoinState.Playing)
		{
			return;
		}
		int id = (int)photonEvent[20];
		if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
		{
			Debug.LogError((object)"Attempt to update world object, but object not registered in world");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(id).State != MVWorldObjectState.Destroyed)
		{
			NetworkTransformPackage networkTransformPackage = new NetworkTransformPackage();
			networkTransformPackage.position = new Vector3((float)photonEvent[22], (float)photonEvent[23], (float)photonEvent[24]);
			networkTransformPackage.rotation = new Quaternion((float)photonEvent[25], (float)photonEvent[26], (float)photonEvent[27], (float)photonEvent[28]);
			networkTransformPackage.timestamp = (int)photonEvent[33];
			networkTransformPackage.packageType = (TransformPackageType)(byte)photonEvent[34];
			if (WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject != null && (object)WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject.GetType() == typeof(MVNetworkListener))
			{
				(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject as MVNetworkListener).AddTransformPackage(networkTransformPackage);
			}
			else if (WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject != null)
			{
				Debug.LogWarning((object)string.Concat("worldObjectClientManager.WorldObjects[worldObjectID].NetworkObject is ", WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject.GetType(), " this is probably due to ownership switching of vehicle"));
			}
		}
		else
		{
			Debug.LogWarning((object)"Attempt to update world object, but object in destroyed state");
		}
	}

	private void OnWorldObjectRPCEvent(EventData photonEvent)
	{
		if (JoinState == MVJoinState.Playing)
		{
			int id = (int)photonEvent[20];
			if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
			{
				Debug.LogError((object)"Attempt to update world object, but object not registered in world");
			}
			else if (WorldObjectClientManager.GetWorldObjectClient(id).State != MVWorldObjectState.Destroyed)
			{
				int key = (int)photonEvent[254];
				MVPlayer p = Players[key];
				MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
				worldObjectClient.ReceivePackage(p, (Hashtable)photonEvent[83]);
			}
			else
			{
				Debug.LogWarning((object)"Attempt to update world object, but object in destroyed state");
			}
		}
	}

	private void OnUpdateNetworkInputEvent(EventData photonEvent)
	{
		int id = (int)photonEvent[20];
		NetworkInputActionCodes actionCode = (NetworkInputActionCodes)(byte)Enum.ToObject(typeof(NetworkInputActionCodes), (byte)photonEvent[35]);
		NetworkInputKeyCodes keyCode = (NetworkInputKeyCodes)(byte)photonEvent[36];
		int timestamp = (int)photonEvent[33];
		if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
		{
			Debug.LogError((object)"Attempt to update network input on world object that is not in list");
			return;
		}
		if (!(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject is MVNetworkListener))
		{
			Debug.LogError((object)"Attempt to update network input, but NetworkObject is not a listener");
		}
		NetworkInputPackage networkInputPackage = new NetworkInputPackage();
		networkInputPackage.actionCode = actionCode;
		networkInputPackage.keyCode = keyCode;
		networkInputPackage.timestamp = timestamp;
		(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject as MVNetworkListener).AddNetworkInputPackage(networkInputPackage);
	}

	private void OnTransferOwnershipEvent(EventData photonEvent)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (bool)photonEvent[84];
		int id = (int)photonEvent[20];
		int ownerActorNr = (int)photonEvent[18];
		worldNetwork.WorldObjectClientManagerNetwork.TransferOwnershipProxy(id, ownerActorNr);
		if (flag)
		{
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
			if (worldObjectClient != null)
			{
				Vector3 position = new Vector3((float)photonEvent[22], (float)photonEvent[23], (float)photonEvent[24]);
				Quaternion rotation = new Quaternion((float)photonEvent[25], (float)photonEvent[26], (float)photonEvent[27], (float)photonEvent[28]);
				worldObjectClient.ClearTransformQueue();
				worldObjectClient.Position = position;
				worldObjectClient.Rotation = rotation;
			}
		}
	}

	private void OnUnregisterPrototypeEvent(int worldInventoryID)
	{
		worldNetwork.WorldInventory.RemovePrototype(worldInventoryID);
	}

	private void OnFriendRequestEvent(int friendID, int profileID, int friendProfileID)
	{
		Friends.AddFriend(friendID, profileID, friendProfileID, FriendStatus.Pending);
	}

	private void OnFriendUpdateEvent(int friendID, int profileID, FriendStatus status)
	{
		Friends.UpdateFriend(friendID, profileID, status);
	}

	private void OnAddLinkEvent(int fromID, int toID, int linkID)
	{
		Link link = new Link();
		link.outputWOID = fromID;
		link.inputWOID = toID;
		link.id = linkID;
		worldNetwork.AddLink(link);
	}

	private void OnRemoveLinkEvent(int linkID)
	{
		worldNetwork.RemoveLink(linkID);
	}

	private void OnAddObjectLinkEvent(int fromID, int toID, int linkID)
	{
		ObjectLink objectLink = new ObjectLink();
		objectLink.objectConnectorWOID = fromID;
		objectLink.objectWOID = toID;
		objectLink.id = linkID;
		worldNetwork.AddObjectLink(objectLink);
	}

	private void OnRemoveObjectLinkEvent(int linkID)
	{
		Debug.Log((object)"Remove objectLink!");
		worldNetwork.RemoveObjectLink(linkID);
	}

	private void OnTriggerBoxEnterEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError((object)("OnTriggerBoxEnterEvent received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnEnter(Players[actorNr]);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxExitEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError((object)("OnTriggerBoxExitEvent received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnExit(Players[actorNr]);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxStayBegin(int worldObjectID, int actorNr)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnStayBegin(actorNr);
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVPressurePlate)
		{
			MVPressurePlate mVPressurePlate = (MVPressurePlate)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVPressurePlate.OnStayBegin(actorNr);
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnTriggerBoxStayEnd(int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError((object)("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " does not exist"));
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnStayEnd();
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVPressurePlate)
		{
			MVPressurePlate mVPressurePlate = (MVPressurePlate)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVPressurePlate.OnStayEnd();
		}
		else
		{
			Debug.LogError((object)("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox"));
		}
	}

	private void OnAddItemToInventoryEvent(int actorNr, int itemID, int itemCategoryID, int itemTypeID, string itemName, byte[] itemData, int slotIndex, int worldObjectID, bool isResellable, int authorProfileId, int originalItemID, int priceGold)
	{
		if (PlayerRepository.PlayerInventory.ContainsKey(itemID))
		{
			Debug.LogWarning((object)"Item already exists in inventory. Will be overwritten");
		}
		if (actorNr == LocalPlayerActorNumber)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = itemID;
			mVItem.itemCategoryID = itemCategoryID;
			mVItem.itemTypeID = itemTypeID;
			mVItem.data = itemData;
			mVItem.name = itemName;
			mVItem.resellable = isResellable;
			mVItem.authorProfileID = authorProfileId;
			mVItem.originalItemID = originalItemID;
			mVItem.priceGold = priceGold;
			mVItem.description = string.Empty;
			mVItem.priceSilver = 0;
			Debug.Log((object)("Data length: " + mVItem.data.Length));
			PlayerRepository.PlayerInventory[mVItem.itemID] = mVItem;
			PlayerRepository.itemIDToInventorySlotIndex[mVItem.itemID] = slotIndex;
			PlayerRepository.NotifyRepositoryChange();
			itemBusinessLogic.AddItem(mVItem);
		}
		else
		{
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			if (worldObjectClient == null)
			{
				Debug.LogWarning((object)"Attempted to assign itemId to worldObject failed. This is probably because the worldObject was deleted");
				return;
			}
		}
		MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
		{
			wo.ItemId = itemID;
		};
		MVWorldObjectClient worldObjectClient2 = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		worldObjectClient2.TraverseRecursiveTail(callBack);
	}

	private void OnRemoveItemFromInventory(int itemID)
	{
		if (!PlayerRepository.PlayerInventory.ContainsKey(itemID))
		{
			Debug.LogError((object)"Attempt to remove item to inventory, but itemID not in inventory");
		}
		else
		{
			PlayerRepository.RemoveItem(itemID);
		}
	}

	private void OnChatMsgEvent(int actorNr, string chatMsg)
	{
		MVPlayer sender = Players[actorNr];
		if (OnReceivedChatMessage != null)
		{
			OnReceivedChatMessage(sender, chatMsg);
		}
	}

	private void OnWoUniquePrototypeEvent(int woId, int worldInventoryId)
	{
		worldNetwork.WorldInventory.OnReplaceWoPrototype(woId, worldInventoryId);
	}

	private void OnGameStateChange(MVGameStateType gameStateType, int startTime, int duration, MVGameStateReason reason, int actorNr)
	{
		Debug.Log((object)string.Concat(new object[6] { "OnGameStateChange EVENT: ", gameStateType, ", reason: ", reason, ", duration: ", duration }));
		networkGameStateListener.ChangeState(this, gameStateType, startTime, duration, reason, actorNr);
	}

	private void AddPlayer(string username, int profileID, int actorNr, int score, MVTeam team = MVTeam.None, int planetOwnershipTypeID = 0)
	{
		if (Players.ContainsKey(actorNr))
		{
			Debug.LogWarning((object)"Duplicate player");
			return;
		}
		MVPlayer mVPlayer = new MVPlayer();
		mVPlayer.ActorNr = actorNr;
		mVPlayer.ProfileID = profileID;
		mVPlayer.Username = username;
		mVPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		mVPlayer.Team = team;
		Debug.Log((object)("initial score " + score));
		mVPlayer.Score = score;
		Players.Add(actorNr, mVPlayer);
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	private void SelectTeamFromJoinFlow()
	{
		List<MVTeam> teamList = TeamManager.GetTeamList();
		if (teamList.Count == 1 || GameMode != MVGameMode.Play)
		{
			LocalPlayer.Team = MVTeam.None;
			GotoNextJoinState();
		}
		else
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateCustomDialog("Prefabs/GUI/TeamSelect/TeamSelectDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: false, canClose: false).SetOnResultCallback(TeamSelectCallBack)
				.Show();
		}
	}

	private void TeamSelectCallBack(UXDialogBox dialog)
	{
		MVTeam team = (MVTeam)(int)dialog.GetResult();
		LocalPlayer.Team = team;
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
		GotoNextJoinState();
	}

	private void SetTeamFromJoinFlow()
	{
		SetTeam(LocalPlayer.Team);
		GotoNextJoinState();
	}

	public void SetTeam(MVTeam team)
	{
		LocalPlayer.Team = team;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(90, (int)team);
		peer.OpCustom(43, dictionary, sendReliable: true);
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	public void OnAddTeamEvent(MVTeam team)
	{
		TeamManager.AddTeam(team);
	}

	public void OnRemoveTeamEvent(MVTeam team)
	{
		TeamManager.RemoveTeam(team);
	}

	public void OnSetTeamDataEvent(MVTeam team, Hashtable teamData)
	{
		if (teamData.ContainsKey((byte)1))
		{
			TeamManager.AddScore(team, (int)teamData[(byte)1]);
		}
	}

	public void OnResetTeamData(MVTeam team, Hashtable teamData)
	{
		if (teamData.ContainsKey((byte)1))
		{
			TeamManager.SetScore(team, (int)teamData[(byte)1]);
		}
	}

	public void OnSetWorldObjectsToPurchasedEvent(int purchaseProfileId, int itemId)
	{
		worldNetwork.WorldObjectClientManagerNetwork.OnSetWorldObjectsToPurchasedEvent(purchaseProfileId, itemId);
	}

	public void OnCreditStatusEvent(int silverAmount, int goldAmount)
	{
		LocalPlayer.SilverAmount = silverAmount;
		LocalPlayer.GoldAmount = goldAmount;
		BrowserComm.ToWeb.ExternalCall("refreshCredentials");
	}

	public void OnTransferWorldObjectsToGroup(EventData eventData)
	{
		int groupId = (int)eventData[20];
		int[] array = (int[])eventData[72];
		string text = string.Empty;
		int[] array2 = array;
		foreach (int num in array2)
		{
			text = text + num + " ";
		}
		worldNetwork.WorldObjectClientManagerNetwork.OnTransferWorldObjectsToGroupEvent(groupId, array);
	}

	public void OnCloneWorldObjectTree(EventData eventData)
	{
		int[] array = (int[])eventData[72];
		int ownerActorNumber = (int)eventData[18];
		int cloneLinkId = (int)eventData[56];
		int cloneObjectLinkId = (int)eventData[93];
		bool cloneToRootGroup = (bool)eventData[102];
		int previewProfileOwnerId = (int)eventData[129];
		worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, previewProfileOwnerId, cloneToRootGroup, array[0], array[1], cloneLinkId, cloneObjectLinkId);
	}

	public void OnGetGameBatch(EventData eventData)
	{
		if (!eventData.Parameters.ContainsKey(245))
		{
			Debug.LogError((object)"eventData.Contains((byte)MVParameterKeys.Data");
			return;
		}
		int instigator = (int)eventData[254];
		BytePacker bp = new BytePacker((byte[])eventData[245]);
		QueryType queryType = (QueryType)(byte)eventData[135];
		int queryId = -1;
		bool queryDataLeft = false;
		if (eventData.Parameters.ContainsKey(100))
		{
			queryId = (int)eventData[100];
		}
		if (eventData.Parameters.ContainsKey(101))
		{
			queryDataLeft = (bool)eventData[101];
		}
		gameDataQueryManager.HandleDataBatch(instigator, queryId, queryType, queryDataLeft, bp);
	}

	private void OnGameQueryReady(EventData eventData)
	{
		int queryId = (int)eventData[100];
		gameDataQueryManager.OnGameQueryReady(queryId);
	}

	private void OnPostWinnerReportEvent(EventData photonEvent)
	{
		Debug.Log((object)"OnPostWinnerReportEvent");
		WinnerReportBase winnerReportBase = new WinnerReportBase();
		winnerReportBase.winningState = (MVWinningState)(int)photonEvent[121];
		winnerReportBase.winningType = (MVWinningCondition)(int)photonEvent[122];
		winnerReportBase.isTeamGame = TeamManager.TeamCount() > 1;
		int[] array = (int[])photonEvent[123];
		int[] array2 = (int[])photonEvent[124];
		Debug.Log((object)("WINNER-LIST LENGTH: " + array.Length + ", " + array2.Length));
		Debug.Log((object)string.Concat(new object[4] { "WinningState: ", winnerReportBase.winningState, ", WinningType: ", winnerReportBase.winningType }));
		Debug.Log((object)"WINNER SCORES CLIENT-SIDE:");
		int[] array3 = array2;
		foreach (int num in array3)
		{
			Debug.Log((object)("SCORE: " + num));
		}
		for (int j = 0; j < array.Length; j++)
		{
			Debug.Log((object)j);
			winnerReportBase.AddWinner(array[j], array2[j]);
		}
		MVGameController.Instance.IngameController.OnWinnerReportReceived(winnerReportBase);
	}

	private void OnCollectiblePickedUp(EventData photonEvent)
	{
		Debug.Log((object)"OnCollectiblePickedUp");
		int actorNr = (int)photonEvent[254];
		int id = (int)photonEvent[20];
		if (WorldObjectClientManager.GetWorldObjectClient(id) != null)
		{
			if (WorldObjectClientManager.GetWorldObjectClient(id) is MVCollectible)
			{
				(WorldObjectClientManager.GetWorldObjectClient(id) as MVCollectible).OnPickup(actorNr);
			}
			else
			{
				Debug.LogError((object)"Attempt to call WO that is not collectible, OnCollectiblePickedUpEvent");
			}
		}
	}

	private void RequestGetNextGameBatch(int queryId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[100] = queryId;
		peer.OpCustom(55, dictionary, sendReliable: true);
	}

	private void RequestStreamingAssetList(params StreamingAssetType[] assetTypes)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		int[] value = assetTypes.Select((StreamingAssetType type) => (int)type).ToArray();
		dictionary.Add(108, value);
		peer.OpCustom(56, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetListResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (returnCode != 0)
		{
			Debug.LogError((object)("RequestStreamingAssetList failed. returnCode: " + returnCode));
		}
		else
		{
			Hashtable hashtable = (Hashtable)returnValues[109];
			foreach (int key in hashtable.Keys)
			{
				Hashtable hashtable2 = (Hashtable)hashtable[key];
				StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
				streamingAssetInfo.ProductID = key;
				streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)hashtable2[(byte)65];
				streamingAssetInfo.CategoryID = (int)hashtable2[(byte)67];
				streamingAssetInfo.Name = (string)hashtable2[(byte)68];
				streamingAssetInfo.Desc = (string)hashtable2[(byte)69];
				streamingAssetInfo.AssetPath = (string)hashtable2[(byte)70];
				streamingAssetInfo.Version = (int)(hashtable2[(byte)71] ?? ((object)0));
				StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
				hashSet.Add((int)streamingAssetInfo.StreamedAssetType);
				ProductShopInfo productShopInfo = null;
				if (hashtable2.ContainsKey((byte)77))
				{
					productShopInfo = new ProductShopInfo();
					productShopInfo.PriceGold = (int)hashtable2[(byte)77];
					productShopInfo.PriceSilver = (int)hashtable2[(byte)78];
					productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
					productShopInfo.RentPriceGold = (int)hashtable2[(byte)80];
					productShopInfo.RentPriceSilver = (int)hashtable2[(byte)79];
					productShopInfo.RentExpireSeconds = (int)hashtable2[(byte)81];
					productShopInfo.IsRentable = productShopInfo.RentPriceGold != 0 || productShopInfo.RentPriceSilver != 0;
					streamingAssetInfo.ShopInfo = productShopInfo;
				}
				if (streamingAssetInfo.ShopInfo != null)
				{
					StreamingAssetShopInventory.Add(streamingAssetInfo);
				}
			}
		}
		StreamingAssetShopInventory.NotifyProductShopInventoryChange();
		GotoNextJoinState();
	}

	private void RequestStreamingAssetInventory(params StreamingAssetType[] assetTypes)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		int[] value = assetTypes.Select((StreamingAssetType type) => (int)type).ToArray();
		dictionary.Add(108, value);
		peer.OpCustom(57, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetInventoryResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		if (returnCode != 0)
		{
			Debug.LogError((object)("RequestStreamingAssetInventory failed. returnCode: " + returnCode));
		}
		else
		{
			Hashtable hashtable = (Hashtable)returnValues[110];
			foreach (int key in hashtable.Keys)
			{
				Hashtable hashtable2 = (Hashtable)hashtable[key];
				int num2 = key;
				DateTime purchaseTime = new DateTime((long)hashtable2[(byte)84]);
				bool isRented = (bool)hashtable2[(byte)82];
				int num3 = (int)hashtable2[(byte)63];
				StreamingAssetInfo value = null;
				StreamingAssetInfoMap.TryGetValue(num3, out value);
				if (value == null)
				{
					Debug.LogError((object)("Missing asset info " + num3 + " for asset " + num2));
				}
				else
				{
					ProductInventoryInfo<StreamingAssetInfo> invInfo = new ProductInventoryInfo<StreamingAssetInfo>(num2, value, purchaseTime, isRented);
					StreamingAssetInventory.Add(invInfo);
				}
			}
		}
		StreamingAssetInventory.NotifyProductInventoryChange();
		GotoNextJoinState();
	}

	public void RequestStreamingAssetInventoryItems(int[] inventoryIDs)
	{
		Debug.LogWarning((object)("Request SA inventory items " + inventoryIDs.BuildString(null, eachEntryNewLine: false)));
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(113, inventoryIDs);
		peer.OpCustom(58, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetInventoryItemsResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		int[] array = (int[])returnValues[113];
		int[] array2 = array;
		foreach (int item in array2)
		{
			ownedRequestedIDs.Remove(item);
		}
		if (returnCode != 0)
		{
			Debug.LogError((object)("RequestStreamingAssetInventoryItem failed. returnCode: " + returnCode));
			int[] array3 = array;
			foreach (int item2 in array3)
			{
				ownedRequestFailedIDs.Add(item2);
			}
			return;
		}
		Hashtable hashtable = (Hashtable)returnValues[111];
		int[] array4 = array;
		foreach (int num in array4)
		{
			if (!hashtable.Contains(num))
			{
				ownedRequestFailedIDs.Add(num);
				continue;
			}
			Hashtable hashtable2 = (Hashtable)hashtable[num];
			StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
			streamingAssetInfo.ProductID = (int)hashtable2[(byte)63];
			streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)hashtable2[(byte)65];
			streamingAssetInfo.CategoryID = (int)hashtable2[(byte)67];
			streamingAssetInfo.Name = (string)hashtable2[(byte)68];
			streamingAssetInfo.Desc = (string)hashtable2[(byte)69];
			streamingAssetInfo.AssetPath = (string)hashtable2[(byte)70];
			if (!StreamingAssetInfoMap.ContainsKey(streamingAssetInfo.ProductID))
			{
				StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
			}
			ProductShopInfo productShopInfo = null;
			if (hashtable2.ContainsKey((byte)77))
			{
				productShopInfo = new ProductShopInfo();
				productShopInfo.PriceGold = (int)hashtable2[(byte)77];
				productShopInfo.PriceSilver = (int)hashtable2[(byte)78];
				productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
				productShopInfo.RentPriceGold = (int)hashtable2[(byte)80];
				productShopInfo.RentPriceSilver = (int)hashtable2[(byte)79];
				productShopInfo.RentExpireSeconds = (int)hashtable2[(byte)81];
				productShopInfo.IsRentable = productShopInfo.RentPriceGold != 0 || productShopInfo.RentPriceSilver != 0;
				streamingAssetInfo.ShopInfo = productShopInfo;
			}
			if (streamingAssetInfo.ShopInfo != null && !StreamingAssetShopInventory.Contains(streamingAssetInfo.ProductID))
			{
				StreamingAssetShopInventory.Add(streamingAssetInfo);
			}
			if (!StreamingAssetInventory.Contains(num))
			{
				DateTime purchaseTime = new DateTime((long)hashtable2[(byte)84]);
				bool isRented = (bool)hashtable2[(byte)82];
				ProductInventoryInfo<StreamingAssetInfo> invInfo = new ProductInventoryInfo<StreamingAssetInfo>(num, streamingAssetInfo, purchaseTime, isRented);
				StreamingAssetInventory.Add(invInfo);
			}
			ownedRequestedIDs.Remove(num);
			Debug.Log((object)("Fetched StreamingAssetInventoryItem: " + streamingAssetInfo.Name + " rentSilver " + productShopInfo.RentPriceSilver));
		}
	}

	private void OnGetCreditStatus(int silverAmount, int goldAmount)
	{
		LocalPlayer.SilverAmount = silverAmount;
		LocalPlayer.GoldAmount = goldAmount;
		GotoNextJoinState();
	}

	private void OnGetActiveAvatarResponse(int returnCode, int woid)
	{
		(MVGameController.Instance.IngameController as CharacterEditorController).SetActiveAvatar(woid);
		GotoNextJoinState();
	}

	public void OnSetTeamEvent(int actorNr, MVTeam team)
	{
		if (LocalPlayer.ActorNr != actorNr)
		{
			Players[actorNr].Team = team;
			if (Players[actorNr].Avatar != null)
			{
				Players[actorNr].Avatar.SetTeam();
			}
			if (onPlayerListChanged != null)
			{
				onPlayerListChanged();
			}
		}
	}

	public void RequestDBQuery(DBQuery query, Hashtable inData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(0, (byte)query);
		dictionary.Add(2, inData);
		peer.OpCustom(1, dictionary, sendReliable: true);
	}

	public void OnDBQueryResponse(Hashtable outData)
	{
		if (outData == null)
		{
			Debug.LogError((object)"OnDBQueryResponse: outData is null");
		}
		else if (JoinState == MVJoinState.FetchingItemTypes)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (object key in outData.Keys)
			{
				dictionary.Add((string)outData[(int)key], (int)key);
			}
			itemCategories = new ItemCategories(dictionary);
		}
		else if (JoinState == MVJoinState.FetchingOwnershipTypes)
		{
			foreach (object key2 in outData.Keys)
			{
				PlayerRepository.PlanetOwnershipTypes.Add((int)key2, (string)outData[(int)key2]);
			}
		}
		if (JoinState != MVJoinState.Playing)
		{
			GotoNextJoinState();
		}
	}

	public void OnDBQueryFailed(DBReasonCode reason)
	{
		Debug.Log((object)("DBQuery failed during GameState '" + JoinState.ToString() + "'. Reason: " + reason));
		if (JoinState != MVJoinState.Playing)
		{
			GotoNextJoinState();
		}
	}

	public void RequestMarketPlaceItem(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(69, dictionary, sendReliable: true);
	}

	public void RequestAddItemToMarketPlace(int itemID, string itemName, string itemDescription, int silverPrice)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		dictionary.Add(40, itemName);
		dictionary.Add(136, itemDescription);
		dictionary.Add(69, silverPrice);
		peer.OpCustom(70, dictionary, sendReliable: true);
	}

	public void RequestRemoveItemFromMarketPlace(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(71, dictionary, sendReliable: true);
	}

	public void RequestLargeDBQuery(DBQuery query, Hashtable inData, int numRowsPerReturn)
	{
		Debug.Log((object)$"RequestLargeDBQuery: {query}. InData: {inData.BuildStringRecursive()}. NumRowsPerReturn: {numRowsPerReturn}");
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(0, (byte)query);
		dictionary.Add(2, inData);
		dictionary.Add(6, numRowsPerReturn);
		peer.OpCustom(2, dictionary, sendReliable: true);
	}

	public void OnLargeDBQueryResponse(int largeDBQueryID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(5, largeDBQueryID);
		peer.OpCustom(3, dictionary, sendReliable: true);
	}

	private void OnInventoryResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.itemCategoryID = (int)((Hashtable)outData[key])[(byte)117];
			mVItem.itemTypeID = (int)((Hashtable)outData[key])[(byte)15];
			mVItem.name = (string)((Hashtable)outData[key])[(byte)10];
			mVItem.description = (string)((Hashtable)outData[key])[(byte)108];
			mVItem.data = (byte[])((Hashtable)outData[key])[(byte)11];
			mVItem.resellable = (bool)((Hashtable)outData[key])[(byte)105];
			mVItem.priceSilver = (int)((Hashtable)outData[key])[(byte)78];
			mVItem.priceGold = (int)((Hashtable)outData[key])[(byte)77];
			mVItem.shopInventoryID = (int)((Hashtable)outData[key])[(byte)109];
			mVItem.authorProfileID = (int)((Hashtable)outData[key])[(byte)107];
			mVItem.originalItemID = (int)((Hashtable)outData[key])[(byte)111];
			if (!(bool)((Hashtable)outData[key])[(byte)110])
			{
				PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
				int num2 = (int)((Hashtable)outData[key])[(byte)22];
				PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
			}
			itemBusinessLogic.AddItem(mVItem);
		}
		PlayerRepository.NotifyRepositoryChange();
	}

	private void OnShopInventoryResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		ShopRepository shopRepository = ShopRepository;
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.itemCategoryID = (int)((Hashtable)outData[key])[(byte)117];
			mVItem.itemTypeID = (int)((Hashtable)outData[key])[(byte)15];
			mVItem.name = (string)((Hashtable)outData[key])[(byte)10];
			mVItem.description = (string)((Hashtable)outData[key])[(byte)108];
			mVItem.data = (byte[])((Hashtable)outData[key])[(byte)11];
			mVItem.resellable = (bool)((Hashtable)outData[key])[(byte)105];
			int priceSilver = (int)((Hashtable)outData[key])[(byte)78];
			int priceGold = (int)((Hashtable)outData[key])[(byte)77];
			mVItem.priceSilver = priceSilver;
			mVItem.priceGold = priceGold;
			ShopRepository.ShopInventory.Add(mVItem.itemID, mVItem);
			int num2 = (int)((Hashtable)outData[key])[(byte)102];
			shopRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
			if (!shopRepository.ItemCategoriesInShop.Contains(mVItem.itemCategoryID))
			{
				shopRepository.ItemCategoriesInShop.Add(mVItem.itemCategoryID);
			}
		}
		if (isDone)
		{
			shopRepository.ReorganizeItemsByItemType();
			shopRepository.NotifyRepositoryChange();
		}
	}

	private void OnAvatarShopInventoryResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.data = (byte[])((Hashtable)outData[key])[(byte)93];
			mVItem.name = "Avatar " + key;
			int priceSilver = (int)((Hashtable)outData[key])[(byte)78];
			int priceGold = (int)((Hashtable)outData[key])[(byte)77];
			mVItem.priceSilver = priceSilver;
			mVItem.priceGold = priceGold;
			int num2 = (int)((Hashtable)outData[key])[(byte)102];
			AvatarShopRepository.ShopInventory.Add(mVItem.itemID, mVItem);
			AvatarShopRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
		}
		AvatarShopRepository.NotifyRepositoryChange();
	}

	public void OnHandleGetNextResultSetResponse(Hashtable outData, int largeQueryId, bool isDone)
	{
		if (OnGetNextResultSetResponse != null)
		{
			OnGetNextResultSetResponse(outData, largeQueryId, isDone);
		}
		if (isDone)
		{
			OnGetNextResultSetResponse = null;
			GotoNextJoinState();
		}
		else
		{
			OnLargeDBQueryResponse(largeQueryId);
		}
	}

	public void OnAddWorldObjectToInventoryResponse(int returnCode, int price, int itemID, int worldObjectID)
	{
		bool flag = false;
		TextSlotIndex messageIndex = TextSlotIndex.Empty;
		ValueInsert values = null;
		switch (returnCode)
		{
		case 0:
			messageIndex = TextSlotIndex.AddedModelToInventory;
			break;
		case -1:
			messageIndex = TextSlotIndex.UndefinedFail;
			break;
		case -5:
			messageIndex = TextSlotIndex.FailedtoAddToInventory;
			break;
		case -4:
			messageIndex = TextSlotIndex.FailedToCreateItem;
			break;
		case -3:
			messageIndex = TextSlotIndex.NotCreatorMessage;
			break;
		case -6:
			messageIndex = TextSlotIndex.NotCreatorMessageForSale;
			values = new ValueInsert().AddInt(price);
			flag = true;
			break;
		case -7:
			messageIndex = TextSlotIndex.ItemExistsInInventory;
			break;
		case -2:
			messageIndex = TextSlotIndex.PrototypeNotFound;
			break;
		}
		if (!flag)
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(messageIndex, TextSlotIndex.AddToInventory).Show();
			return;
		}
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(messageIndex, TextSlotIndex.BuyModelQuestion, UXDialogType.Simple, noButtons: false, stackDialog: false, canClose: true, values).AddPositiveButton(TextSlotIndex.Confirm)
			.AddNegativeButton(TextSlotIndex.Reject)
			.SetOnResultCallback((UXDialogBox dialog) =>
			{
				if (dialog.DialogResult == UXDialogResult.Positive)
				{
					PurchaseItem(itemID, worldObjectID);
				}
			})
			.Show();
	}

	public void OnAddWorldObjectToInventoryResponseDev(int returnCode, int worldObjectID, int itemID)
	{
		string empty = string.Empty;
		empty = ((returnCode != 0) ? "Item not added to inventory" : ("Successfully added model to your inventory. ItemID is: " + itemID));
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDevelopmentDialog(empty, string.Empty).Show();
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		try
		{
			HandleOperationResponse(operationResponse);
		}
		catch (Exception ex)
		{
			string text = $"MVOperationCodes: {(MVOperationCodes)operationResponse.OperationCode} ReturnCode {operationResponse.ReturnCode}";
			Debug.LogError((object)("OperationResponse " + text));
			throw ex;
		}
		operationResponsePendingManager.TryRemovePendingOperation((MVOperationCodes)operationResponse.OperationCode);
	}

	private void HandleOperationResponse(OperationResponse operationResponse)
	{
		short returnCode = operationResponse.ReturnCode;
		MVOperationCodes operationCode = (MVOperationCodes)operationResponse.OperationCode;
		Dictionary<byte, object> parameters = operationResponse.Parameters;
		switch (operationCode)
		{
		case MVOperationCodes.Join:
		{
			ConnState = MVConnState.Joined;
			if (returnCode == 0)
			{
				OnJoinResponse(parameters);
				break;
			}
			string message = "Undefined join error.";
			if (operationResponse.DebugMessage != null)
			{
				message = operationResponse.DebugMessage;
			}
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDevelopmentDialog(message, "Error").Show();
			peer.Disconnect();
			break;
		}
		case MVOperationCodes.Leave:
			ConnState = MVConnState.Disconnecting;
			peer.Disconnect();
			break;
		case MVOperationCodes.PublishPlanet:
		{
			TextSlotIndex textSlotIndex = TextSlotIndex.Empty;
			textSlotIndex = returnCode switch
			{
				0 => TextSlotIndex.SuccesfullyPublished, 
				-1 => TextSlotIndex.UndefinedFail, 
				-2 => TextSlotIndex.NotAuthorizedToPublish, 
				_ => TextSlotIndex.UnhandledReturnCode, 
			};
			UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(textSlotIndex);
			if ((Object)(object)uXDialogFactory != (Object)null)
			{
				uXDialogFactory.Show();
			}
			string textWithValues = Localization.Instance.GetTextWithValues(textSlotIndex, null);
			BrowserComm.ToWeb.ExternalCall("publishPlanetResult", returnCode == 0, textWithValues);
			break;
		}
		case MVOperationCodes.SetActorReady:
			if (JoinState == MVJoinState.SettingActorReady)
			{
				MVGameController.Instance.LoadLevel("ActorReady", MVGameController.LoadMode.Additive, GotoNextJoinState);
			}
			else
			{
				Debug.LogError((object)("SetActorReady returned, but we're not in SettingActorReadyState, but in " + JoinState));
			}
			break;
		case MVOperationCodes.GetBuiltInItemBusinessData:
			OnGetBuiltInItemBusinessData(parameters);
			break;
		case MVOperationCodes.RequestFriends:
			OnRequestFriendsResponse((Hashtable)parameters[49]);
			break;
		case MVOperationCodes.UnregisterWorldObject:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"Failed to unregister worldObject");
			}
			else
			{
				OnUnregisterWorldObjectResponse((int)parameters[20]);
			}
			break;
		case MVOperationCodes.UpdateWorldObjectData:
			if (returnCode != 0)
			{
				Debug.LogError((object)"UpdateWorldObjectData FAILED on server!");
			}
			break;
		case MVOperationCodes.UpdateWorldObjectDataPartial:
			if (returnCode != 0)
			{
				Debug.LogError((object)"UpdateWorldObjectDataPartial FAILED on server!");
			}
			break;
		case MVOperationCodes.UpdateWorldObjectRunTimeData:
			if (returnCode != 0)
			{
				Debug.LogError((object)"UpdateWorldObjectRunTimeData FAILED on server!");
			}
			break;
		case MVOperationCodes.TransferOwnership:
			OnTransferOwnershipResponse(parameters, returnCode);
			break;
		case MVOperationCodes.DBQuery:
			if ((byte)parameters[3] == 0)
			{
				OnDBQueryResponse((Hashtable)parameters[1]);
			}
			else
			{
				OnDBQueryFailed((DBReasonCode)(byte)parameters[4]);
			}
			break;
		case MVOperationCodes.LargeDBQuery:
			if (returnCode == 0)
			{
				OnLargeDBQueryResponse((int)parameters[5]);
			}
			else
			{
				Debug.LogError((object)"LargeDBQuery response failed...");
			}
			break;
		case MVOperationCodes.GetNextResultSet:
			OnHandleGetNextResultSetResponse((Hashtable)parameters[1], (int)parameters[5], !(bool)parameters[7]);
			break;
		case MVOperationCodes.AddWorldObjectToInventory:
			OnAddWorldObjectToInventoryResponse(returnCode, (int)parameters[69], (int)parameters[38], (int)parameters[20]);
			break;
		case MVOperationCodes.AddWorldObjectToInventoryDev:
			if (returnCode != 0)
			{
				Debug.LogError((object)("Failed to AddWorldObjectToInventoryDev reason " + operationResponse.DebugMessage));
			}
			else if (returnCode != 0)
			{
				Debug.LogError((object)("Failed to AddWorldObjectToInventoryDev reason " + operationResponse.DebugMessage));
			}
			else
			{
				OnAddWorldObjectToInventoryResponseDev(returnCode, (int)parameters[20], (int)parameters[38]);
			}
			break;
		case MVOperationCodes.RequestFriendshipByName:
			OnRequestFriendshipResponse(returnCode);
			break;
		case MVOperationCodes.RequestFriendshipByProfileID:
			OnRequestFriendshipResponse(returnCode);
			break;
		case MVOperationCodes.Ungroup:
			OnUngroupResponse(returnCode == 0);
			break;
		case MVOperationCodes.LockHierarchy:
			OnLockHierarchyResponse(parameters, returnCode);
			break;
		case MVOperationCodes.RequestWoUniquePrototype:
			if (returnCode != 0)
			{
				OnRequestWoUniquePrototypeFailed(parameters);
			}
			break;
		case MVOperationCodes.AddLink:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"AddLink FAILED");
				this.worldNetwork.HandleAddLinkResponse(success: false, (int)parameters[56]);
			}
			else
			{
				this.worldNetwork.HandleAddLinkResponse(success: true, (int)parameters[56]);
			}
			break;
		case MVOperationCodes.RemoveLink:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"RemoveLink FAILED");
				this.worldNetwork.HandleRemoveLinkResponse(success: false);
			}
			else
			{
				Debug.Log((object)"RemoveLink SUCCESS");
				this.worldNetwork.HandleRemoveLinkResponse(success: true);
			}
			break;
		case MVOperationCodes.AddObjectLink:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"AddObjectLink FAILED");
				this.worldNetwork.HandleAddObjectLinkResponse(success: false, (int)parameters[56]);
			}
			else
			{
				this.worldNetwork.HandleAddObjectLinkResponse(success: true, (int)parameters[56]);
			}
			break;
		case MVOperationCodes.RemoveObjectLink:
			if (returnCode != 0)
			{
				Debug.LogWarning((object)"Remove ObjectLink FAILED");
				this.worldNetwork.HandleRemoveObjectLinkResponse(success: false);
			}
			else
			{
				this.worldNetwork.HandleRemoveObjectLinkResponse(success: true);
			}
			break;
		case MVOperationCodes.PurchaseItem:
			OnPurchaseItemResponse(returnCode);
			break;
		case MVOperationCodes.RequestTeamList:
			OnRequestTeamListResponse((Hashtable)parameters[91]);
			break;
		case MVOperationCodes.TransferWorldObjectsToGroup:
			this.worldNetwork.WorldObjectClientManagerNetwork.HandleTransferWorldObjectsToGroup(returnCode == 0);
			break;
		case MVOperationCodes.RequestMaterials:
			OnRequestMaterialsResponse((Hashtable)parameters[94]);
			break;
		case MVOperationCodes.PurchaseProduct:
			OnPurchaseProductResponse(returnCode, (Hashtable)parameters[96]);
			break;
		case MVOperationCodes.RentProduct:
			if (PurchaseProductResponseHandler != null)
			{
				PurchaseProductResponseHandler(returnCode, (Hashtable)parameters[97]);
			}
			break;
		case MVOperationCodes.ExpireProduct:
			OnExpireProductResponse(returnCode, parameters);
			break;
		case MVOperationCodes.CloneWorldObjectTree:
		{
			if (returnCode == -1)
			{
				Debug.LogWarning((object)"CloneWorldObjectTree failed. This should be handled in a general way!");
				if (MVGameController.Instance.GameMode == MVGameMode.Edit)
				{
					MVGameController.Instance.EditController.EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
				}
				break;
			}
			foreach (KeyValuePair<byte, object> item in parameters)
			{
				Debug.Log((object)("key " + item.Key));
				Debug.Log((object)("key " + item.Value));
			}
			Debug.Log((object)string.Empty);
			int num5 = (int)parameters[20];
			Debug.Log((object)("rootID " + num5));
			this.worldNetwork.WorldObjectClientManagerNetwork.OnCloneWorldObjectTreeResponse(returnCode == 0, num5);
			break;
		}
		case MVOperationCodes.RequestStreamingAssetList:
			OnRequestStreamingAssetListResponse(returnCode, parameters);
			break;
		case MVOperationCodes.RequestStreamingAssetInventory:
			OnRequestStreamingAssetInventoryResponse(returnCode, parameters);
			break;
		case MVOperationCodes.RequestStreamingAssetInventoryItems:
			OnRequestStreamingAssetInventoryItemsResponse(returnCode, parameters);
			break;
		case MVOperationCodes.GetActiveAvatar:
		{
			int num4 = (int)parameters[20];
			Debug.Log((object)string.Concat(new object[6]
			{
				"Operation response: ",
				MVOperationCodes.GetActiveAvatar,
				" returnCode: ",
				returnCode,
				" woid ",
				num4
			}));
			OnGetActiveAvatarResponse(returnCode, num4);
			break;
		}
		case MVOperationCodes.UploadScreenshot:
			if (ScreenshotUploaded != null)
			{
				ScreenshotUploaded(this, new ScreenshotUploadedEventArgs(returnCode == 0));
			}
			break;
		case MVOperationCodes.GetCreditStatus:
			if (returnCode == 0)
			{
				int silverAmount = (int)parameters[132];
				int goldAmount = (int)parameters[131];
				OnGetCreditStatus(silverAmount, goldAmount);
			}
			else
			{
				Debug.LogError((object)"GetCreditStatus operation failed");
			}
			break;
		case MVOperationCodes.GetDBTimeTicks:
			if (returnCode == 0)
			{
				localTimeInMillisecondsOnDBTimeSync = LocalTimeInMilliSeconds;
				long ticks = (long)parameters[134];
				dbTimeBase = new DateTime(ticks);
				GotoNextJoinState();
			}
			else
			{
				Debug.LogError((object)"GetDBTimeTicks operation failed");
			}
			break;
		case MVOperationCodes.AddItemToMarketPlace:
			if (returnCode == 0)
			{
				int num2 = (int)parameters[38];
				int num3 = (int)parameters[137];
				Debug.Log((object)$"ItemID: {num2} shopInventoryId {num3}");
				itemBusinessLogic.GetItem(num2).shopInventoryID = num3;
				PlayerRepository.PlayerInventory[num2].shopInventoryID = num3;
			}
			else
			{
				Debug.LogWarning((object)"Failed to add item to marketPlace");
			}
			if (OnMarketPlaceActionComplete != null)
			{
				OnMarketPlaceActionComplete(returnCode == 0);
			}
			break;
		case MVOperationCodes.RemoveItemFromMarketPlace:
			if (OnMarketPlaceActionComplete != null)
			{
				OnMarketPlaceActionComplete(returnCode == 0);
			}
			break;
		case MVOperationCodes.SetAvatarAccessorySlot:
			if (OnSetAvatarAccessoryResponse != null)
			{
				OnSetAvatarAccessoryResponse(returnCode == 0);
			}
			if (returnCode != 0)
			{
				Debug.LogError((object)"SetAvatarAccessorySlot operation failed");
			}
			break;
		case MVOperationCodes.CreateGameSnapshot:
		{
			WorldNetwork worldNetwork = this.worldNetwork;
			worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
			CreatePlayersFromUserList((Hashtable)parameters[13]);
			RequestGetNextGameBatch((int)parameters[100]);
			MVGameStateType gameStateType = (MVGameStateType)(int)parameters[64];
			int startTime = (int)parameters[66];
			int num = (int)parameters[65];
			MVGameStateReason reason = (MVGameStateReason)(int)parameters[67];
			Debug.Log((object)("DURING JOIN, game-state duration: " + num));
			networkGameStateListener.ChangeState(this, gameStateType, startTime, num, reason, 0);
			break;
		}
		case MVOperationCodes.AttachWorldObjectToSeat:
			PlayerController.HandleAttachWorldObjectToSeat(returnCode == 0);
			break;
		case MVOperationCodes.DetachWorldObjectFromVehicle:
			PlayerController.HandleDetachWorldObjectFromVehicle(returnCode == 0);
			break;
		case MVOperationCodes.SpawnVehicleWithDriver:
			PlayerController.HandleAttachWorldObjectToSeat(returnCode == 0);
			break;
		case MVOperationCodes.AddItemToWorld:
			if (returnCode == -1)
			{
				Debug.LogWarning((object)"Add item to world returned -1. This means that a singleton wo is already present and thus the request was rejected. We need a way to handle server operations in consistent manner.");
				if (MVGameController.Instance.GameMode == MVGameMode.Edit)
				{
					MVGameController.Instance.EditController.EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
				}
			}
			break;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
		switch (photonEvent.Code)
		{
		case byte.MaxValue:
		{
			string username = (string)photonEvent[9];
			int profileID3 = (int)photonEvent[11];
			int num6 = (int)photonEvent[254];
			if (num6 != LocalPlayerActorNumber)
			{
				AddPlayer(username, profileID3, num6, 0);
			}
			break;
		}
		case 254:
		{
			int num4 = (int)photonEvent[254];
			if (num4 != LocalPlayer.ActorNr)
			{
				Players.Remove(num4);
				if (onPlayerListChanged != null)
				{
					onPlayerListChanged();
				}
			}
			else
			{
				Debug.Log((object)"Local player leave event");
			}
			break;
		}
		case 1:
			logger.Log("UnregisterWorldObject event...");
			OnUnregisterWorldObjectEvent((int)photonEvent[20]);
			break;
		case 2:
			OnUpdateWorldObjectEvent(photonEvent);
			break;
		case 7:
			OnUpdateNetworkInputEvent(photonEvent);
			break;
		case 6:
			OnTransferOwnershipEvent(photonEvent);
			break;
		case 9:
			OnUnregisterPrototypeEvent((int)photonEvent[45]);
			break;
		case 10:
			worldNetwork.WorldInventory.OnUpdatePrototypeEvent((int)photonEvent[45], (byte[])photonEvent[47]);
			break;
		case 3:
			worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataEvent((int)photonEvent[20], (Hashtable)photonEvent[16]);
			break;
		case 4:
		{
			int worldObjectID9 = (int)photonEvent[20];
			Hashtable worldObjectData = (Hashtable)photonEvent[16];
			worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataPartialEvent(worldObjectID9, worldObjectData);
			break;
		}
		case 5:
		{
			int worldObjectID8 = (int)photonEvent[20];
			Hashtable worldObjectDataToRemove = (Hashtable)photonEvent[17];
			worldNetwork.WorldObjectClientManagerNetwork.OnRemoveWorldObjectDataPartialEvent(worldObjectID8, worldObjectDataToRemove);
			break;
		}
		case 32:
			if ((int)photonEvent[254] != LocalPlayer.ActorNr)
			{
				worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectRunTimeDataEvent((int)photonEvent[20], (Hashtable)photonEvent[70]);
			}
			break;
		case 13:
			logger.Log("Add link event...");
			OnAddLinkEvent((int)photonEvent[55], (int)photonEvent[54], (int)photonEvent[56]);
			break;
		case 14:
			logger.Log("Remove link event...");
			OnRemoveLinkEvent((int)photonEvent[56]);
			break;
		case 40:
			OnAddObjectLinkEvent((int)photonEvent[55], (int)photonEvent[54], (int)photonEvent[56]);
			break;
		case 41:
			OnRemoveObjectLinkEvent((int)photonEvent[56]);
			break;
		case 15:
		{
			logger.Log("Add item to inventory event...");
			int actorNr4 = (int)photonEvent[254];
			int itemID = (int)photonEvent[38];
			int itemCategoryID = (int)photonEvent[155];
			int itemTypeID = (int)photonEvent[39];
			string itemName = (string)photonEvent[40];
			byte[] itemData = (byte[])photonEvent[41];
			int slotIndex = (int)photonEvent[43];
			int worldObjectID7 = (int)photonEvent[20];
			bool isResellable = (bool)photonEvent[140];
			int authorProfileId = (int)photonEvent[139];
			int originalItemID = (int)photonEvent[141];
			int priceGold = (int)photonEvent[69];
			OnAddItemToInventoryEvent(actorNr4, itemID, itemCategoryID, itemTypeID, itemName, itemData, slotIndex, worldObjectID7, isResellable, authorProfileId, originalItemID, priceGold);
			break;
		}
		case 16:
			logger.Log("Remove item from inventory event...");
			OnRemoveItemFromInventory((int)photonEvent[38]);
			break;
		case 17:
		{
			int friendID2 = (int)photonEvent[50];
			int profileID2 = (int)photonEvent[11];
			int friendProfileID = (int)photonEvent[51];
			OnFriendRequestEvent(friendID2, profileID2, friendProfileID);
			break;
		}
		case 18:
		{
			int friendID = (int)photonEvent[50];
			int profileID = (int)photonEvent[11];
			FriendStatus status = (FriendStatus)(int)photonEvent[52];
			OnFriendUpdateEvent(friendID, profileID, status);
			break;
		}
		case 19:
		{
			int worldObjectID6 = (int)photonEvent[20];
			int actorNr3 = (int)photonEvent[254];
			OnTriggerBoxEnterEvent(actorNr3, worldObjectID6);
			break;
		}
		case 20:
		{
			int worldObjectID5 = (int)photonEvent[20];
			int actorNr2 = (int)photonEvent[254];
			OnTriggerBoxExitEvent(actorNr2, worldObjectID5);
			break;
		}
		case 21:
		{
			int worldObjectID4 = (int)photonEvent[20];
			int actorNr = (int)photonEvent[254];
			OnTriggerBoxStayBegin(worldObjectID4, actorNr);
			break;
		}
		case 22:
		{
			int worldObjectID3 = (int)photonEvent[20];
			OnTriggerBoxStayEnd(worldObjectID3);
			break;
		}
		case 23:
			Debug.Log((object)"Ungroup event...");
			OnUngroupEvent(photonEvent);
			break;
		case 25:
			OnLockHierarchyEvent(photonEvent);
			break;
		case 27:
			OnChatMsgEvent((int)photonEvent[254], (string)photonEvent[63]);
			break;
		case 28:
			OnWoUniquePrototypeEvent((int)photonEvent[20], (int)photonEvent[45]);
			break;
		case 29:
			OnGameStateChange((MVGameStateType)(int)photonEvent[64], (int)photonEvent[66], (int)photonEvent[65], (MVGameStateReason)(int)photonEvent[67], (int)photonEvent[254]);
			break;
		case 253:
		{
			int num5 = (int)photonEvent[253];
			Hashtable hashtable3 = (Hashtable)photonEvent[251];
			Debug.Log((object)("ACTOR-NR: " + num5));
			Debug.Log((object)Players[num5].Username);
			{
				foreach (string key in hashtable3.Keys)
				{
					Debug.Log((object)(key + ": " + hashtable3[key]));
				}
				break;
			}
		}
		case 30:
			OnSyncAvatarStatusEvent((int)photonEvent[254], (Hashtable)photonEvent[68]);
			break;
		case 31:
			OnResetLogicChunkEvent((int)photonEvent[20]);
			break;
		case 33:
			OnPickupItemStateChangeEvent((PickupItemState)(int)photonEvent[71], (int)photonEvent[20], (int)photonEvent[254]);
			break;
		case 34:
			OnRemoveCubesWithinRadiusEvent((float)photonEvent[73], new Vector3((float)photonEvent[22], (float)photonEvent[23], (float)photonEvent[24]), (float)photonEvent[80], (DamageFallOffType)(int)photonEvent[81]);
			break;
		case 35:
			OnUpdateLineOfFire(camOrigin: new Vector3((float)photonEvent[74], (float)photonEvent[75], (float)photonEvent[76]), camDir: new Vector3((float)photonEvent[77], (float)photonEvent[78], (float)photonEvent[79]), worldObjectID: (int)photonEvent[20]);
			break;
		case 36:
			OnWorldObjectRPCEvent(photonEvent);
			break;
		case 38:
			OnPostGameMsg((MVGameMsgType)(int)photonEvent[88], (Hashtable)photonEvent[89]);
			break;
		case 39:
			OnSetTeamEvent((int)photonEvent[254], (MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[90]));
			break;
		case 42:
			OnAddTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[90]));
			break;
		case 43:
			OnRemoveTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[90]));
			break;
		case 44:
			OnSetTeamDataEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[90]), (Hashtable)photonEvent[92]);
			break;
		case 45:
			OnTransferWorldObjectsToGroup(photonEvent);
			break;
		case 46:
			OnCloneWorldObjectTree(photonEvent);
			break;
		case 47:
			OnGetGameBatch(photonEvent);
			break;
		case 48:
			OnGameQueryReady(photonEvent);
			break;
		case 49:
			OnPostWinnerReportEvent(photonEvent);
			break;
		case 50:
			OnCollectiblePickedUp(photonEvent);
			break;
		case 51:
			OnResetTeamData((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[90]), (Hashtable)photonEvent[92]);
			break;
		case 52:
			OnSetWorldObjectsToPurchasedEvent((int)photonEvent[11], (int)photonEvent[38]);
			break;
		case 53:
			Debug.Log((object)$"Profile with ID {(int)photonEvent[11]} unlocked Achievement {(AchievementType)(int)photonEvent[130]}");
			break;
		case 54:
			Debug.Log((object)$"Received credit event update gold {(int)photonEvent[131]} silver {(int)photonEvent[132]}");
			OnCreditStatusEvent((int)photonEvent[132], (int)photonEvent[131]);
			break;
		case 55:
		{
			Hashtable hashtable2 = (Hashtable)photonEvent[72];
			int seatOwnerWoID = (int)hashtable2[(byte)4];
			int worldObjectID2 = (int)hashtable2[(byte)0];
			PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], seatOwnerWoID, worldObjectID2, (byte)photonEvent[143]);
			break;
		}
		case 56:
		{
			int num3 = (int)photonEvent[20];
			Debug.Log((object)("WorldObjectID " + num3));
			MVWorldObjectClient worldObjectClient2 = WorldObjectClientManager.GetWorldObjectClient(num3);
			if (worldObjectClient2 != null && worldObjectClient2 is MVAvatar)
			{
				((MVAvatar)worldObjectClient2).OnLeaveVehicle();
			}
			break;
		}
		case 57:
		{
			Hashtable hashtable = (Hashtable)photonEvent[72];
			int id = (int)hashtable[(byte)1];
			int worldObjectID = (int)hashtable[(byte)0];
			MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)WorldObjectClientManager.GetWorldObjectClient(id);
			int spawnWorldObjectID = mVWorldObjectSpawnerVehicle.SpawnWorldObjectID;
			int num2 = (int)hashtable[(byte)3];
			int ownerActorNumber = (int)photonEvent[254];
			int cloneLinkId = (int)photonEvent[56];
			int cloneObjectLinkId = (int)photonEvent[93];
			int takeTime = (int)photonEvent[33];
			worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, 0, cloneToRootGroup: true, spawnWorldObjectID, num2, cloneLinkId, cloneObjectLinkId);
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(num2);
			MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
			{
				wo.InteractionFlags = InteractionFlags.None;
			};
			worldObjectClient.TraverseRecursiveTail(callBack);
			PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], num2, worldObjectID, (byte)photonEvent[143]);
			mVWorldObjectSpawnerVehicle.Take(takeTime);
			break;
		}
		case 58:
		{
			int num = (int)photonEvent[145];
			RewardReason rewardReason = (RewardReason)(byte)photonEvent[147];
			RewardType rewardType = (RewardType)(byte)photonEvent[146];
			Debug.Log((object)$"Amount {num}, rewardReason {rewardReason}, rewardType {rewardType} ");
			BrowserComm.ToWeb.ExternalCall("refreshCredentials");
			break;
		}
		}
	}

	public void OnStatusChanged(StatusCode returnCode)
	{
		Debug.Log((object)("PeerStatusCallback():" + returnCode));
		switch (returnCode)
		{
		case StatusCode.Connect:
			JoinGame();
			break;
		case StatusCode.Disconnect:
			ConnState = MVConnState.Disconnected;
			break;
		case StatusCode.Exception:
			ConnState = MVConnState.Exception;
			break;
		case StatusCode.SendError:
			ConnState = MVConnState.SendError;
			break;
		case StatusCode.TimeoutDisconnect:
			ConnState = MVConnState.TimeoutDisconnect;
			break;
		case StatusCode.DisconnectByServerLogic:
			ConnState = MVConnState.Disconnected;
			break;
		default:
			Debug.LogWarning((object)("Unhandled PeerStatusCallback, returnCode: " + returnCode));
			break;
		}
	}

	public void DebugReturn(DebugLevel level, string debug)
	{
		Debug.LogError((object)debug);
	}
}
