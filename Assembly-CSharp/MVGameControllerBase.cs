using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AntiHack;
using Assets.Scripts.AdIntegration;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public abstract class MVGameControllerBase : MonoBehaviour, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	public delegate void OnReceivedGameMsgDelegate(MVGameMsgType type, Dictionary<object, object> gameMsgData);

	public delegate void OnReceivedNotificationEventDelegate(NotificationType type, Dictionary<object, object> data);

	public delegate void OnPostGameInitDelegate();

	protected class VersionData
	{
		public int minVersion { get; set; }

		public int version { get; set; }

		public bool ForceUpdate(int clientVersion)
		{
			if (clientVersion < minVersion)
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return $"version {version}. minVersion {minVersion}.";
		}
	}

	[SerializeField]
	protected KoGaMaSettingsContainer koGaMaSettings;

	[SerializeField]
	private MainCameraManager mainCameraManager;

	[SerializeField]
	private Styles styles;

	[SerializeField]
	private MaterialLoader materialLoader;

	[SerializeField]
	protected PrefabPool prefabPool;

	[SerializeField]
	protected TextureIntegrityChecker textureIntegrityChecker;

	[SerializeField]
	private ThemeRepository themeRepository;

	[SerializeField]
	private StreamingAssetManager streamingAssetManager;

	public const bool LevelingTestMode = false;

	public static OnReceivedGameMsgDelegate OnReceivedGameMsg;

	public static OnReceivedNotificationEventDelegate OnReceivedNotification;

	public static OnPostGameInitDelegate OnPostGameInit;

	protected static MVGameControllerBase instance;

	protected MVNetworkGame game;

	private AudioManager audioManager;

	private BrowserComm browserComm;

	private LevelLoader levelLoader;

	private SkinnedMeshOptimizeManager skinnedMeshOptimizeManager = new SkinnedMeshOptimizeManager();

	private FlagDebriefingControl flagDebriefingControl = new FlagDebriefingControl();

	private GoldRewardManager goldRewardManager = new GoldRewardManager();

	private bool quitHasBeenCalled;

	private TimeReward timeReward;

	private OverrideMaterials overrideMaterials;

	private LoadStats loadStats;

	private MVJoinState _joinState;

	private FirstFrameUpdateActorReady firstFrameUpdateActorReady;

	private int reAuthTestTries = 3;

	private Action<MVJoinState> onJoinStateChanged;

	[SerializeField]
	private AudioBuild audioBuild;

	[SerializeField]
	private WaterPlaneManager waterPlaneManagerPrefab;

	private WaterPlaneManager waterPlaneManager;

	[SerializeField]
	private SkyboxManager skyboxManager;

	private bool reportedError;

	private bool reportedOngoingError;

	public static SpawnRoleDataMediator SpawnRoleDataMediatorLocal => Game.LocalPlayer.SpawnRoleDataMediator;

	public static MVLocalPlayer LocalPlayer => Game.LocalPlayer;

	public static GameEventManager GameEventManager => Game.GameEventManager;

	public static TextureIntegrityChecker TextureIntegrityChecker => instance.textureIntegrityChecker;

	public static StreamingAssetManager StreamingAssetManager => instance.streamingAssetManager;

	public static bool IsInitialized { get; protected set; }

	public static bool DisconnectIsOk { get; private set; }

	public static IPlayModeUI PlayModeUI { get; protected set; }

	public static IEditModeUI EditModeUI { get; protected set; }

	public static bool IsAlive => instance != null;

	public static MVNetworkGame Game => instance.game;

	public static AudioManager AudioManager => instance.audioManager;

	public static BrowserComm BrowserComm => instance.browserComm;

	public static LevelLoader LevelLoader => instance.levelLoader;

	public static SkinnedMeshOptimizeManager SkinnedMeshOptimizeManager => instance.skinnedMeshOptimizeManager;

	public static FlagDebriefingControl FlagDebriefingControl => instance.flagDebriefingControl;

	public static GoldRewardManager GoldRewardManager => instance.goldRewardManager;

	public static GameSessionData GameSessionData { get; private set; }

	public static BuildTarget BuildTarget => BuildTarget.StandAlone;

	public static Action OnFirstFrameUpdateActorReady
	{
		get
		{
			return instance.firstFrameUpdateActorReady.callbacks;
		}
		set
		{
			instance.firstFrameUpdateActorReady.callbacks = value;
		}
	}

	private static bool OkToReAuth
	{
		get
		{
			instance.reAuthTestTries--;
			return instance.reAuthTestTries >= 0;
		}
	}

	public static int ReAuthTries => instance.reAuthTestTries;

	public static bool UsingDevSessionData => instance.koGaMaSettings.ShowDebugLogin || Application.isEditor;

	public static MVNetworkGame.OperationRequests OperationRequests => Game.OperationRequestSender;

	public static LoadStats LoadStats => instance.loadStats;

	public static MVGameMode GameMode => GameSessionData.gameMode;

	public static MVWorldObjectClientManager WOCM => Game.WorldObjectClientManager;

	public static TimeReward TimeReward => instance.timeReward;

	public static KoGaMaSettingsContainer KoGaMaSettings => instance.koGaMaSettings;

	public static bool IsTouristSession => GameSessionData.profileID <= 0;

	public static IAdManager AdManager => instance.GetAdManager;

	protected abstract IAdManager GetAdManager { get; }

	public static bool SeekAdConsent { get; set; }

	public static MVJoinState JoinState
	{
		get
		{
			return instance._joinState;
		}
		set
		{
			instance._joinState = value;
			if (instance.onJoinStateChanged != null)
			{
				instance.onJoinStateChanged(value);
			}
		}
	}

	public static Action<MVJoinState> OnJoinStateChanged
	{
		get
		{
			return instance.onJoinStateChanged;
		}
		set
		{
			instance.onJoinStateChanged = value;
			if (instance.onJoinStateChanged != null)
			{
				instance.onJoinStateChanged(instance._joinState);
			}
		}
	}

	protected abstract bool IsPlayingInternal { get; }

	public static bool IsPlaying => instance.IsPlayingInternal;

	public static MaterialLoader MaterialLoader
	{
		get
		{
			if (instance.materialLoader == null)
			{
				throw new NullReferenceException();
			}
			return instance.materialLoader;
		}
	}

	public static MainCameraManager MainCameraManager
	{
		get
		{
			if (instance.mainCameraManager == null)
			{
				throw new NullReferenceException();
			}
			return instance.mainCameraManager;
		}
	}

	public static WaterPlaneManager WaterPlaneManager => instance.waterPlaneManager;

	public static SkyboxManager SkyboxManager => instance.skyboxManager;

	public static bool Quitting { get; private set; }

	protected virtual void Awake()
	{
		instance = this;
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.Append("Build info\n");
		stringBuilder.AppendFormat("Version Number: {0}\n", KoGaMaSettings.VersionString);
		stringBuilder.AppendFormat("Release Name: {0}\n", KoGaMaSettings.ReleaseName);
		stringBuilder.AppendFormat("Branch: {0}\n", KoGaMaSettings.BranchName);
		stringBuilder.AppendFormat("Latest commit message: {0}\n", koGaMaSettings.LatestCommitMessage);
		stringBuilder.AppendFormat("Build time: {0}\n", koGaMaSettings.BuildTime);
		Debug.Log(stringBuilder);
		DebugLogHandler.Init();
		if (!DebugLogHandler.IsSampling && !Debug.isDebugBuild)
		{
			Debug.unityLogger.filterLogType = LogType.Warning;
		}
		PlayerPrefsManager.EarlyInitialize();
		Debug.Log("Is first time session " + PlayerPrefsManager.IsFirstTimeSession);
		styles = UnityEngine.Object.Instantiate(styles);
		styles.transform.parent = transform;
		loadStats = new LoadStats();
		loadStats.GameStartTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		UnityEngine.Object.Instantiate(prefabPool);
		levelLoader = GetComponent<LevelLoader>();
		audioManager = GetComponent<AudioManager>();
		browserComm = GetComponentInChildren<BrowserComm>();
		overrideMaterials = GetComponentInChildren<OverrideMaterials>();
		timeReward = new TimeReward();
		CheatHandling.Init();
		AudioEventHandler.Init(audioBuild);
		textureIntegrityChecker.Initialize();
		themeRepository.Initialize();
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		MeshDataPool.Create();
		waterPlaneManager = UnityEngine.Object.Instantiate(instance.waterPlaneManagerPrefab);
		Application.runInBackground = true;
	}

	protected virtual void OnDestroy()
	{
		try
		{
			DrawPlane.Reset();
			BadgeManager.Reset();
			MeshDataPool.Destroy();
			LevelingManager.Destroy();
			StreamedSharedMaterialHandler.Reset();
			LoggerManager.Destroy();
			CullingApiWrapper.Destroy();
			HighlightManager.Reset();
			FirstTimeEventManager.Destroy();
			ThemeRepository.Destroy();
			AudioEventHandler.Destroy();
			DebugLogHandler.Reset();
			AwayMonitor.Destroy();
			TM.Destroy();
			AsyncWWWManager.Reset();
			UpdateController.Clear();
			StreamingAsset.ClearCache();
			AccessoryDataManager.Reset();
			DataUploadManager.Reset();
			TimedPlayReward.RewardTracker.Reset();
			UpdateController.RemoveUpdateObject(this);
		}
		catch (Exception ex)
		{
			Debug.LogError("MVGameControllerBase.OnDestroy exception: " + ex.Message);
		}
		finally
		{
			OnPostGameInit = null;
			GameSessionData = null;
			IsInitialized = false;
			instance = null;
		}
	}

	protected void Update()
	{
		UpdateController.Update();
		HandleDebugShortCuts();
		HandleStatHatErrorCount();
	}

	protected void FixedUpdate()
	{
		UpdateController.FixedUpdate();
	}

	protected virtual void LateUpdate()
	{
		UpdateController.LateUpdate();
		if (IsInitialized)
		{
			Game.World.WorldInventory.LateUpdate();
			MainCameraManager.UpdateCamera();
		}
	}

	protected void OnDrawGizmos()
	{
		CullingApiWrapper.DebugVisualize();
	}

	public static void PostGameMsg(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
	{
		if (OnReceivedGameMsg != null)
		{
			OnReceivedGameMsg(gameMsgType, gameMsgData);
		}
	}

	public static void PostGameMsg(MVGameMsgType gameMsgType, string message)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)5, message);
		PostGameMsg(gameMsgType, dictionary);
	}

	public static void PostDestroyCleanup()
	{
		if (OnReceivedNotification != null)
		{
			Debug.LogWarning("OnReceivedNotification still have subscribers");
			OnReceivedNotification = null;
		}
		if (OnReceivedGameMsg != null)
		{
			Debug.LogWarning("OnReceivedGameMsg still have subscribers");
			OnReceivedGameMsg = null;
		}
	}

	public void UpdateControllerUpdate()
	{
		AsyncWWWManager.Update();
		if (Game == null)
		{
			return;
		}
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
			Debug.LogError("Exception in Update: " + ex.ToString());
		}
	}

	public static void SetGameSessionData(GameSessionData gameSessionData)
	{
		GameSessionData = gameSessionData;
		PlayerPrefsManager.Initialize(gameSessionData);
		AwayMonitor.Initialize(gameSessionData.gameMode);
	}

	public static bool TryReauth()
	{
		if (Game != null && OkToReAuth)
		{
			DisconnectIsOk = true;
			Game.Peer.Disconnect();
			AsyncWWWManager.WWWRequest(new GetRequest(GameSessionData.reauthURL, instance.OnReceivedReAuthWebParametersFromHttpRequest, WWWRequestPriority.ExecuteIgnoreAllConstraints));
			return true;
		}
		return false;
	}

	public static void ApplicationQuit(QuitBaseCallback applicationQuitObject)
	{
		Debug.Log("Application quit");
		if (!instance.quitHasBeenCalled)
		{
			instance.quitHasBeenCalled = true;
			AsyncWWWManager.ShutDown(() =>
			{
				instance.HandleApplicationQuit(applicationQuitObject);
			});
		}
	}

	public static void RegisterOverrideMaterials()
	{
		if (Application.isEditor)
		{
			instance.overrideMaterials.Register();
		}
	}

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
	{
		SetWindowPos(FindWindow(null, "KoGaMa"), 0, x, y, resX, resY, (resX * resY == 0) ? 1 : 0);
	}

	protected void OnApplicationQuit()
	{
		Quitting = true;
		ShutDown();
	}

	protected void ShutDown()
	{
		HandleQuitDisconnect();
		CleanUp();
	}

	protected abstract void HandleApplicationQuit(QuitBaseCallback quitBaseCallback);

	protected virtual void CleanUp()
	{
		Game.Cleanup();
		if (PlayModeUI != null)
		{
			((MonoBehaviour)PlayModeUI).gameObject.SetActive(value: false);
		}
		MainCameraManager.gameObject.SetActive(value: false);
		gameObject.SetActive(value: false);
		GameLoader.UnloadGame();
	}

	protected void HandleQuitDisconnect()
	{
		DisconnectIsOk = true;
		if (Game != null)
		{
			if (GameSessionData != null)
			{
				SessionLocatorPing.LeaveSession();
			}
			if (Game.Peer != null && Game.ConnState == MVConnState.Joined)
			{
				Game.Peer.Disconnect();
			}
		}
	}

	protected virtual void StartGame()
	{
		DisconnectIsOk = false;
		StatHatWrapper.Count("MVGameControllerStartGame", 1);
		game = new MVNetworkGame();
		firstFrameUpdateActorReady = new FirstFrameUpdateActorReady();
		if (!Game.Join())
		{
			Debug.LogError("Failed to connect");
		}
	}

	protected virtual void InitWebGL(bool developmentMode)
	{
		BrowserComm.enableExternalCall = !developmentMode;
		if (developmentMode)
		{
			StartGame();
			return;
		}
		BrowserComm.ToJavaScript.GetBrowserVersion();
		BrowserComm.ToJavaScript.ExternalCall("sendPlayerParams", StartGameWithSessionData);
		BrowserComm.ToJavaScript.ExternalCall("sendLoadStats", ReceivedLoadStatsCallback);
	}

	protected virtual void InitStandAlone(bool developmentMode)
	{
		DeleteScreenPlayerPrefs();
		BrowserComm.enableBrowserRequest = !developmentMode;
		SetPosition(0, 0, Screen.width, Screen.height);
		Application.targetFrameRate = 60;
		if (developmentMode)
		{
			StartGame();
			return;
		}
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		string text = string.Empty;
		string[] array = commandLineArgs;
		foreach (string text2 in array)
		{
			string[] array2 = text2.Split(new string[1] { "kogamaPackage:" }, StringSplitOptions.None);
			if (array2.Length == 2)
			{
				text += array2[1];
			}
		}
		Debug.Log("combined " + text);
		byte[] bytes = Convert.FromBase64String(text);
		string text3 = Encoding.UTF8.GetString(bytes);
		Debug.Log(text3);
		AsyncWWWManager.WWWRequest(new GetRequest(text3, OnReceivedSessionData, WWWRequestPriority.ExecuteIgnoreAllConstraints));
	}

	protected void OnReceivedReAuthWebParametersFromHttpRequest(UnityWebRequest www)
	{
		string text = www.downloadHandler.text;
		Debug.Log("Reauth webParameters " + text);
		StartGameWithSessionData(ok: true, text);
	}

	protected void OnReceivedSessionData(UnityWebRequest www)
	{
		string text = www.downloadHandler.text;
		Debug.Log(text);
		StartGameWithSessionData(ok: true, text);
	}

	protected virtual void UpdateInternal()
	{
	}

	protected void Initialize()
	{
		IsInitialized = true;
		materialLoader.Initialize();
	}

	private static void HandleStatHatErrorCount()
	{
		if (!instance.reportedError && DebugLogHandler.ErrorDetected)
		{
			StatHatWrapper.Count("errorcount", 1);
			instance.reportedError = true;
		}
		if (!instance.reportedOngoingError && DebugLogHandler.OngoingErrorDetected)
		{
			StatHatWrapper.Count("errorcountongoing", 1);
			instance.reportedOngoingError = true;
		}
	}

	private void HandleDebugShortCuts()
	{
		if (Input.GetKey(KeyCode.Alpha7) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			if (Debug.unityLogger.filterLogType == LogType.Warning)
			{
				Debug.unityLogger.filterLogType = LogType.Log;
				Debug.Log("Enabling logging!");
			}
			else if (Debug.unityLogger.filterLogType == LogType.Log)
			{
				Debug.Log("Disabling logging!");
				Debug.unityLogger.filterLogType = LogType.Warning;
			}
		}
	}

	private void StartGameWithSessionData(bool ok, string sessionDataJson)
	{
		if (ok)
		{
			Debug.Log("WEBPARAMS: " + sessionDataJson);
			GameSessionData gameSessionData = JsonConvert.DeserializeObject<GameSessionData>(sessionDataJson);
			StatHatWrapper.Initialize(PlayerPrefsManager.IsFirstTimeSession);
			Debug.Log(gameSessionData.pingURL);
			Debug.Log(gameSessionData.disconnectURL);
			SetGameSessionData(gameSessionData);
			StartGame();
		}
	}

	private void ReceivedLoadStatsCallback(bool ok, string data)
	{
		if (!ok)
		{
			Debug.LogWarning("Failed to get load stats data");
			return;
		}
		LoadStats loadStats = JsonConvert.DeserializeObject<LoadStats>(data);
		this.loadStats.DOMReady = loadStats.DOMReady;
		this.loadStats.PluginInit = loadStats.PluginInit;
	}

	private void UpdateGame()
	{
		Game.Update();
		if (JoinState == MVJoinState.Playing)
		{
			UpdateInternal();
			AwayMonitor.Update();
		}
		AudioEventHandler.Update();
	}

	protected static void DeleteScreenPlayerPrefs()
	{
		PlayerPrefs.DeleteKey("Screenmanager Is Fullscreen mode");
		PlayerPrefs.DeleteKey("Screenmanager Resolution Height");
		PlayerPrefs.DeleteKey("Screenmanager Resolution Width");
	}

	public void UpdateControllerLateUpdate()
	{
		throw new NotImplementedException();
	}
}
