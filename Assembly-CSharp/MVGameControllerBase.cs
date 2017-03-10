using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public abstract class MVGameControllerBase : MonoBehaviour, IUpdatecontrollerSubscriber
{
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

	public delegate void OnReceivedGameMsgDelegate(MVGameMsgType type, Dictionary<object, object> gameMsgData);

	public delegate void OnReceivedNotificationEventDelegate(NotificationType type, Dictionary<object, object> data);

	public delegate void OnPostGameInitDelegate();

	private static bool disconnectIsOk;

	private static bool quitHasBeenCalled;

	private static int reAuthTestTries = 3;

	private static TimeReward timeReward;

	private static OverrideMaterials overrideMaterials;

	private static GameSessionData gameSessionData;

	private static LoadStats loadStats;

	private static MVJoinState _joinState;

	[SerializeField]
	protected KoGaMaSettingsContainer koGaMaSettings;

	[SerializeField]
	private MVCameraController cameraController;

	[SerializeField]
	private Styles styles;

	[SerializeField]
	private MaterialLoader materialLoader;

	[SerializeField]
	protected PrefabPool prefabPool;

	protected static MVGameControllerBase instance;

	protected static bool isInitialized;

	protected static IPlayModeUI playModeUI;

	protected static IEditModeUI editModeUI;

	public static OnReceivedGameMsgDelegate OnReceivedGameMsg;

	public static OnReceivedNotificationEventDelegate OnReceivedNotification;

	public static OnPostGameInitDelegate OnPostGameInit;

	public static bool LevelingTestMode;

	[SerializeField]
	private WaterPlaneManager waterPlanetManager;

	public static bool IsInitialized => isInitialized;

	public static bool DisconnectIsOk => disconnectIsOk;

	public static IPlayModeUI IPlayModeUI => playModeUI;

	public static IEditModeUI IEditModeUI => editModeUI;

	public static BuildTarget BuildTarget => GetBuildTarget();

	private static bool OkToReAuth
	{
		get
		{
			reAuthTestTries--;
			if (reAuthTestTries >= 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool UsingDevSessionData => instance.koGaMaSettings.ShowDebugLogin || Application.isEditor;

	public static MVNetworkGame Game { get; private set; }

	public static MVNetworkGame.OperationRequests OperationRequests => Game.OperationRequestSender;

	public static GameSessionData GameSessionData => gameSessionData;

	public static LoadStats LoadStats => loadStats;

	public static MVGameMode GameMode => gameSessionData.gameMode;

	public static MVWorldObjectClientManager WOCM => Game.WorldObjectClientManager;

	public static AudioManager AudioManager { get; private set; }

	public static BrowserComm BrowserComm { get; set; }

	public static TimeReward TimeReward => timeReward;

	public static LevelLoader LevelLoader { get; private set; }

	public static KoGaMaSettingsContainer KoGaMaSettings => instance.koGaMaSettings;

	public static bool IsTouristSession => GameSessionData.profileID <= 0;

	public static MVJoinState JoinState
	{
		get
		{
			return _joinState;
		}
		set
		{
			_joinState = value;
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

	public static MVCameraController CameraController
	{
		get
		{
			if (instance.cameraController == null)
			{
				throw new NullReferenceException();
			}
			return instance.cameraController;
		}
	}

	public static WaterPlaneManager WaterPlaneManager
	{
		get
		{
			if (instance.cameraController == null)
			{
				throw new NullReferenceException();
			}
			return instance.waterPlanetManager;
		}
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

	private void Awake()
	{
		instance = this;
		Debug.Log(KoGaMaSettings.VersionString);
		Debug.Log("Branch " + KoGaMaSettings.BranchName);
		Debug.Log("Latest commit message " + koGaMaSettings.LatestCommitMessage);
		Debug.Log("Build time " + koGaMaSettings.BuildTime);
		DebugLogHandler.Init();
		if (!DebugLogHandler.IsSampling && !Debug.isDebugBuild)
		{
			Debug.logger.filterLogType = LogType.Warning;
		}
		styles = UnityEngine.Object.Instantiate(styles);
		styles.transform.parent = transform;
		loadStats = new LoadStats();
		loadStats.GameStartTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		UnityEngine.Object.Instantiate(prefabPool);
		LevelLoader = GetComponent<LevelLoader>();
		AudioManager = GetComponent<AudioManager>();
		BrowserComm = GetComponentInChildren<BrowserComm>();
		overrideMaterials = GetComponentInChildren<OverrideMaterials>();
		timeReward = new TimeReward();
		CheatHandling.Init();
		AudioEventHandler.Init();
		InitUpdateController();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		Application.runInBackground = true;
	}

	private void Update()
	{
		UpdateController.Update();
		HandleDebugShortCuts();
	}

	private void OnDrawGizmos()
	{
		CullingApiWrapper.DebugVisualize();
	}

	private void HandleDebugShortCuts()
	{
		if (Input.GetKey(KeyCode.Alpha7) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			if (Debug.logger.filterLogType == LogType.Warning)
			{
				Debug.logger.filterLogType = LogType.Log;
				Debug.Log("Enabling logging!");
			}
			else if (Debug.logger.filterLogType == LogType.Log)
			{
				Debug.Log("Disabling logging!");
				Debug.logger.filterLogType = LogType.Warning;
			}
		}
	}

	private void FixedUpdate()
	{
		UpdateController.FixedUpdate();
	}

	protected virtual void LateUpdate()
	{
		if (isInitialized)
		{
			Game.World.WorldInventory.LateUpdate();
			CameraController.UpdateCamera();
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log("On application quit");
		HandleQuitDisconnect();
		CleanUp();
		CullingApiWrapper.Destroy();
	}

	protected void HandleQuitDisconnect()
	{
		disconnectIsOk = true;
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

	public void UpdateControllerFixedUpdate()
	{
	}

	public static void SetGameSessionData(GameSessionData gameSessionData)
	{
		MVGameControllerBase.gameSessionData = gameSessionData;
	}

	protected void StartGame()
	{
		disconnectIsOk = false;
		StatHatWrapper.Count("MVGameControllerStartGame", 1);
		Game = new MVNetworkGame();
		if (!Game.Join())
		{
			Debug.LogError("Failed to connect");
		}
	}

	public static bool TryReauth()
	{
		if (Game != null && OkToReAuth)
		{
			disconnectIsOk = true;
			Game.Peer.Disconnect();
			AsyncWWWManager.WWWRequest(new GetRequest(gameSessionData.reauthURL, instance.OnReceivedReAuthWebParametersFromHttpRequest, WWWRequestPriority.ExecuteIgnoreAllConstraints));
			return true;
		}
		return false;
	}

	public static void ApplicationQuit(QuitBaseCallback applicationQuitObject)
	{
		Debug.Log("Application quit");
		if (!quitHasBeenCalled)
		{
			quitHasBeenCalled = true;
			instance.HandleApplicationQuit(applicationQuitObject);
		}
	}

	public abstract void HandleApplicationQuit(QuitBaseCallback quitBaseCallback);

	public static void RegisterOverrideMaterials()
	{
		if (Application.isEditor)
		{
			overrideMaterials.Register();
		}
	}

	private static BuildTarget GetBuildTarget()
	{
		return BuildTarget.StandAlone;
	}

	private void InitUpdateController()
	{
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	private void ReceivedWebParamsCallback(bool ok, string data)
	{
		if (ok)
		{
			Debug.Log("WEBPARAMS: " + data);
			GameSessionData gameSessionData = JsonConvert.DeserializeObject<GameSessionData>(data);
			if (gameSessionData.detailedStats)
			{
				StatHatWrapper.DoDetailedStatsForSession();
			}
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
		MVGameControllerBase.loadStats.DOMReady = loadStats.DOMReady;
		MVGameControllerBase.loadStats.PluginInit = loadStats.PluginInit;
	}

	protected virtual void InitWebPlayer(bool developmentMode)
	{
		BrowserComm.enableExternalCall = !developmentMode;
		if (developmentMode)
		{
			StartGame();
			return;
		}
		BrowserComm.ToJavaScript.GetBrowserVersion();
		BrowserComm.ToJavaScript.ExternalCall("sendPlayerParams", ReceivedWebParamsCallback);
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
		AsyncWWWManager.WWWRequest(new GetRequest(text3, OnReceivedWebParametersFromHttpRequest, WWWRequestPriority.ExecuteIgnoreAllConstraints));
	}

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
	{
		SetWindowPos(FindWindow(null, "KoGaMa"), 0, x, y, resX, resY, (resX * resY == 0) ? 1 : 0);
	}

	protected void OnReceivedReAuthWebParametersFromHttpRequest(WWW www)
	{
		string text = www.text;
		Debug.Log("Reauth webParameters " + text);
		ReceivedWebParamsCallback(ok: true, text);
	}

	protected void OnReceivedWebParametersFromHttpRequest(WWW www)
	{
		string text = www.text;
		Debug.Log(text);
		ReceivedWebParamsCallback(ok: true, text);
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

	protected virtual void UpdateInternal()
	{
	}

	protected void Initialize()
	{
		isInitialized = true;
		materialLoader.Initialize();
	}

	protected virtual void CleanUp()
	{
		try
		{
			Game.Cleanup();
			Game = null;
			UpdateController.Clear();
			UnityEngine.Object.Destroy(gameObject);
			DeleteScreenPlayerPrefs();
			GC.Collect();
		}
		catch (Exception ex)
		{
			Debug.Log("From clean up " + ex.Message);
		}
	}

	private static void DeleteScreenPlayerPrefs()
	{
		PlayerPrefs.DeleteKey("Screenmanager Is Fullscreen mode");
		PlayerPrefs.DeleteKey("Screenmanager Resolution Height");
		PlayerPrefs.DeleteKey("Screenmanager Resolution Width");
	}
}
