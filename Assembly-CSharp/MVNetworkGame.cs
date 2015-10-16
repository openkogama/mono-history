using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using MV.WorldObject.Security;
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
					MVGameController.Game.RequestGetNextGameBatch(queryId);
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
			switch (gameDataQuery.QueryType)
			{
			case QueryType.GameWorld:
				MVGameController.Game.worldNetwork.CreateGameWorldFromQueryData(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
				break;
			case QueryType.AddToGameWorld:
				MVGameController.Game.worldNetwork.AddGameQueryDataToGameWorld(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
				break;
			case QueryType.Item:
				if (MVGameController.Game.ReceivedItemFromQuery != null)
				{
					ReceivedItemFromQueryEventArgs e = new ReceivedItemFromQueryEventArgs(gameDataQuery.GetBytePacker(), gameDataQuery.InstigatorActorNumber);
					MVGameController.Game.ReceivedItemFromQuery(this, e);
				}
				break;
			}
		}
	}

	public delegate void OnPlayerListChangedDelegate();

	public delegate void OnGetNextResultSetResponseDelegate(Dictionary<object, object> outData, int largeQueryId, bool isDone);

	public delegate void OnReceivedChatMessageDelegate(MVPlayer sender, string message);

	public delegate void OnReceivedGameMsgDelegate(MVGameMsgType type, Dictionary<object, object> gameMsgData);

	public delegate void OnReceivedXPDelegate(byte xpId, int actorNumber);

	public delegate void OnMarketPlaceActionCompleteDelegate(bool success);

	private const string appName = "MVGameServer";

	public const int numInventoryItemsPerBatch = 10;

	private const float ExpirationStateCheckPeriod = 1f;

	private const float ExpirationDataFetchCheckPeriod = 15f;

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(MVNetworkGame));

	private PhotonPeer peer;

	private MVConnState connState;

	private MVJoinState _joinState = MVJoinState.Joining;

	private MVGameType gameType;

	private SessionTime sessionTime;

	private MVItemBusinessLogic itemBusinessLogic = new MVItemBusinessLogic();

	private MVNetworkGameStateListener networkGameStateListener;

	private ItemCategories itemCategories;

	private bool isPublished;

	private OperationResponsePendingManager operationResponsePendingManager;

	private GameDataQueryManager gameDataQueryManager = new GameDataQueryManager();

	private MVGameCoinManager gameCoinManager;

	private int lastFrameServerTimeUpdate = -1;

	private int lastFrameLocalTimeUpdate = -1;

	private int serverTimeInMilliseconds;

	private int localTimeInMilliseconds;

	private DateTime dbTimeBase;

	private int localTimeInMillisecondsOnDBTimeSync;

	public Action<int, Dictionary<object, object>> PurchaseProductResponseHandler;

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

	private bool avatarResetPending;

	private MVLocalObjectController playerController;

	private PlayerRepository playerRepository;

	private ShopRepository shopRepository;

	private ShopRepository avatarShopRepository;

	private Dictionary<int, MVPlayer> players;

	private MVTeamManager teamManager = new MVTeamManager();

	private GameStatCounterManager gameStatCounterManager = new GameStatCounterManager();

	private WinningConditionManager winningConditionManager;

	private FriendList friends;

	private MVMaterialRepository materialRepository;

	private int localPlayerActorNumber = -1;

	private WorldNetwork worldNetwork;

	private MvAvatarMetaDataWoMap avatarMetaDataWoMap;

	public MVGameType GameType => gameType;

	public MVItemBusinessLogic ItemBusinessLogic => itemBusinessLogic;

	public MVGameCoinManager GameCoinManager => gameCoinManager;

	public ItemCategories ItemCategories => itemCategories;

	public SessionTime SessionTime => sessionTime;

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

	public bool IsPlaying
	{
		get
		{
			if (JoinState != MVJoinState.Playing)
			{
				return false;
			}
			return MVGameController.GameSessionData.gameMode == MVGameMode.Play || (MVGameController.GameSessionData.gameMode == MVGameMode.Edit && EditorController.PlayInEditor);
		}
	}

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

	public PhotonPeer Peer => peer;

	public ObscuredString XpKey { get; private set; }

	public int MarketPlaceLevel { get; private set; }

	public int PublishLevel { get; private set; }

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

	public AIngameController IngameController { get; private set; }

	public AEditController EditController => IngameController as AEditController;

	public CharacterEditorController CharacterEditorController
	{
		get
		{
			if (MVGameController.GameSessionData.gameMode == MVGameMode.CharacterEditor)
			{
				return IngameController as CharacterEditorController;
			}
			return null;
		}
	}

	public EditorController EditorController
	{
		get
		{
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
			{
				return IngameController as EditorController;
			}
			return null;
		}
	}

	public PlayControllerBase PlayController
	{
		get
		{
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Play)
			{
				return IngameController as PlayControllerBase;
			}
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
			{
				return ((EditorController)IngameController).PlayController;
			}
			Debug.LogError("PlayController does not exist");
			return null;
		}
	}

	public bool IsTouristSession => MVGameController.GameSessionData.profileID <= 0;

	public MVLocalPlayer LocalPlayer
	{
		get
		{
			players.TryGetValue(localPlayerActorNumber, out var value);
			return (MVLocalPlayer)value;
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

	public MvAvatarMetaDataWoMap AvatarMetaDataWoMap => avatarMetaDataWoMap;

	public MVTeamManager TeamManager => teamManager;

	public MVGameModeChangeNotifier GameStateController { get; private set; }

	public MVCameraController CameraController { get; set; }

	public FriendList Friends => friends;

	public MVLocalObjectController PlayerController => playerController;

	public Dictionary<int, MVPlayer> Players => players;

	public GameStatCounterManager GameStatCounterManager => gameStatCounterManager;

	public WinningConditionManager WinningConditionManager => winningConditionManager;

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
		CreateInGameController();
		peer = new PhotonPeer(this, ConnectionProtocol.Udp);
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

	private void CreateInGameController()
	{
		switch (MVGameController.GameSessionData.gameMode)
		{
		case MVGameMode.Play:
			if (IsTouristSession)
			{
				IngameController = new PlayControllerTourist();
			}
			else
			{
				IngameController = new PlayController();
			}
			break;
		case MVGameMode.Edit:
			IngameController = new EditorController();
			break;
		case MVGameMode.CharacterEditor:
			IngameController = new CharacterEditorController();
			break;
		}
	}

	private void networkGameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		if (JoinState == MVJoinState.Playing)
		{
			MVGameStateType currentGameState = networkGameStateListener.CurrentGameState;
			if (currentGameState == MVGameStateType.Round)
			{
				worldNetwork.WorldObjectClientManagerNetwork.ResetWorld();
				winningConditionManager.Reset();
			}
		}
	}

	public void Update()
	{
		try
		{
			UpdateGame();
		}
		catch (Exception ex)
		{
			if (Application.isEditor)
			{
				throw;
			}
			Debug.LogError("Exception in update loop: " + ex.ToString());
		}
	}

	public void FixedUpdate()
	{
		try
		{
			FixedUpdateGame();
		}
		catch (Exception ex)
		{
			if (Application.isEditor)
			{
				throw;
			}
			Debug.LogError("Exception in FixedUpdate loop: " + ex.ToString());
		}
	}

	public void Cleanup()
	{
		if (worldNetwork != null && worldNetwork.WorldObjectClientManagerNetwork != null)
		{
			worldNetwork.WorldObjectClientManagerNetwork.Cleanup();
		}
		IngameController = null;
	}

	private void UpdateGame()
	{
		if (peer != null)
		{
			Service();
			if (JoinState == MVJoinState.Playing)
			{
				CheckStreamingAsssetExpiration();
				worldNetwork.Update(this);
				gameCoinManager.Update(this);
			}
			networkGameStateListener.Update(this);
		}
	}

	private void FixedUpdateGame()
	{
		if (peer != null && JoinState == MVJoinState.Playing)
		{
			worldNetwork.FixedUpdate();
		}
	}

	public void Service()
	{
		if (peer != null)
		{
			peer.Service();
		}
	}

	private void GotoNextJoinState()
	{
		switch (JoinState)
		{
		case MVJoinState.Joining:
			Debug.Log("Profile ID " + LocalPlayer.Username);
			JoinState = MVJoinState.LoadGUI;
			LoadModeGui();
			break;
		case MVJoinState.LoadGUI:
			JoinState = MVJoinState.SynchronizingGameTime;
			GetCreditStatus();
			break;
		case MVJoinState.SynchronizingGameTime:
			peer.OpCustom(64, new Dictionary<byte, object>(), sendReliable: true);
			JoinState = MVJoinState.FetchingCreditStatus;
			break;
		case MVJoinState.FetchingCreditStatus:
		{
			JoinState = MVJoinState.FetchingMaterials;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, MVGameController.GameSessionData.profileID);
			peer.OpCustom(46, dictionary, sendReliable: true);
			break;
		}
		case MVJoinState.FetchingMaterials:
			JoinState = MVJoinState.FetchingItemTypes;
			RequestDBQuery(DBQuery.RequestItemCategories, new Dictionary<object, object>());
			break;
		case MVJoinState.FetchingItemTypes:
			JoinState = MVJoinState.FetchingOwnershipTypes;
			RequestDBQuery(DBQuery.RequestPlanetOwnershipTypes, new Dictionary<object, object>());
			break;
		case MVJoinState.FetchingOwnershipTypes:
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
			{
				JoinState = MVJoinState.FetchingStreamingAssetsAmbientAudio;
			}
			else
			{
				JoinState = MVJoinState.FetchingStreamingAssets;
			}
			RequestStreamingAssetList(StreamingAssetType.AmbientAudio, StreamingAssetType.AvatarAccessory);
			break;
		case MVJoinState.FetchingStreamingAssetsAmbientAudio:
			JoinState = MVJoinState.FetchingStreamingAssets;
			RequestStreamingAssetInventory(StreamingAssetType.AmbientAudio);
			break;
		case MVJoinState.FetchingStreamingAssets:
			JoinState = MVJoinState.FetchingStreamingAssetInventory;
			RequestStreamingAssetInventory(StreamingAssetType.AvatarAccessory);
			break;
		case MVJoinState.FetchingStreamingAssetInventory:
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
			{
				JoinState = MVJoinState.FetchingInventory;
				OnGetNextResultSetResponse = OnInventoryResultSetResponse;
				Dictionary<object, object> inData = new Dictionary<object, object>();
				RequestLargeDBQuery(MVOperationCodes.LargeDBQueryInventory, DBQuery.RequestInventory, inData, 10);
			}
			else if (MVGameController.GameSessionData.gameMode == MVGameMode.CharacterEditor)
			{
				JoinState = MVJoinState.FetchingAvatarShopInventory;
				OnGetNextResultSetResponse = OnAvatarShopInventoryResultSetResponse;
				Dictionary<object, object> inData2 = new Dictionary<object, object>();
				RequestLargeDBQuery(MVOperationCodes.LargeDBQueryAvatarShopInventory, DBQuery.RequestAvatarShopInventory, inData2, 25);
			}
			else
			{
				FetchGameSnapshot();
			}
			break;
		case MVJoinState.FetchingInventory:
			if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
			{
				JoinState = MVJoinState.FetchingShopInventory;
				OnGetNextResultSetResponse = OnShopInventoryResultSetResponse;
				Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
				dictionary2.Add((byte)0, LocalPlayer.ProfileID);
				RequestLargeDBQuery(MVOperationCodes.LargeDBQuery, DBQuery.RequestClientShopInventoryForPlayer, dictionary2, 10);
			}
			else
			{
				FetchGameSnapshot();
			}
			break;
		case MVJoinState.FetchingAvatarShopInventory:
			JoinState = MVJoinState.InitializeAvatarEdit;
			peer.OpCustom(77, new Dictionary<byte, object>(), sendReliable: true);
			break;
		case MVJoinState.InitializeAvatarEdit:
			FetchGameSnapshot();
			break;
		case MVJoinState.FetchingShopInventory:
			JoinState = MVJoinState.FetchingBuiltInItems;
			peer.OpCustom(63, new Dictionary<byte, object>(), sendReliable: true);
			break;
		case MVJoinState.FetchingBuiltInItems:
			FetchGameSnapshot();
			break;
		case MVJoinState.FetchingGameSnapShot:
			JoinState = MVJoinState.FetchingFriends;
			peer.OpCustom(20, new Dictionary<byte, object>(), sendReliable: true);
			break;
		case MVJoinState.FetchingFriends:
			JoinState = MVJoinState.SettingTeam;
			SelectTeamFromJoinFlow();
			break;
		case MVJoinState.SettingTeam:
			JoinState = MVJoinState.SettingActorReady;
			WorldObjectClientManager.AvatarLocal.Respawn(toHiddenState: true);
			CameraController.GetCamera<JetPackCamera>().SetCameraToAvatarEulerHack();
			SetActorReady();
			break;
		case MVJoinState.SettingActorReady:
			JoinState = MVJoinState.FetchingActiveAvatar;
			GotoNextJoinState();
			break;
		case MVJoinState.FetchingActiveAvatar:
			JoinState = MVJoinState.Playing;
			if (MVGameController.GameSessionData.gameMode == MVGameMode.CharacterEditor)
			{
				peer.OpCustom(61, new Dictionary<byte, object>(), sendReliable: true);
			}
			else
			{
				GotoNextJoinState();
			}
			break;
		case MVJoinState.Playing:
			StatHatWrapper.Value("JoinTime", Time.realtimeSinceStartup);
			if (IsTouristSession)
			{
				StatHatWrapper.Count("SessionType.Tourist" + gameType, 1);
			}
			else
			{
				StatHatWrapper.Count("SessionType." + gameType, 1);
			}
			gameCoinManager.Reset(this);
			break;
		case MVJoinState.SelectingTeam:
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
			return peer.Connect(MVGameController.GameSessionData.serverIP, "MVGameServer");
		}
		catch (SecurityException ex)
		{
			Debug.LogError("Security Exception (check policy file): " + ex);
			return false;
		}
		catch (Exception ex2)
		{
			Debug.LogError("Unknown exception during connect: '" + ex2.ToString() + "', Message: '" + ex2.Message + "'");
			return false;
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
			if (MVGameController.GameSessionData.gameMode != MVGameMode.CharacterEditor && body.AccessoriesLoaded)
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
			Debug.Log("Expired assets " + list.BuildString(eachEntryNewLine: false));
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
		peer.OpCustom(69, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void PublishPlanet()
	{
		if (MVGameController.Game == null || JoinState != MVJoinState.Playing || MVGameController.GameSessionData.gameMode != MVGameMode.Edit)
		{
			return;
		}
		if (IsOperationPending(MVOperationCodes.PublishPlanet))
		{
			Debug.LogWarning("Publish planet operation is pending. Aborting publish");
		}
		else if (LocalPlayer.Level < PublishLevel)
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("You can not publish game before reaching level: ") + PublishLevel, string.Empty, UXDialogType.Simple, noButtons: true, stackDialog: true).Show();
		}
		else if (!isPublished)
		{
			if (GenerateTextureData.IsCreatingScreenShot)
			{
				Debug.LogWarning("Texture is already being generated. Aborting publish");
			}
			else
			{
				GeneratePlanetScreenShot(PublishPlanet);
			}
		}
		else
		{
			PublishPlanet(new byte[0], ImageType.Planet, MVGameController.GameSessionData.planetID);
		}
	}

	public void UploadGameScreenShot()
	{
		if (MVGameController.Game.LocalPlayer.PlanetOwnershipTypeID != 2)
		{
			Debug.LogWarning("No screen shot when not planet owner");
		}
		else if (GenerateTextureData.IsCreatingScreenShot)
		{
			Debug.LogWarning("Texture is already being generated. Aborting UploadGameScreenShot");
		}
		else if (IsOperationPending(MVOperationCodes.UploadScreenshot))
		{
			Debug.LogWarning("UploadScreenshot operation is pending. Aborting UploadGameScreenShot");
		}
		else
		{
			GeneratePlanetScreenShot(UploadScreenshot);
		}
	}

	private static void GeneratePlanetScreenShot(Func<byte[], ImageType, int, bool> callback)
	{
		GameObject gameObject = new GameObject("GenerateTexture");
		GenerateTextureData generateTextureData = gameObject.AddComponent<GenerateTextureData>();
		generateTextureData.GenerateTextureDataCameraView(callback, ImageType.Planet, MVGameController.GameSessionData.planetID);
	}

	public bool IsOperationPending(MVOperationCodes operationCode)
	{
		return operationResponsePendingManager.IsOperationPending(operationCode);
	}

	private bool PublishPlanet(byte[] pngImageAsByteArray, ImageType image, int imageId)
	{
		MVGameController.Game.MaterialRepository.Validate();
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(62, pngImageAsByteArray);
		return operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.PublishPlanet, dictionary);
	}

	public void GetCreditStatus()
	{
		peer.OpCustom(62, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void SetActorReady()
	{
		peer.OpCustom(0, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void LocalPlayerLevelChanged(int level)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(170, level);
		peer.OpCustom(80, dictionary, sendReliable: true);
	}

	public void Ban(CheatType cheatType)
	{
		if (!IsTouristSession)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(179, (byte)cheatType);
			peer.OpCustom(82, dictionary, sendReliable: true);
			peer.SendOutgoingCommands();
		}
	}

	private void OnLocalPlayerXPProgress(XPProgressData xpProgressData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(85, xpProgressData.XpID);
		peer.OpCustom(81, dictionary, sendReliable: true);
	}

	public void AutoRegisterLocalPrototype(int woId, int worldInventoryID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(45, worldInventoryID);
		dictionary.Add(20, woId);
		peer.OpCustom(31, dictionary, sendReliable: true);
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
		MVGameController.Game.MaterialRepository.Validate();
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(42, itemTextureData);
		peer.OpCustom(57, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void AddWorldObjectToInventorDev(int worldObjectID, byte[] itemTextureData, string itemName, int itemCategory, bool overWrite)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(152, itemName);
		dictionary.Add(151, itemCategory);
		dictionary.Add(153, overWrite);
		dictionary.Add(42, itemTextureData);
		peer.OpCustom(58, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void RemoveItemFromInventory(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(18, dictionary, sendReliable: true);
	}

	public void UpdateInventorySlots(Dictionary<object, object> itemIdToSlotIndexTable)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(44, itemIdToSlotIndexTable);
		peer.OpCustom(19, dictionary, sendReliable: true);
	}

	public void SendClientLog(string logString, string stackTrace, LogType type, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(147, logString);
		dictionary.Add(148, stackTrace);
		dictionary.Add(149, (byte)type);
		dictionary.Add(154, extraSentryData);
		dictionary.Add(188, tags);
		peer.OpCustom(73, dictionary, sendReliable: true);
		peer.SendOutgoingCommands();
	}

	public void UpdateWorldObjectData(int worldObjectID, Dictionary<object, object> worldObjectData)
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
			Debug.LogError("Trying to update WO with an empty path key");
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(array[array.Length - 1], value);
		int num = array.Length - 2;
		while (0 <= num)
		{
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			dictionary2[array[num]] = dictionary;
			dictionary = dictionary2;
			num--;
		}
		Dictionary<byte, object> dictionary3 = new Dictionary<byte, object>();
		dictionary3.Add(20, worldObjectID);
		dictionary3.Add(16, dictionary);
		Debug.Log(dictionary3.BuildStringRecursive("Update wo " + worldObjectID + " partial data: "));
		peer.OpCustom(8, dictionary3, sendReliable: true);
	}

	public void UpdateWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woData)
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
			Debug.LogError("Trying to remoe WO data with an empty path key");
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		Dictionary<object, object> dictionary2 = dictionary;
		for (int i = 0; i < array.Length; i++)
		{
			if (i < array.Length - 1)
			{
				Dictionary<object, object> dictionary3 = new Dictionary<object, object>();
				dictionary2[array[i]] = dictionary3;
				dictionary2 = dictionary3;
			}
			else
			{
				dictionary2[array[i]] = string.Empty;
			}
		}
		Dictionary<byte, object> dictionary4 = new Dictionary<byte, object>();
		dictionary4.Add(20, worldObjectID);
		dictionary4.Add(17, dictionary);
		peer.OpCustom(9, dictionary4, sendReliable: true);
	}

	public void RemoveWorldObjectDataPartial(int worldObjectID, Dictionary<object, object> woDataToRemove)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(17, woDataToRemove);
		peer.OpCustom(9, dictionary, sendReliable: true);
	}

	public void WorldObjectRPC(int worldObjectID, Dictionary<object, object> dataPackage)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(82, dataPackage);
		peer.OpCustom(38, dictionary, sendReliable: true);
	}

	public void UpdateWorldObjectRunTimeData(int worldObjectID, Dictionary<object, object> worldObjectRunTimeData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(69, worldObjectRunTimeData);
		peer.OpCustom(35, dictionary, sendReliable: true);
	}

	public void RegisterWorldObject(WorldObjectType type, int groupId, Dictionary<object, object> woData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
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

	public void RequestBuiltInItem(BuiltInItem builtInItem, int groupId, Dictionary<object, object> customData, Vector3 position, Quaternion rotation, Vector3 scale, bool localOwner, bool transferOwnershipToServerOnLeave)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(115, builtInItem);
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
		peer.OpCustom(55, dictionary, sendReliable: true);
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
		dictionary.Add(125, isPreviewItem);
		peer.OpCustom(56, dictionary, sendReliable: true);
	}

	public void CloneWorldObjectTree(MVWorldObjectClient root, bool localOwner, bool setAsPreviewItem, bool cloneToRootGroup)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, root.Id);
		dictionary.Add(18, localOwner ? LocalPlayerActorNumber : 0);
		dictionary.Add(101, cloneToRootGroup);
		dictionary.Add(127, setAsPreviewItem);
		peer.OpCustom(45, dictionary, sendReliable: true);
	}

	public void AddPlanetToPlanet(int planetId, int subtreeId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(86, planetId);
		dictionary.Add(20, subtreeId);
		peer.OpCustom(47, dictionary, sendReliable: true);
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
			Debug.LogWarning("Ungroup failed");
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(27, dictionary, sendReliable: true);
	}

	public void ReportCaptureFlag()
	{
		peer.OpCustom(32, new Dictionary<byte, object>(), sendReliable: true);
	}

	public void SetActorProperty(Dictionary<object, object> properties)
	{
		Debug.LogError("Not implemented");
	}

	public void ResetLogicChunk(int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(34, dictionary, sendReliable: true);
	}

	public void OnUnregisterWorldObjectResponse(int worldObjectID)
	{
		if (!worldNetwork.OnUnregisterWorldObject(worldObjectID))
		{
			Debug.LogWarning("OnUnregisterWorldObjectResponse failed!");
		}
	}

	public void UpdateWorldObject(int id, Vector3 position, byte[] rotation, TransformPackageType packageType)
	{
		if (JoinState == MVJoinState.Playing)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, id);
			dictionary.Add(33, ServerTimeInMilliSeconds);
			dictionary.Add(22, position.x);
			dictionary.Add(23, position.y);
			dictionary.Add(24, position.z);
			dictionary.Add(158, rotation);
			dictionary.Add(34, (byte)packageType);
			bool sendReliable = TransformPackageType.Stop == packageType;
			peer.OpCustom(6, dictionary, sendReliable);
		}
	}

	public void UpdateLineOfFire(int worldObjectIDPickupOwner, Vector3 camDir, Vector3 camOrigin)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectIDPickupOwner);
		dictionary.Add(73, camOrigin.x);
		dictionary.Add(74, camOrigin.y);
		dictionary.Add(75, camOrigin.z);
		dictionary.Add(76, camDir.x);
		dictionary.Add(77, camDir.y);
		dictionary.Add(78, camDir.z);
		peer.OpCustom(37, dictionary, sendReliable: false);
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
		if (worldObjects.Contains(groupId))
		{
			Debug.LogError("Trying to transfer WO " + groupId + " to itself !");
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, groupId);
		dictionary.Add(71, worldObjects);
		peer.OpCustom(44, dictionary, sendReliable: true);
	}

	public void TransferOwnership(int worldObjectID, int ownerActorNr, Transform t)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		dictionary.Add(18, ownerActorNr);
		if (t == null)
		{
			dictionary.Add(83, false);
		}
		else
		{
			dictionary.Add(83, true);
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

	public void PostGameMsg(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(87, (int)gameMsgType);
		dictionary.Add(88, gameMsgData);
		peer.OpCustom(40, dictionary, sendReliable: true);
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
			UXUtils.UXDialogFactory.CreateDialog(TM._("Can't request friendship from yourself!"), string.Empty).Show();
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
			UXUtils.UXDialogFactory.CreateDialog(TM._("Can't request friendship from yourself!"), string.Empty).Show();
		}
	}

	public void OnSyncAvatarStatusEvent(int actorNr, Dictionary<object, object> data)
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
				Debug.LogError("PickUpItemStateChangeEvent failed, since WOID is not derived from MVPickupItemBase");
				return;
			}
			MVPickupItemBase mVPickupItemBase = (MVPickupItemBase)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVPickupItemBase.HandleStateChange(state);
		}
		else
		{
			Debug.LogWarning("OnPickupItemStateChangeEvent failed, since WorldObjectID does not exist. WOID: " + worldObjectID);
		}
	}

	public void OnUpdateLineOfFire(int worldObjectID, Vector3 camOrigin, Vector3 camDir)
	{
		MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
		MVPickupOwner component = worldObjectClient.GameObject.GetComponent<MVPickupOwner>();
		if (component == null)
		{
			Debug.LogError("Pickup owner not found");
		}
		else
		{
			component.SetLineOfFire(camOrigin, camDir);
		}
	}

	public List<CommonOverlapArg> GetWOIdsWithinRadius(float radius, Vector3 worldPos)
	{
		Collider[] array = Physics.OverlapSphere(worldPos, radius);
		List<CommonOverlapArg> list = new List<CommonOverlapArg>();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < array.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(array[i].transform);
			if (mVObject is MVCubeModelBase)
			{
				CommonOverlapArg item = new CommonOverlapArg(mVObject);
				list.Add(item);
				hashSet.Add(mVObject.Id);
			}
		}
		return list;
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

	public void OnPostGameMsg(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
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
		peer.OpCustom(31, dictionary, sendReliable: true);
	}

	public void JoinGame()
	{
		ConnState = MVConnState.Joining;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		Debug.Log("JoinGame " + MVGameController.GameSessionData.planetID);
		if (MVGameController.GameSessionData.gameMode == MVGameMode.CharacterEditor)
		{
			Debug.Log("Setting planetId to -1 as GameMode CharacterEditor is not using a planet");
		}
		dictionary.Add(byte.MaxValue, MVGameController.GameSessionData.planetName);
		dictionary.Add(86, MVGameController.GameSessionData.planetID);
		dictionary.Add(116, MVGameController.GameSessionData.gameMode);
		dictionary.Add(155, MVGameController.GameSessionData.language);
		dictionary.Add(168, MVGameController.GameSessionData.token);
		dictionary.Add(172, MVGameController.GameSessionData.newToken);
		dictionary.Add(173, MVGameController.GameSessionData.newPlanetName);
		BuildTarget buildTarget = BuildTarget.StandAlone;
		dictionary.Add(189, buildTarget);
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
			Debug.LogError("Attempt to add objectLink, but link not connected...");
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(55, link.objectConnectorWOID);
		dictionary.Add(54, link.objectWOID);
		peer.OpCustom(42, dictionary, sendReliable: true);
		worldNetwork.AddPendingObjectLink(link);
	}

	public void RemoveLink(int linkID)
	{
		if (worldNetwork.RemovePendingLink(linkID))
		{
			Debug.Log("RemoveLink");
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
			peer.OpCustom(43, dictionary, sendReliable: true);
		}
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

	public bool UploadScreenshot(byte[] textureData, ImageType imageType, int imageId)
	{
		MVGameController.Game.MaterialRepository.Validate();
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(118, textureData);
		dictionary.Add(119, imageId);
		dictionary.Add(117, (byte)imageType);
		return operationResponsePendingManager.AddOperationCodeToPending(MVOperationCodes.UploadScreenshot, dictionary);
	}

	public void PurchaseItem(int itemID, int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(36, dictionary, sendReliable: true);
	}

	private void PurchaseProduct(MVProductType productTypeID, Dictionary<object, object> productData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(94, (int)productTypeID);
		dictionary.Add(95, productData);
		peer.OpCustom(48, dictionary, sendReliable: true);
	}

	public void OnPurchaseProductResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		Debug.Log("PurchaseProductResponse");
		if (PurchaseProductResponseHandler != null)
		{
			PurchaseProductResponseHandler(returnCode, purchaseResponseData);
		}
	}

	public void PurchaseStreamingAsset(StreamingAssetType assetType, int streamingAssetID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)105] = streamingAssetID;
		dictionary[(byte)106] = assetType;
		PurchaseProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void UnlockMaterial(int materialID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)103, materialID);
		PurchaseProduct(MVProductType.MaterialUnlock, dictionary);
	}

	public void UnlockClientShopInventoryItem(int itemId)
	{
		Debug.Log("Purchase item with id: " + itemId);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)9, itemId);
		PurchaseProduct(MVProductType.Item, dictionary);
	}

	public void PurchaseAvatar(int avatarId)
	{
		PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)126, avatarId);
		PurchaseProduct(MVProductType.Avatar, dictionary);
	}

	public void PurchaseRespawnNow()
	{
		Debug.Log("Purchase respawn now");
		Dictionary<object, object> productData = new Dictionary<object, object>();
		PurchaseProduct(MVProductType.RespawnNow, productData);
	}

	public void PurchaseGameCoinBooster()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)186, PricesManager.GetPrice("GameCoinBoost").ToArray());
		PurchaseProduct(MVProductType.GameCoinBooster, dictionary);
	}

	public void SetGameCoinBoostState(bool gameCoinBoosterEnabled)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(184, gameCoinBoosterEnabled);
		peer.OpCustom(83, dictionary, sendReliable: true);
	}

	private void RentProduct(MVProductType productTypeID, Dictionary<object, object> productData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(94, (int)productTypeID);
		dictionary.Add(96, productData);
		peer.OpCustom(49, dictionary, sendReliable: true);
	}

	public void RentStreamingAsset(StreamingAssetType assetType, int assetID, int streamingAssetInventoryID = 0)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = assetType;
		dictionary[(byte)105] = assetID;
		dictionary[(byte)111] = streamingAssetInventoryID;
		RentProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void PurchaseAvatarAccessory(int streamingAssetID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		PurchaseProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void PurchaseAvatarAccessory(int streamingAssetID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		dictionary[(byte)20] = bodyWoID;
		dictionary[(byte)113] = slot;
		dictionary[(byte)114] = offset;
		PurchaseProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void UpdateAvatarAccessoryOffset(int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[126] = bodyWoID;
		dictionary[113] = (int)slot;
		dictionary[114] = offset;
		peer.OpCustom(74, dictionary, sendReliable: true);
	}

	public void RentAvatarAccessory(int streamingAssetID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		dictionary[(byte)20] = bodyWoID;
		dictionary[(byte)113] = slot;
		dictionary[(byte)114] = offset;
		RentProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void RentAvatarAccessory(int streamingAssetID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		RentProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		dictionary[(byte)111] = streamingAssetInventoryID;
		RentProduct(MVProductType.StreamingAsset, dictionary);
	}

	public void ExtendRentAvatarAccessory(int streamingAssetID, int streamingAssetInventoryID, int bodyWoID, AvatarAccessorySlot slot, float offset)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		dictionary[(byte)105] = streamingAssetID;
		dictionary[(byte)111] = streamingAssetInventoryID;
		dictionary[(byte)20] = bodyWoID;
		dictionary[(byte)113] = slot;
		dictionary[(byte)114] = offset;
		RentProduct(MVProductType.StreamingAsset, dictionary);
	}

	private void OnProductPurchaseAvatarResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnProductPurchaseAvatarResponse));
		Debug.Log("Avatar purchase response: " + (MVPurchaseReturnCode)returnCode);
		if (returnCode != 0)
		{
			WorldNetwork worldNetwork = this.worldNetwork;
			worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		}
	}

	private void ExpireProduct(MVProductType productTypeID, int productInventoryID, Dictionary<object, object> expireProductData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[94] = (int)productTypeID;
		dictionary[137] = productInventoryID;
		dictionary[97] = expireProductData;
		peer.OpCustom(50, dictionary, sendReliable: true);
	}

	public void ExpireStreamingAsset(StreamingAssetType assetType, int streamingAssetInventoryID)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)106, assetType);
		Dictionary<object, object> expireProductData = dictionary;
		ExpireProduct(MVProductType.StreamingAsset, streamingAssetInventoryID, expireProductData);
	}

	public void ExpireAvatarAccessory(int avatarAccessoryInventoryID, int bodyWoID = 0)
	{
		Debug.LogWarning("Expire accessory " + avatarAccessoryInventoryID + " on body " + bodyWoID);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary[(byte)106] = StreamingAssetType.AvatarAccessory;
		if (bodyWoID != 0)
		{
			dictionary[(byte)20] = bodyWoID;
		}
		ExpireProduct(MVProductType.StreamingAsset, avatarAccessoryInventoryID, dictionary);
	}

	private void OnExpireProductResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		MVProductType mVProductType = (MVProductType)(int)returnValues[94];
		int num = (int)returnValues[137];
		if (returnCode != 0)
		{
			Debug.LogError(string.Concat("Expiring product ", mVProductType, " with inventoryID ", num, " failed on server"));
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
				Debug.LogError("Cant find expiration infor for expired streaming asset with inventoryID " + num);
			}
		}
	}

	public void SetAvatarAccessorySlot(int avatarBodyWoID, int accessoryInventoryID, AvatarAccessorySlot slot, float slotOffset)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[126] = avatarBodyWoID;
		dictionary[111] = accessoryInventoryID;
		dictionary[113] = slot;
		dictionary[114] = slotOffset;
		peer.OpCustom(68, dictionary, sendReliable: true);
	}

	public void ResetAvatar(int AvatarID)
	{
		if (avatarResetPending)
		{
			Debug.LogWarning("Avatar reset is pending");
			return;
		}
		avatarResetPending = true;
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
		Debug.Log("Reset ActiveAvatar  called");
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(126, AvatarID);
		peer.OpCustom(60, dictionary, sendReliable: true);
	}

	private void InitializedResetAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
		avatarResetPending = false;
		if (e.RootWO != null)
		{
			Debug.Log("Reset avatar has been added to world. WorldObjectId is: " + e.RootWO);
			(IngameController as CharacterEditorController).SubstituteAvatar(e.RootWO.Id);
		}
	}

	private void InitializedPurchasedAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedPurchasedAvatar));
		if (e.RootWO != null)
		{
			Debug.Log("Purchased avatar has been added to world. WorldObjectId is: " + e.RootWO);
			(IngameController as CharacterEditorController).AddNewAvatar(e.RootWO.Id);
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(e.RootWO.Id);
			GameObject bodyCloneGO = UnityEngine.Object.Instantiate(worldObjectClient.GameObject);
			Action<Texture2D> screenShotDataTexHandler = (Texture2D pngData) =>
			{
				UploadScreenshot(pngData.EncodeToPNG(), ImageType.Avatar, LocalPlayer.ProfileID);
			};
			AvatarScreenshotGenerator.Generate(bodyCloneGO, screenShotDataTexHandler);
		}
	}

	public void SetActiveAvatar(int AvatarID)
	{
		Debug.Log("SetActiveAvatar  called");
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(126, AvatarID);
		peer.OpCustom(59, dictionary, sendReliable: true);
	}

	public void AddCloneToWorldObjects(MVWorldObjectClient wo)
	{
		worldNetwork.WorldObjectClientManagerNetwork.AddToWorldObjects(wo);
	}

	private Dictionary<byte, object> GetAttachWorldObjectToSeatData(VehicleSeatBase seatBase)
	{
		Vector3 localPosition = seatBase.gameObject.transform.localPosition;
		Quaternion localRotation = seatBase.gameObject.transform.localRotation;
		int seatID = seatBase.SeatID;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(22, localPosition.x);
		dictionary.Add(23, localPosition.y);
		dictionary.Add(24, localPosition.z);
		dictionary.Add(25, localRotation.x);
		dictionary.Add(26, localRotation.y);
		dictionary.Add(27, localRotation.z);
		dictionary.Add(28, localRotation.w);
		dictionary.Add(142, (byte)seatID);
		dictionary.Add(143, (byte)seatBase.SeatType);
		return dictionary;
	}

	public void AttachWorldObjectToSeat(int seatOwnerWoID, int worldObjectID, VehicleSeatBase seatBase)
	{
		Dictionary<byte, object> attachWorldObjectToSeatData = GetAttachWorldObjectToSeatData(seatBase);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)4, seatOwnerWoID);
		dictionary.Add((byte)0, worldObjectID);
		attachWorldObjectToSeatData.Add(71, dictionary);
		peer.OpCustom(70, attachWorldObjectToSeatData, sendReliable: true);
	}

	public void AddAvatarToAvatarShopInventory(int worldObjectId, int priceSilver, string name, byte[] imageData)
	{
		MaterialRepository.Validate();
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectId);
		dictionary.Add(131, priceSilver);
		dictionary.Add(167, name);
		dictionary.Add(118, imageData);
		peer.OpCustom(78, dictionary, sendReliable: true);
	}

	public void DeleteAvatarFromShopInventory(int worldObjectId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectId);
		peer.OpCustom(79, dictionary, sendReliable: true);
	}

	public void SpawnVehicleWithDriver(int worldObjectSpawnerVehicleID, int worldObjectID, VehicleSeatBase seatBase)
	{
		Dictionary<byte, object> attachWorldObjectToSeatData = GetAttachWorldObjectToSeatData(seatBase);
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, worldObjectSpawnerVehicleID);
		dictionary.Add((byte)0, worldObjectID);
		attachWorldObjectToSeatData.Add(71, dictionary);
		peer.OpCustom(72, attachWorldObjectToSeatData, sendReliable: true);
	}

	public void DetachWorldObjectFromVehicle(int worldObjectID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(20, worldObjectID);
		peer.OpCustom(71, dictionary, sendReliable: true);
	}

	private void OnJoinResponse(Dictionary<byte, object> returnValues)
	{
		Dictionary<object, object> prices = (Dictionary<object, object>)returnValues[183];
		PricesManager.Init(prices);
		Debug.Log("GameCoinBoost left " + (int)returnValues[180]);
		gameCoinManager = new MVGameCoinManager((int)returnValues[180]);
		MarketPlaceLevel = (int)returnValues[182];
		PublishLevel = (int)returnValues[185];
		XpKey = SecurityHelper.Decrypt((string)returnValues[178]);
		sessionTime = new SessionTime();
		if (!MVGameController.UsingDevSessionData)
		{
			new SessionLocatorPing();
		}
		InitializeManagers();
		int actorNumber = (int)returnValues[254];
		int planetOwnershipTypeID = (int)returnValues[14];
		LocalPlayerActorNumber = actorNumber;
		Debug.Log("Username " + (string)returnValues[9]);
		MVLocalPlayer mVLocalPlayer = ((!IsTouristSession) ? ((MVLocalPlayer)new MVLocalPlayerRegistered(actorNumber, MVGameController.GameSessionData.profileID, (string)returnValues[9], MVGameController.GameSessionData.language)) : ((MVLocalPlayer)new MVLocalPlayerTourist(actorNumber, MVGameController.GameSessionData.profileID, (string)returnValues[9], MVGameController.GameSessionData.language)));
		mVLocalPlayer.OnLevelChanged = (MVPlayer.OnLevelChangedDelegate)Delegate.Combine(mVLocalPlayer.OnLevelChanged, new MVPlayer.OnLevelChangedDelegate(LocalPlayerLevelChanged));
		mVLocalPlayer.PlanetOwnershipTypeID = planetOwnershipTypeID;
		AddPlayer(mVLocalPlayer);
		MVClientSettings.ClientSettingFlags = (ClientSettingFlags)(int)returnValues[169];
		isPublished = (bool)returnValues[81];
		gameType = (MVGameType)(int)returnValues[171];
		Debug.Log("GAME-TYPE = " + gameType);
		Debug.Log("Loading level joined");
		MVGameController.LevelLoader.LoadLevel("Joined", LoadMode.Overwrite, OnJoinedLevelLoaded);
		string apiUrl = (string)returnValues[175];
		string streamingAssetsUrl = (string)returnValues[104];
		if (Application.isEditor)
		{
			streamingAssetsUrl = (string)returnValues[104];
		}
		Urls.Init(apiUrl, streamingAssetsUrl);
		Debug.Log("Load language");
		TM.LoadLanguage(MVGameController.GameSessionData.language);
		Debug.Log("Initialize leveling manager");
		LevelingManager.Initialize(mVLocalPlayer.ProfileID);
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
		GameStateController = new MVGameModeChangeNotifier();
		teamManager.OnTeamAdded += gameStatCounterManager.OnTeamAdded;
		teamManager.OnTeamRemoved += gameStatCounterManager.OnTeamRemoved;
		winningConditionManager = new WinningConditionManagerClient(gameStatCounterManager);
	}

	public void OnJoinedLevelLoaded()
	{
		Debug.Log("LevelLoaded");
		GotoNextJoinState();
	}

	private void OnRequestMaterialsResponse(Dictionary<object, object> materialList)
	{
		foreach (byte key in materialList.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)materialList[key];
			string name = (string)dictionary[(byte)51];
			string description = (string)dictionary[(byte)52];
			string path = (string)dictionary[(byte)53];
			int materialSound = (int)dictionary[(byte)54];
			int modifierPackageType = (int)dictionary[(byte)55];
			string text = (string)dictionary[(byte)56];
			int priceGold = (int)dictionary[(byte)57];
			int priceSilver = (int)dictionary[(byte)58];
			bool isUnlocked = (bool)dictionary[(byte)59];
			float[] physicalProperties = (float[])dictionary[(byte)115];
			if (text.Length == 0)
			{
				MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, priceSilver, isUnlocked, physicalProperties);
			}
			else
			{
				MaterialRepository.AddMaterial(name, description, path, (MaterialSound)materialSound, (AvatarModifierPackageType)modifierPackageType, priceGold, priceSilver, isUnlocked, physicalProperties, Type.GetType(text));
			}
		}
		if (MaterialRepository.GetMaterial(21).IsDestructible || !MaterialRepository.GetMaterial(21).isUnlocked)
		{
			throw new Exception("Default material is invalid");
		}
		MVGameController.RegisterOverrideMaterials();
		GotoNextJoinState();
	}

	private void CreatePlayersFromUserList(Dictionary<object, object> userList)
	{
		string text = "Users in user list:\n";
		if (userList != null)
		{
			foreach (int key in userList.Keys)
			{
				string text2 = text;
				text = string.Concat(text2, (userList[key] as Dictionary<byte, object>)[9], " ", key, "\n");
				if (key != LocalPlayerActorNumber)
				{
					int profileID = (int)(userList[key] as Dictionary<byte, object>)[11];
					int team = (int)(userList[key] as Dictionary<byte, object>)[89];
					string userName = (string)(userList[key] as Dictionary<byte, object>)[9];
					int level = (int)(userList[key] as Dictionary<byte, object>)[170];
					string regionCode = (string)(userList[key] as Dictionary<byte, object>)[155];
					Dictionary<byte, object> dictionary = userList[key] as Dictionary<byte, object>;
					Debug.Log("testing " + dictionary);
					MVPlayer mVPlayer = new MVPlayer(key, profileID, userName, level, regionCode);
					mVPlayer.Team = (MVTeam)team;
					AddPlayer(mVPlayer);
				}
			}
		}
		else
		{
			Debug.LogError("UserList is null");
		}
		Debug.Log(text);
	}

	private void OnGetBuiltInItemBusinessData(Dictionary<byte, object> returnValues)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)returnValues[132];
		foreach (KeyValuePair<object, object> item in dictionary)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = (int)item.Key;
			Dictionary<object, object> dictionary2 = (Dictionary<object, object>)item.Value;
			mVItem.itemCategoryID = (int)dictionary2[(byte)116];
			mVItem.itemTypeID = (int)dictionary2[(byte)15];
			mVItem.name = (string)dictionary2[(byte)10];
			mVItem.resellable = (bool)dictionary2[(byte)104];
			itemBusinessLogic.AddItem(mVItem);
		}
		GotoNextJoinState();
	}

	private void OnRequestFriendsResponse(Dictionary<object, object> friendsList)
	{
		if (friendsList != null)
		{
			foreach (int key in friendsList.Keys)
			{
				Dictionary<object, object> dictionary = (Dictionary<object, object>)friendsList[key];
				int profileID = (int)dictionary[(byte)0];
				int friendProfileID = (int)dictionary[(byte)26];
				FriendStatus status = (FriendStatus)(int)dictionary[(byte)28];
				Friends.AddFriend(key, profileID, friendProfileID, status);
			}
		}
		else
		{
			Debug.LogWarning("Friendslist is null");
		}
		GotoNextJoinState();
	}

	private void WOCM_InitializedGameQueryDataHandler(object sender, InitializedGameQueryDataEventArgs e)
	{
		WorldNetwork worldNetwork = this.worldNetwork;
		worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
		if (e.RootWO is MVGroup)
		{
			WorldObjectClientManager.RootGroup = (MVGroup)e.RootWO;
		}
		else
		{
			Debug.LogError("RootGroup is not found!");
		}
		if (MVGameController.OnPostGameInit != null)
		{
			MVGameController.OnPostGameInit();
		}
		GotoNextJoinState();
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
		Debug.Log("TransferWosResponseHandler");
		if (!e.success)
		{
			Debug.LogError("Body transfer failed!");
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
		Debug.LogWarning("OnRequestWoUniquePrototypeFailed");
		int woId = (int)returnValues[20];
		worldNetwork.WorldInventory.UnpendRuntimePrototype(woId);
	}

	private void CreateTeamList(Dictionary<object, object> teamList)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)teamList[0];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)teamList[1];
		Dictionary<object, object> dictionary3 = (Dictionary<object, object>)teamList[2];
		Dictionary<object, object> dictionary4 = (Dictionary<object, object>)teamList[3];
		if ((bool)dictionary[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Blue);
		}
		if ((bool)dictionary2[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Red);
		}
		if ((bool)dictionary3[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Green);
		}
		if ((bool)dictionary4[(byte)0])
		{
			OnAddTeamEvent(MVTeam.Yellow);
		}
	}

	private void OnPurchaseItemResponse(int returnCode)
	{
		string message = TM._(string.Empty);
		switch (returnCode)
		{
		case 0:
			message = TM._("Item purchased");
			break;
		case -1:
			message = TM._("Undefined fail!");
			break;
		case -2:
			message = TM._("Not enough silver to purchase product");
			break;
		case -3:
			message = TM._("You cannot purchase the item,\nsince you are the owner");
			break;
		case -4:
			message = TM._("The item to purchase\nwas not found");
			break;
		case -5:
			message = TM._("The item is already in your inventory");
			break;
		}
		UXUtils.UXDialogFactory.CreateDialog(message, string.Empty).Show();
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
		if (JoinState != MVJoinState.Playing)
		{
			return;
		}
		int id = (int)photonEvent[20];
		if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
		{
			Debug.LogError("Attempt to update world object, but object not registered in world");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(id).State != MVWorldObjectState.Destroyed)
		{
			NetworkTransformPackage networkTransformPackage = new NetworkTransformPackage();
			networkTransformPackage.position = new Vector3((float)photonEvent[22], (float)photonEvent[23], (float)photonEvent[24]);
			networkTransformPackage.rotation = QuaternionCompression.ToQuaternion((byte[])photonEvent[158]);
			networkTransformPackage.timestamp = (int)photonEvent[33];
			networkTransformPackage.packageType = (TransformPackageType)(byte)photonEvent[34];
			if (WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject != null && WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject.GetType() == typeof(MVNetworkListener))
			{
				(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject as MVNetworkListener).AddTransformPackage(networkTransformPackage);
			}
			else if (WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject != null)
			{
				Debug.LogWarning(string.Concat("worldObjectClientManager.WorldObjects[worldObjectID].NetworkObject is ", WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject.GetType(), " this is probably due to ownership switching of vehicle"));
			}
		}
		else
		{
			Debug.LogWarning("Attempt to update world object, but object in destroyed state");
		}
	}

	private void OnWorldObjectRPCEvent(EventData photonEvent)
	{
		if (JoinState == MVJoinState.Playing)
		{
			int id = (int)photonEvent[20];
			if (WorldObjectClientManager.GetWorldObjectClient(id) == null)
			{
				Debug.LogError("Attempt to update world object, but object not registered in world");
			}
			else if (WorldObjectClientManager.GetWorldObjectClient(id).State != MVWorldObjectState.Destroyed)
			{
				int key = (int)photonEvent[254];
				MVPlayer p = Players[key];
				MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(id);
				worldObjectClient.ReceivePackage(p, (Dictionary<object, object>)photonEvent[82]);
			}
			else
			{
				Debug.LogWarning("Attempt to update world object, but object in destroyed state");
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
			Debug.LogError("Attempt to update network input on world object that is not in list");
			return;
		}
		if (!(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject is MVNetworkListener))
		{
			Debug.LogError("Attempt to update network input, but NetworkObject is not a listener");
		}
		NetworkInputPackage networkInputPackage = new NetworkInputPackage();
		networkInputPackage.actionCode = actionCode;
		networkInputPackage.keyCode = keyCode;
		networkInputPackage.timestamp = timestamp;
		(WorldObjectClientManager.GetWorldObjectClient(id).NetworkObject as MVNetworkListener).AddNetworkInputPackage(networkInputPackage);
	}

	private void OnTransferOwnershipEvent(EventData photonEvent)
	{
		bool flag = (bool)photonEvent[83];
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
		Debug.Log("Remove objectLink!");
		worldNetwork.RemoveObjectLink(linkID);
	}

	private void OnTriggerBoxEnterEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxEnterEvent received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnEnter(Players[actorNr]);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxExitEvent(int actorNr, int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxExitEvent received, but worldObjectID: " + worldObjectID + " does not exist");
		}
		else if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) is MVTriggerBox)
		{
			MVTriggerBox mVTriggerBox = (MVTriggerBox)WorldObjectClientManager.GetWorldObjectClient(worldObjectID);
			mVTriggerBox.OnExit(Players[actorNr]);
		}
		else
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxStayBegin(int worldObjectID, int actorNr)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " does not exist");
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
			Debug.LogError("OnTriggerBoxStayBegin received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnTriggerBoxStayEnd(int worldObjectID)
	{
		if (WorldObjectClientManager.GetWorldObjectClient(worldObjectID) == null)
		{
			Debug.LogError("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " does not exist");
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
			Debug.LogError("OnTriggerBoxStayEnd received, but worldObjectID: " + worldObjectID + " is not a triggerbox or a togglebox");
		}
	}

	private void OnAddItemToInventoryEvent(int actorNr, int itemID, int itemCategoryID, int itemTypeID, string itemName, byte[] itemData, int slotIndex, int worldObjectID, bool isResellable, int authorProfileId, int originalItemID, int priceGold)
	{
		if (PlayerRepository.PlayerInventory.ContainsKey(itemID))
		{
			Debug.LogWarning("Item already exists in inventory. Will be overwritten");
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
			Debug.Log("Data length: " + mVItem.data.Length);
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
				Debug.LogWarning("Attempted to assign itemId to worldObject failed. This is probably because the worldObject was deleted");
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
			Debug.LogError("Attempt to remove item to inventory, but itemID not in inventory");
		}
		else
		{
			PlayerRepository.RemoveItem(itemID);
		}
	}

	private void OnWoUniquePrototypeEvent(int woId, int worldInventoryId)
	{
		worldNetwork.WorldInventory.OnReplaceWoPrototype(woId, worldInventoryId);
	}

	private void OnGameStateChange(MVGameStateType gameStateType, int startTime, int duration, MVGameStateReason reason, int actorNr)
	{
		networkGameStateListener.ChangeState(this, gameStateType, startTime, duration, reason, actorNr);
	}

	private void AddPlayer(MVPlayer player)
	{
		if (Players.ContainsKey(player.ActorNr))
		{
			Debug.LogWarning("Duplicate player " + player.Username + " " + player.ActorNr);
			return;
		}
		Debug.Log("Adding player " + player.Username + " " + player.ActorNr);
		Players.Add(player.ActorNr, player);
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	private void SelectTeamFromJoinFlow()
	{
		List<MVTeam> teamList = TeamManager.GetTeamList();
		if (teamList.Count == 1 || MVGameController.GameSessionData.gameMode != MVGameMode.Play)
		{
			SetTeam(teamList[0]);
		}
		else
		{
			UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/TeamSelect/TeamSelectDialog", string.Empty, noButtons: true, stackDialog: false, canClose: false).SetOnResultCallback(TeamSelectCallBack).Show();
		}
	}

	private void TeamSelectCallBack(UXDialogBox dialog)
	{
		MVTeam team = (MVTeam)(int)dialog.GetResult();
		SetTeam(team);
	}

	public void ResetPlayer()
	{
		MVGameController.WOCM.AvatarLocal.Respawn(toHiddenState: true);
		worldNetwork.WorldObjectClientManagerNetwork.ResetLocalWorldObject();
		gameCoinManager.Reset(this);
		LocalPlayer.ResetCheckpoint();
		GameStatCounterManager.RemoveStatsFromActor(LocalPlayer.ActorNr);
	}

	public void SetTeam(MVTeam team)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(89, (int)team);
		peer.OpCustom(41, dictionary, sendReliable: true);
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

	public void OnSetWorldObjectsToPurchasedEvent(int purchaseProfileId, int itemId)
	{
		worldNetwork.WorldObjectClientManagerNetwork.OnSetWorldObjectsToPurchasedEvent(purchaseProfileId, itemId);
	}

	public void OnCreditStatusEvent(int silverAmount, int goldAmount)
	{
		BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
	}

	public void OnTransferWorldObjectsToGroup(EventData eventData)
	{
		int groupId = (int)eventData[20];
		int[] array = (int[])eventData[71];
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
		int[] array = (int[])eventData[71];
		int ownerActorNumber = (int)eventData[18];
		int cloneLinkId = (int)eventData[56];
		int cloneObjectLinkId = (int)eventData[92];
		bool cloneToRootGroup = (bool)eventData[101];
		int previewProfileOwnerId = (int)eventData[128];
		worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, previewProfileOwnerId, cloneToRootGroup, array[0], array[1], cloneLinkId, cloneObjectLinkId);
	}

	public void OnGetGameBatch(EventData eventData)
	{
		if (!eventData.Parameters.ContainsKey(245))
		{
			Debug.LogError("eventData.Contains((byte)MVParameterKeys.Data");
			return;
		}
		int instigator = (int)eventData[254];
		BytePacker bp = new BytePacker((byte[])eventData[245]);
		QueryType queryType = (QueryType)(byte)eventData[134];
		int queryId = -1;
		bool queryDataLeft = false;
		if (eventData.Parameters.ContainsKey(99))
		{
			queryId = (int)eventData[99];
		}
		if (eventData.Parameters.ContainsKey(100))
		{
			queryDataLeft = (bool)eventData[100];
		}
		gameDataQueryManager.HandleDataBatch(instigator, queryId, queryType, queryDataLeft, bp);
	}

	private void OnGameQueryReady(EventData eventData)
	{
		int queryId = (int)eventData[99];
		gameDataQueryManager.OnGameQueryReady(queryId);
	}

	private void OnPostWinnerReportEvent()
	{
		IWinningCondition winningCondition = null;
		if (winningConditionManager.WinningConditionFound)
		{
			List<IWinningCondition> forfilledWinningConditions = winningConditionManager.GetForfilledWinningConditions();
			if (forfilledWinningConditions.Count == 0)
			{
				Debug.LogError("No winning condition found even though server reported game ended");
				return;
			}
			if (forfilledWinningConditions.Count > 1)
			{
				Debug.LogError("Only 1 winning condition currently supported");
				return;
			}
			winningCondition = forfilledWinningConditions[0];
		}
		else
		{
			Debug.LogError("Did not find winner condition");
		}
		PlayController.OnWinningConditionReceived(winningCondition);
	}

	private void OnCollectiblePickedUp(EventData photonEvent)
	{
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
				Debug.LogError("Attempt to call WO that is not collectible, OnCollectiblePickedUpEvent");
			}
		}
	}

	private void RequestGetNextGameBatch(int queryId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[99] = queryId;
		peer.OpCustom(51, dictionary, sendReliable: true);
	}

	private void RequestStreamingAssetList(params StreamingAssetType[] assetTypes)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		int[] value = assetTypes.Select((StreamingAssetType type) => (int)type).ToArray();
		dictionary.Add(107, value);
		peer.OpCustom(52, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetListResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (returnCode != 0)
		{
			Debug.LogError("RequestStreamingAssetList failed. returnCode: " + returnCode);
		}
		else
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)returnValues[108];
			foreach (int key in dictionary.Keys)
			{
				Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[key];
				StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
				streamingAssetInfo.ProductID = key;
				streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)dictionary2[(byte)64];
				streamingAssetInfo.CategoryID = (int)dictionary2[(byte)66];
				streamingAssetInfo.Name = (string)dictionary2[(byte)67];
				streamingAssetInfo.Desc = (string)dictionary2[(byte)68];
				streamingAssetInfo.AssetPath = (string)dictionary2[(byte)69];
				StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
				hashSet.Add((int)streamingAssetInfo.StreamedAssetType);
				ProductShopInfo productShopInfo = null;
				if (dictionary2.ContainsKey((byte)76))
				{
					productShopInfo = new ProductShopInfo();
					productShopInfo.PriceGold = (int)dictionary2[(byte)76];
					productShopInfo.PriceSilver = (int)dictionary2[(byte)77];
					productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
					productShopInfo.RentExpireSeconds = (int)dictionary2[(byte)80];
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
		dictionary.Add(107, value);
		peer.OpCustom(53, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetInventoryResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		if (returnCode != 0)
		{
			Debug.LogError("RequestStreamingAssetInventory failed. returnCode: " + returnCode);
		}
		else
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)returnValues[109];
			foreach (int key in dictionary.Keys)
			{
				Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[key];
				int num2 = key;
				DateTime purchaseTime = new DateTime((long)dictionary2[(byte)83]);
				bool isRented = (bool)dictionary2[(byte)81];
				int num3 = (int)dictionary2[(byte)62];
				StreamingAssetInfo value = null;
				StreamingAssetInfoMap.TryGetValue(num3, out value);
				if (value == null)
				{
					Debug.LogError("Missing asset info " + num3 + " for asset " + num2);
				}
				else
				{
					ProductInventoryInfo invInfo = new ProductInventoryInfo(num2, value, purchaseTime, isRented);
					StreamingAssetInventory.Add(invInfo);
				}
			}
		}
		StreamingAssetInventory.NotifyProductInventoryChange();
		GotoNextJoinState();
	}

	public void RequestStreamingAssetInventoryItems(int[] inventoryIDs)
	{
		Debug.LogWarning("Request SA inventory items " + inventoryIDs.BuildString(null, eachEntryNewLine: false));
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(112, inventoryIDs);
		peer.OpCustom(54, dictionary, sendReliable: true);
	}

	private void OnRequestStreamingAssetInventoryItemsResponse(int returnCode, Dictionary<byte, object> returnValues)
	{
		int[] array = (int[])returnValues[112];
		int[] array2 = array;
		foreach (int item in array2)
		{
			ownedRequestedIDs.Remove(item);
		}
		if (returnCode != 0)
		{
			Debug.LogError("RequestStreamingAssetInventoryItem failed. returnCode: " + returnCode);
			int[] array3 = array;
			foreach (int item2 in array3)
			{
				ownedRequestFailedIDs.Add(item2);
			}
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)returnValues[110];
		int[] array4 = array;
		foreach (int num in array4)
		{
			if (!dictionary.ContainsKey(num))
			{
				ownedRequestFailedIDs.Add(num);
				continue;
			}
			Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[num];
			StreamingAssetInfo streamingAssetInfo = new StreamingAssetInfo();
			streamingAssetInfo.ProductID = (int)dictionary2[(byte)62];
			streamingAssetInfo.StreamedAssetType = (StreamingAssetType)(int)dictionary2[(byte)64];
			streamingAssetInfo.CategoryID = (int)dictionary2[(byte)66];
			streamingAssetInfo.Name = (string)dictionary2[(byte)67];
			streamingAssetInfo.Desc = (string)dictionary2[(byte)68];
			streamingAssetInfo.AssetPath = (string)dictionary2[(byte)69];
			if (!StreamingAssetInfoMap.ContainsKey(streamingAssetInfo.ProductID))
			{
				StreamingAssetInfoMap.Add(streamingAssetInfo.ProductID, streamingAssetInfo);
			}
			ProductShopInfo productShopInfo = null;
			if (dictionary2.ContainsKey((byte)76))
			{
				productShopInfo = new ProductShopInfo();
				productShopInfo.PriceGold = (int)dictionary2[(byte)76];
				productShopInfo.PriceSilver = (int)dictionary2[(byte)77];
				productShopInfo.IsBuyable = productShopInfo.PriceGold != 0 || productShopInfo.PriceSilver != 0;
				productShopInfo.RentExpireSeconds = (int)dictionary2[(byte)80];
				streamingAssetInfo.ShopInfo = productShopInfo;
			}
			if (streamingAssetInfo.ShopInfo != null && !StreamingAssetShopInventory.Contains(streamingAssetInfo.ProductID))
			{
				StreamingAssetShopInventory.Add(streamingAssetInfo);
			}
			if (!StreamingAssetInventory.Contains(num))
			{
				DateTime purchaseTime = new DateTime((long)dictionary2[(byte)83]);
				bool isRented = (bool)dictionary2[(byte)81];
				ProductInventoryInfo invInfo = new ProductInventoryInfo(num, streamingAssetInfo, purchaseTime, isRented);
				StreamingAssetInventory.Add(invInfo);
			}
			ownedRequestedIDs.Remove(num);
		}
	}

	private void OnGetCreditStatus(int silverAmount, int goldAmount)
	{
		GotoNextJoinState();
	}

	private void OnGetActiveAvatarResponse(int returnCode, int woid)
	{
		(IngameController as CharacterEditorController).SetActiveAvatar(woid);
		GotoNextJoinState();
	}

	public void OnSetTeamEvent(int actorNr, MVTeam team)
	{
		if (!Players.ContainsKey(actorNr))
		{
			Debug.LogError("!Players.ContainsKey(actorNr): " + actorNr);
		}
		Players[actorNr].Team = team;
		GameStatCounterManager.RemoveStatsFromActor(actorNr);
		if (LocalPlayer.ActorNr == actorNr)
		{
			if (MVGameController.Game.IsPlaying)
			{
				MVGameController.Game.ResetPlayer();
			}
			if (JoinState == MVJoinState.SettingTeam)
			{
				GotoNextJoinState();
			}
		}
		else if (Players[actorNr].Avatar != null)
		{
			Players[actorNr].Avatar.SetTeam();
		}
		if (onPlayerListChanged != null)
		{
			onPlayerListChanged();
		}
	}

	public void RequestDBQuery(DBQuery query, Dictionary<object, object> inData)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(0, (byte)query);
		dictionary.Add(2, inData);
		peer.OpCustom(1, dictionary, sendReliable: true);
	}

	public void RequestResetTerrain()
	{
		Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
		peer.OpCustom(76, customOpParameters, sendReliable: true);
	}

	public void SendRuntimeEventOperation(RuntimeEvent runtimeEvent)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(245, runtimeEvent.Data);
		peer.OpCustom(75, dictionary, sendReliable: true);
	}

	public void OnDBQueryResponse(Dictionary<object, object> outData)
	{
		if (outData == null)
		{
			Debug.LogError("OnDBQueryResponse: outData is null");
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
		Debug.Log("DBQuery failed during GameState '" + JoinState.ToString() + "'. Reason: " + reason);
		if (JoinState != MVJoinState.Playing)
		{
			GotoNextJoinState();
		}
	}

	public void RequestMarketPlaceItem(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(65, dictionary, sendReliable: true);
	}

	public void RequestAddItemToMarketPlace(int itemID, string itemName, string itemDescription, int silverPrice)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		dictionary.Add(40, itemName);
		dictionary.Add(135, itemDescription);
		dictionary.Add(68, silverPrice);
		peer.OpCustom(66, dictionary, sendReliable: true);
	}

	public void RequestRemoveItemFromMarketPlace(int itemID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(38, itemID);
		peer.OpCustom(67, dictionary, sendReliable: true);
	}

	public void RequestLargeDBQuery(MVOperationCodes operationCode, DBQuery query, Dictionary<object, object> inData, int numRowsPerReturn)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(0, (byte)query);
		dictionary.Add(2, inData);
		dictionary.Add(6, numRowsPerReturn);
		peer.OpCustom((byte)operationCode, dictionary, sendReliable: true);
	}

	public void OnLargeDBQueryResponse(int largeDBQueryID)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(5, largeDBQueryID);
		peer.OpCustom(3, dictionary, sendReliable: true);
	}

	private void OnInventoryResultSetResponse(Dictionary<object, object> outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)outData[key];
			MVItem mVItem = new MVItem(key, dictionary);
			if (!mVItem.isDeleted)
			{
				PlayerRepository.PlayerInventory.Add(mVItem.itemID, mVItem);
				int num2 = (int)dictionary[(byte)22];
				PlayerRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
			}
			itemBusinessLogic.AddItem(mVItem);
		}
		PlayerRepository.NotifyRepositoryChange();
	}

	private void OnShopInventoryResultSetResponse(Dictionary<object, object> outData, int largeQueryId, bool isDone)
	{
		ShopRepository shopRepository = ShopRepository;
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.itemCategoryID = (int)((Dictionary<object, object>)outData[key])[(byte)116];
			mVItem.itemTypeID = (int)((Dictionary<object, object>)outData[key])[(byte)15];
			mVItem.name = (string)((Dictionary<object, object>)outData[key])[(byte)10];
			mVItem.description = (string)((Dictionary<object, object>)outData[key])[(byte)107];
			mVItem.data = (byte[])((Dictionary<object, object>)outData[key])[(byte)11];
			mVItem.resellable = (bool)((Dictionary<object, object>)outData[key])[(byte)104];
			int priceSilver = (int)((Dictionary<object, object>)outData[key])[(byte)77];
			int priceGold = (int)((Dictionary<object, object>)outData[key])[(byte)76];
			mVItem.priceSilver = priceSilver;
			mVItem.priceGold = priceGold;
			ShopRepository.ShopInventory.Add(mVItem.itemID, mVItem);
			int num2 = (int)((Dictionary<object, object>)outData[key])[(byte)101];
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

	private void OnAvatarShopInventoryResultSetResponse(Dictionary<object, object> outData, int largeQueryId, bool isDone)
	{
		foreach (int key in outData.Keys)
		{
			MVItem mVItem = new MVItem();
			mVItem.itemID = key;
			mVItem.data = (byte[])((Dictionary<object, object>)outData[key])[(byte)92];
			mVItem.name = "Avatar " + key;
			int priceSilver = (int)((Dictionary<object, object>)outData[key])[(byte)77];
			int priceGold = (int)((Dictionary<object, object>)outData[key])[(byte)76];
			mVItem.priceSilver = priceSilver;
			mVItem.priceGold = priceGold;
			int num2 = (int)((Dictionary<object, object>)outData[key])[(byte)101];
			AvatarShopRepository.ShopInventory.Add(mVItem.itemID, mVItem);
			AvatarShopRepository.itemIDToInventorySlotIndex.Add(mVItem.itemID, num2);
		}
		AvatarShopRepository.NotifyRepositoryChange();
	}

	public void OnHandleGetNextResultSetResponse(Dictionary<object, object> outData, int largeQueryId, bool isDone)
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
		string message = string.Empty;
		ValueInsert values = null;
		switch (returnCode)
		{
		case 0:
			message = TM._("Successfully added model to your inventory");
			break;
		case -1:
			message = TM._("Undefined fail!");
			break;
		case -5:
			message = TM._("Failed to add to inventory");
			break;
		case -4:
			message = TM._("Failed to create item");
			break;
		case -3:
			message = TM._("You are not the creator of this model. \n\nYou can only add models to your inventory, if they are created by you");
			break;
		case -6:
			message = TM._("You are not the creator of this model. \n\nBuy the model to add it to your Inventory\nPrice: {0} silver");
			values = new ValueInsert().AddInt(price);
			flag = true;
			break;
		case -7:
			message = TM._("The item is already in your inventory");
			break;
		case -2:
			message = TM._("Prototype not found");
			break;
		}
		if (!flag)
		{
			UXUtils.UXDialogFactory.CreateDialog(message, TM._("Add To Inventory")).Show();
			return;
		}
		UXUtils.UXDialogFactory.CreateDialog(message, TM._("Buy Model ?"), UXDialogType.Simple, noButtons: false, stackDialog: false, canClose: true, values).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
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
		UXUtils.UXDialogFactory.CreateDevelopmentDialog(empty, string.Empty).Show();
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
			Debug.LogError("OperationResponse " + text);
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
			ConnState = MVConnState.Joined;
			switch (returnCode)
			{
			case 0:
				OnJoinResponse(parameters);
				break;
			case -12:
				if (MVGameController.TryReauth())
				{
					break;
				}
				goto default;
			default:
				Debug.Log("Quiting from join because of of ping not ok and out of reauth");
				MVGameController.ApplicationQuit(new QuitConnectionError());
				break;
			}
			break;
		case MVOperationCodes.PublishPlanet:
		{
			string empty = string.Empty;
			switch (returnCode)
			{
			case 0:
				empty = TM._("Successfully published planet");
				isPublished = true;
				break;
			case -1:
				empty = TM._("Undefined fail!");
				break;
			case -2:
				empty = TM._("You are not authorized to publish this planet");
				break;
			default:
				empty = TM._("Unhandled returnCode");
				break;
			}
			UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory.CreateDialog(empty, string.Empty);
			if (uXDialogFactory != null)
			{
				uXDialogFactory.Show();
			}
			if (!string.IsNullOrEmpty(MVGameController.GameSessionData.gamePublishedURL))
			{
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("token", MVGameController.GameSessionData.token);
				wWWForm.AddField("profile_id", MVGameController.GameSessionData.profileID);
				wWWForm.AddField("planet_id", MVGameController.GameSessionData.planetID);
				AsyncWWWManager.WWWRequest(new PostRequest(MVGameController.GameSessionData.gamePublishedURL, wWWForm, null));
			}
			else
			{
				Debug.LogWarning("s.gamePublishedURL is null or empty string");
			}
			break;
		}
		case MVOperationCodes.SetActorReady:
			if (JoinState == MVJoinState.SettingActorReady)
			{
				GotoNextJoinState();
			}
			else
			{
				Debug.LogError("SetActorReady returned, but we're not in SettingActorReadyState, but in " + JoinState);
			}
			break;
		case MVOperationCodes.GetBuiltInItemBusinessData:
			OnGetBuiltInItemBusinessData(parameters);
			break;
		case MVOperationCodes.RequestFriends:
			OnRequestFriendsResponse((Dictionary<object, object>)parameters[49]);
			break;
		case MVOperationCodes.UnregisterWorldObject:
			if (returnCode != 0)
			{
				Debug.LogWarning("Failed to unregister worldObject");
			}
			else
			{
				OnUnregisterWorldObjectResponse((int)parameters[20]);
			}
			break;
		case MVOperationCodes.UpdateWorldObjectData:
			if (returnCode != 0)
			{
				Debug.LogError("UpdateWorldObjectData FAILED on server!");
			}
			break;
		case MVOperationCodes.UpdateWorldObjectDataPartial:
			if (returnCode != 0)
			{
				Debug.LogError("UpdateWorldObjectDataPartial FAILED on server!");
			}
			break;
		case MVOperationCodes.UpdateWorldObjectRunTimeData:
			if (returnCode != 0)
			{
				Debug.LogError("UpdateWorldObjectRunTimeData FAILED on server!");
			}
			break;
		case MVOperationCodes.TransferOwnership:
			OnTransferOwnershipResponse(parameters, returnCode);
			break;
		case MVOperationCodes.DBQuery:
			OnDBQueryResponse((Dictionary<object, object>)parameters[1]);
			break;
		case MVOperationCodes.LargeDBQuery:
			if (returnCode == 0)
			{
				OnLargeDBQueryResponse((int)parameters[5]);
			}
			else
			{
				Debug.LogError("LargeDBQuery response failed...");
			}
			break;
		case MVOperationCodes.GetNextResultSet:
			OnHandleGetNextResultSetResponse((Dictionary<object, object>)parameters[1], (int)parameters[5], !(bool)parameters[7]);
			break;
		case MVOperationCodes.AddWorldObjectToInventory:
			OnAddWorldObjectToInventoryResponse(returnCode, (int)parameters[68], (int)parameters[38], (int)parameters[20]);
			break;
		case MVOperationCodes.AddWorldObjectToInventoryDev:
			if (returnCode != 0)
			{
				Debug.LogError("Failed to AddWorldObjectToInventoryDev reason " + operationResponse.DebugMessage);
			}
			else if (returnCode != 0)
			{
				Debug.LogError("Failed to AddWorldObjectToInventoryDev reason " + operationResponse.DebugMessage);
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
				Debug.LogWarning("AddLink FAILED");
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
				Debug.LogWarning("RemoveLink FAILED");
				this.worldNetwork.HandleRemoveLinkResponse(success: false);
			}
			else
			{
				Debug.Log("RemoveLink SUCCESS");
				this.worldNetwork.HandleRemoveLinkResponse(success: true);
			}
			break;
		case MVOperationCodes.AddObjectLink:
			if (returnCode != 0)
			{
				Debug.LogWarning("AddObjectLink FAILED");
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
				Debug.LogWarning("Remove ObjectLink FAILED");
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
		case MVOperationCodes.TransferWorldObjectsToGroup:
			this.worldNetwork.WorldObjectClientManagerNetwork.HandleTransferWorldObjectsToGroup(returnCode == 0);
			break;
		case MVOperationCodes.RequestMaterials:
			OnRequestMaterialsResponse((Dictionary<object, object>)parameters[93]);
			break;
		case MVOperationCodes.PurchaseProduct:
		{
			Dictionary<object, object> purchaseResponseData = null;
			if (parameters.ContainsKey(95))
			{
				purchaseResponseData = (Dictionary<object, object>)parameters[95];
			}
			OnPurchaseProductResponse(returnCode, purchaseResponseData);
			break;
		}
		case MVOperationCodes.RentProduct:
		{
			Dictionary<object, object> arg = null;
			if (parameters.ContainsKey(96))
			{
				arg = (Dictionary<object, object>)parameters[96];
			}
			PurchaseProductResponseHandler(returnCode, arg);
			break;
		}
		case MVOperationCodes.ExpireProduct:
			OnExpireProductResponse(returnCode, parameters);
			break;
		case MVOperationCodes.CloneWorldObjectTree:
		{
			if (returnCode == -1)
			{
				Debug.LogWarning("CloneWorldObjectTree failed. This should be handled in a general way!");
				if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
				{
					EditController.EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
				}
				break;
			}
			foreach (KeyValuePair<byte, object> item in parameters)
			{
				Debug.Log("key " + item.Key);
				Debug.Log("key " + item.Value);
			}
			Debug.Log(string.Empty);
			int num4 = (int)parameters[20];
			Debug.Log("rootID " + num4);
			this.worldNetwork.WorldObjectClientManagerNetwork.OnCloneWorldObjectTreeResponse(returnCode == 0, num4);
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
			int num3 = (int)parameters[20];
			Debug.Log(string.Concat("Operation response: ", MVOperationCodes.GetActiveAvatar, " returnCode: ", returnCode, " woid ", num3));
			OnGetActiveAvatarResponse(returnCode, num3);
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
				int silverAmount = (int)parameters[131];
				int goldAmount = (int)parameters[130];
				OnGetCreditStatus(silverAmount, goldAmount);
			}
			else
			{
				Debug.LogError("GetCreditStatus operation failed");
			}
			break;
		case MVOperationCodes.GetDBTimeTicks:
			if (returnCode == 0)
			{
				localTimeInMillisecondsOnDBTimeSync = LocalTimeInMilliSeconds;
				long ticks = (long)parameters[133];
				dbTimeBase = new DateTime(ticks);
				GotoNextJoinState();
			}
			else
			{
				Debug.LogError("GetDBTimeTicks operation failed");
			}
			break;
		case MVOperationCodes.AddItemToMarketPlace:
			if (returnCode == 0)
			{
				int num = (int)parameters[38];
				int num2 = (int)parameters[136];
				Debug.Log($"ItemID: {num} shopInventoryId {num2}");
				itemBusinessLogic.GetItem(num).shopInventoryID = num2;
				PlayerRepository.PlayerInventory[num].shopInventoryID = num2;
			}
			else
			{
				Debug.LogWarning("Failed to add item to marketPlace");
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
				Debug.LogError("SetAvatarAccessorySlot operation failed");
			}
			break;
		case MVOperationCodes.CreateGameSnapshot:
		{
			WorldNetwork worldNetwork = this.worldNetwork;
			worldNetwork.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(worldNetwork.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryDataHandler));
			CreatePlayersFromUserList((Dictionary<object, object>)parameters[13]);
			CreateTeamList((Dictionary<object, object>)parameters[90]);
			RequestGetNextGameBatch((int)parameters[99]);
			MVGameStateType gameStateType = (MVGameStateType)(int)parameters[63];
			int startTime = (int)parameters[65];
			int duration = (int)parameters[64];
			MVGameStateReason reason = (MVGameStateReason)(int)parameters[66];
			Debug.Log((int)parameters[157]);
			byte[] stats = (byte[])parameters[159];
			gameStatCounterManager.SetStats(stats);
			gameStatCounterManager.OnCounterTypeChanged += GameSessionCounterRules.OnCounterTypeChanged;
			this.worldNetwork.WorldInventory.FineGrainedTerrainPrototypeID = (int)parameters[157];
			networkGameStateListener.ChangeState(this, gameStateType, startTime, duration, reason, 0);
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
				Debug.LogWarning("Add item to world returned -1. This means that a singleton wo is already present and thus the request was rejected. We need a way to handle server operations in consistent manner.");
				if (MVGameController.GameSessionData.gameMode == MVGameMode.Edit)
				{
					EditController.EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
				}
			}
			break;
		case MVOperationCodes.InitializeAvatarEdit:
		{
			byte[] buffer = (byte[])parameters[165];
			avatarMetaDataWoMap = new MvAvatarMetaDataWoMap(new BytePacker(buffer));
			GotoNextJoinState();
			break;
		}
		case MVOperationCodes.AddAvatarToAvatarShopInventory:
			if (OnMarketPlaceActionComplete != null)
			{
				OnMarketPlaceActionComplete(returnCode == 0);
			}
			break;
		case MVOperationCodes.DeleteAvatarFromShopInventory:
			if (OnMarketPlaceActionComplete != null)
			{
				OnMarketPlaceActionComplete(returnCode == 0);
			}
			break;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		byte code = photonEvent.Code;
		switch (code)
		{
		case byte.MaxValue:
		{
			int profileID3 = (int)photonEvent[11];
			int num6 = (int)photonEvent[254];
			string userName = (string)photonEvent[9];
			string regionCode = (string)photonEvent[155];
			if (num6 == LocalPlayerActorNumber)
			{
				Debug.LogError("Received join event for localPlayerActorNumber");
				break;
			}
			MVPlayer player = new MVPlayer(num6, profileID3, userName, regionCode);
			AddPlayer(player);
			break;
		}
		case 254:
		{
			int num4 = (int)photonEvent[254];
			if (num4 != LocalPlayer.ActorNr)
			{
				MVPlayer mVPlayer = Players[num4];
				Debug.Log("Removed actor " + num4);
				Players.Remove(num4);
				gameStatCounterManager.RemoveStatsFromActor(num4);
				if (onPlayerListChanged != null)
				{
					onPlayerListChanged();
				}
				if (OnReceivedGameMsg != null)
				{
					Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
					dictionary2[(byte)0] = num4;
					dictionary2[(byte)3] = mVPlayer.Username;
					dictionary2[(byte)6] = MVGameController.Game.Friends.IsFriend(mVPlayer.ProfileID);
					OnReceivedGameMsg(MVGameMsgType.UserLeft, dictionary2);
				}
			}
			else
			{
				Debug.LogError("Local player leave event");
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
		case 11:
			break;
		case 3:
			worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataEvent((int)photonEvent[20], (Dictionary<object, object>)photonEvent[16]);
			break;
		case 4:
		{
			int worldObjectID9 = (int)photonEvent[20];
			Dictionary<object, object> worldObjectData = (Dictionary<object, object>)photonEvent[16];
			worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectDataPartialEvent(worldObjectID9, worldObjectData);
			break;
		}
		case 5:
		{
			int worldObjectID8 = (int)photonEvent[20];
			Dictionary<object, object> worldObjectDataToRemove = (Dictionary<object, object>)photonEvent[17];
			worldNetwork.WorldObjectClientManagerNetwork.OnRemoveWorldObjectDataPartialEvent(worldObjectID8, worldObjectDataToRemove);
			break;
		}
		case 31:
			if ((int)photonEvent[254] != LocalPlayer.ActorNr)
			{
				worldNetwork.WorldObjectClientManagerNetwork.OnUpdateWorldObjectRunTimeDataEvent((int)photonEvent[20], (Dictionary<object, object>)photonEvent[69]);
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
		case 38:
			OnAddObjectLinkEvent((int)photonEvent[55], (int)photonEvent[54], (int)photonEvent[56]);
			break;
		case 39:
			OnRemoveObjectLinkEvent((int)photonEvent[56]);
			break;
		case 15:
		{
			logger.Log("Add item to inventory event...");
			int actorNr4 = (int)photonEvent[254];
			int itemID = (int)photonEvent[38];
			int itemCategoryID = (int)photonEvent[151];
			int itemTypeID = (int)photonEvent[39];
			string itemName = (string)photonEvent[40];
			byte[] itemData = (byte[])photonEvent[41];
			int slotIndex = (int)photonEvent[43];
			int worldObjectID7 = (int)photonEvent[20];
			bool isResellable = (bool)photonEvent[139];
			int authorProfileId = (int)photonEvent[138];
			int originalItemID = (int)photonEvent[140];
			int priceGold = (int)photonEvent[68];
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
			Debug.Log("Ungroup event...");
			OnUngroupEvent(photonEvent);
			break;
		case 25:
			OnLockHierarchyEvent(photonEvent);
			break;
		case 27:
			OnWoUniquePrototypeEvent((int)photonEvent[20], (int)photonEvent[45]);
			break;
		case 28:
			OnGameStateChange((MVGameStateType)(int)photonEvent[63], (int)photonEvent[65], (int)photonEvent[64], (MVGameStateReason)(int)photonEvent[66], (int)photonEvent[254]);
			break;
		case 253:
		{
			int num5 = (int)photonEvent[253];
			Dictionary<object, object> dictionary4 = (Dictionary<object, object>)photonEvent[251];
			Debug.Log("ACTOR-NR: " + num5);
			Debug.Log(Players[num5].Username);
			{
				foreach (string key in dictionary4.Keys)
				{
					Debug.Log(key + ": " + dictionary4[key]);
				}
				break;
			}
		}
		case 29:
			OnSyncAvatarStatusEvent((int)photonEvent[254], (Dictionary<object, object>)photonEvent[67]);
			break;
		case 30:
			OnResetLogicChunkEvent((int)photonEvent[20]);
			break;
		case 32:
			OnPickupItemStateChangeEvent((PickupItemState)(int)photonEvent[70], (int)photonEvent[20], (int)photonEvent[254]);
			break;
		case 33:
			OnUpdateLineOfFire(camOrigin: new Vector3((float)photonEvent[73], (float)photonEvent[74], (float)photonEvent[75]), camDir: new Vector3((float)photonEvent[76], (float)photonEvent[77], (float)photonEvent[78]), worldObjectID: (int)photonEvent[20]);
			break;
		case 34:
			OnWorldObjectRPCEvent(photonEvent);
			break;
		case 36:
			OnPostGameMsg((MVGameMsgType)(int)photonEvent[87], (Dictionary<object, object>)photonEvent[88]);
			break;
		case 37:
			OnSetTeamEvent((int)photonEvent[254], (MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[89]));
			break;
		case 40:
			OnAddTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[89]));
			break;
		case 41:
			OnRemoveTeamEvent((MVTeam)(int)Enum.ToObject(typeof(MVTeam), (int)photonEvent[89]));
			break;
		case 42:
			OnTransferWorldObjectsToGroup(photonEvent);
			break;
		case 43:
			OnCloneWorldObjectTree(photonEvent);
			break;
		case 44:
			OnGetGameBatch(photonEvent);
			break;
		case 45:
			OnGameQueryReady(photonEvent);
			break;
		case 46:
			OnPostWinnerReportEvent();
			break;
		case 47:
			OnCollectiblePickedUp(photonEvent);
			break;
		case 48:
			OnSetWorldObjectsToPurchasedEvent((int)photonEvent[11], (int)photonEvent[38]);
			break;
		case 49:
			Debug.Log($"Profile with ID {(int)photonEvent[11]} unlocked Achievement {(AchievementType)(int)photonEvent[129]}");
			break;
		case 50:
			Debug.Log($"Received credit event update gold {(int)photonEvent[130]} silver {(int)photonEvent[131]}");
			OnCreditStatusEvent((int)photonEvent[131], (int)photonEvent[130]);
			break;
		case 51:
		{
			Dictionary<object, object> dictionary3 = (Dictionary<object, object>)photonEvent[71];
			int seatOwnerWoID = (int)dictionary3[(byte)4];
			int worldObjectID2 = (int)dictionary3[(byte)0];
			PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], seatOwnerWoID, worldObjectID2, (byte)photonEvent[142]);
			break;
		}
		case 52:
		{
			int num3 = (int)photonEvent[20];
			Debug.Log("WorldObjectID " + num3);
			MVWorldObjectClient worldObjectClient2 = WorldObjectClientManager.GetWorldObjectClient(num3);
			if (worldObjectClient2 != null && worldObjectClient2 is MVAvatar)
			{
				((MVAvatar)worldObjectClient2).OnLeaveVehicle();
			}
			break;
		}
		case 53:
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)photonEvent[71];
			int id = (int)dictionary[(byte)1];
			int worldObjectID = (int)dictionary[(byte)0];
			MVWorldObjectSpawnerVehicle mVWorldObjectSpawnerVehicle = (MVWorldObjectSpawnerVehicle)WorldObjectClientManager.GetWorldObjectClient(id);
			int spawnWorldObjectID = mVWorldObjectSpawnerVehicle.SpawnWorldObjectID;
			int num2 = (int)dictionary[(byte)3];
			int ownerActorNumber = (int)photonEvent[254];
			int cloneLinkId = (int)photonEvent[56];
			int cloneObjectLinkId = (int)photonEvent[92];
			int takeTime = (int)photonEvent[33];
			worldNetwork.OnCloneWorldObjectTreeEvent(ownerActorNumber, 0, cloneToRootGroup: true, spawnWorldObjectID, num2, cloneLinkId, cloneObjectLinkId);
			MVWorldObjectClient worldObjectClient = WorldObjectClientManager.GetWorldObjectClient(num2);
			MVWorldObjectClient.CallBackDelegate callBack = (MVWorldObjectClient wo) =>
			{
				wo.InteractionFlags = InteractionFlags.None;
			};
			worldObjectClient.TraverseRecursiveTail(callBack);
			PlayerController.OnAttachWorldObjectToSeat((int)photonEvent[254], num2, worldObjectID, (byte)photonEvent[142]);
			mVWorldObjectSpawnerVehicle.Take(takeTime);
			break;
		}
		case 54:
		{
			int num = (int)photonEvent[144];
			RewardReason rewardReason = (RewardReason)(byte)photonEvent[146];
			RewardType rewardType = (RewardType)(byte)photonEvent[145];
			Debug.Log($"Amount {num}, rewardReason {rewardReason}, rewardType {rewardType} ");
			BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
			break;
		}
		case 55:
		{
			byte[] buffer2 = (byte[])photonEvent[245];
			worldNetwork.RuntimeEventManagerNetwork.HandleRuntimeEvent(RuntimeEvent.Create(new BytePacker(buffer2)));
			break;
		}
		case 56:
			worldNetwork.RuntimeEventManagerNetwork.ResetTerrain();
			break;
		case 57:
		{
			int actorNumber = (int)photonEvent[254];
			MVTeam team = (MVTeam)(int)photonEvent[89];
			GameStatCounterType counterType = (GameStatCounterType)(byte)photonEvent[160];
			int value = (int)photonEvent[161];
			int otherID = (int)photonEvent[162];
			bool includeTeamScore = (bool)photonEvent[163];
			if ((bool)photonEvent[164])
			{
				gameStatCounterManager.Increment(counterType, team, actorNumber, value, otherID, includeTeamScore);
			}
			else
			{
				gameStatCounterManager.Update(counterType, actorNumber, team, value, otherID, includeTeamScore);
			}
			break;
		}
		case 58:
		{
			byte[] stat = (byte[])photonEvent[159];
			gameStatCounterManager.SetStat(stat);
			break;
		}
		case 59:
		{
			int woID = (int)photonEvent[20];
			byte[] buffer = (byte[])photonEvent[166];
			MvAvatarMetaData mvAvatarMetaData = new MvAvatarMetaData(new BytePacker(buffer));
			Debug.Log(mvAvatarMetaData);
			avatarMetaDataWoMap.Add(woID, mvAvatarMetaData);
			break;
		}
		case 60:
			OnLevelChanged((int)photonEvent[254], (int)photonEvent[170]);
			break;
		case 61:
			OnXPRewarded((int)photonEvent[254], (byte)photonEvent[85]);
			break;
		case 62:
		{
			int boostLeft = (int)photonEvent[180];
			bool boostEnabled = (bool)photonEvent[184];
			gameCoinManager.OnGameBoostChanged(boostLeft, boostEnabled);
			break;
		}
		default:
			Debug.LogError("Unknown event: " + (MVEventCodes)code);
			break;
		}
	}

	public void OnMessage(object messages)
	{
		Debug.LogError("Not implemented");
	}

	private void OnXPRewarded(int actorNr, byte xpId)
	{
		if (players.ContainsKey(actorNr))
		{
			if (OnReceivedXP != null)
			{
				OnReceivedXP(xpId, actorNr);
			}
			Debug.Log("Got xpId " + xpId);
		}
		else
		{
			Debug.LogError("Could not find player");
		}
	}

	private void OnLevelChanged(int actorNr, int level)
	{
		if (players.ContainsKey(actorNr))
		{
			players[actorNr].Level = level;
		}
		else
		{
			Debug.LogError("Could not find player");
		}
	}

	private void LoadModeGui()
	{
		switch (MVGameController.GameSessionData.gameMode)
		{
		case MVGameMode.Play:
			LoadPlayModeBaseGUI();
			break;
		case MVGameMode.Edit:
			MVGameController.LevelLoader.LoadLevel("EditModeGUI", LoadMode.Additive, LoadPlayModeBaseGUI);
			break;
		case MVGameMode.CharacterEditor:
			MVGameController.LevelLoader.LoadLevel("AvatarEditModeGUI", LoadMode.Additive, GotoNextJoinState);
			break;
		}
	}

	private void LoadPlayModeBaseGUI()
	{
		if (IsTouristSession)
		{
			MVGameController.LevelLoader.LoadLevel("PlayModeBaseGUI", LoadMode.Additive, LoadPlayModeTourist);
		}
		else
		{
			MVGameController.LevelLoader.LoadLevel("PlayModeBaseGUI", LoadMode.Additive, LoadPlayModeGUI);
		}
	}

	private void LoadPlayModeGUI()
	{
		MVGameController.LevelLoader.LoadLevel("PlayModeGUI", LoadMode.Additive, GotoNextJoinState);
	}

	private void LoadPlayModeTourist()
	{
		MVGameController.LevelLoader.LoadLevel("PlayModeTouristGUI", LoadMode.Additive, GotoNextJoinState);
	}

	public void OnStatusChanged(StatusCode returnCode)
	{
		Debug.Log("PeerStatusCallback():" + returnCode);
		switch (returnCode)
		{
		case StatusCode.Connect:
			JoinGame();
			break;
		case StatusCode.Disconnect:
			ConnState = MVConnState.DisconnectedByUser;
			Debug.LogWarning("Expecting that this disconnect is done by quiting");
			break;
		case StatusCode.SecurityExceptionOnConnect:
		case StatusCode.ExceptionOnConnect:
		case StatusCode.Exception:
		case StatusCode.SendError:
		case StatusCode.ExceptionOnReceive:
		case StatusCode.TimeoutDisconnect:
		case StatusCode.DisconnectByServerUserLimit:
		case StatusCode.DisconnectByServerLogic:
			Debug.Log("Disconnected bacause: " + returnCode);
			if (ConnState != MVConnState.DisconnectedByUser)
			{
				ConnState = MVConnState.Disconnected;
				MVGameController.ApplicationQuit(new QuitConnectionError());
			}
			break;
		default:
			Debug.LogWarning("Unhandled PeerStatusCallback, returnCode: " + returnCode);
			break;
		}
	}

	public void DebugReturn(DebugLevel level, string debug)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError("DebugReturn: " + debug);
			if (debug.Contains("Exiting receive thread (inside loop) due to error"))
			{
				Debug.LogError("Leave hack. Fix this!!");
				MVGameController.ApplicationQuit(new QuitConnectionError());
			}
		}
		Debug.Log(debug);
	}
}
