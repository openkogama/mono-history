using System;
using System.Runtime.InteropServices;
using System.Text;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public abstract class MVGameControllerBase : MonoBehaviour, IUpdatecontrollerSubscriber
{
	public delegate void OnPostGameInitDelegate();

	private static bool quitHasBeenCalled;

	private static int reAuthTestTries = 3;

	private static CustomBuildSettings customBuildSettings;

	private static TimeReward timeReward;

	private static string versionGuid;

	private static int versionStreamingAssets = -1;

	private static OverrideMaterials overrideMaterials;

	private static GameSessionData gameSessionData;

	private static MVJoinState _joinState;

	[SerializeField]
	private MVCameraController cameraController;

	[SerializeField]
	public PrefabFactory prefabFactory;

	[SerializeField]
	private MaterialLoader materialLoader;

	[SerializeField]
	private PrefabPool prefabPool;

	private static MVGameControllerBase instance;

	protected static bool isInitialized;

	protected static IPlayModeUI playModeUI;

	protected static IEditModeUI editModeUI;

	public static OnPostGameInitDelegate OnPostGameInit;

	public static bool LevelingTestMode;

	[SerializeField]
	private WaterPlaneManager waterPlanetManager;

	public static bool IsInitialized => isInitialized;

	public static IPlayModeUI IPlayModeUI => playModeUI;

	public static IEditModeUI IEditModeUI => editModeUI;

	public static PrefabFactory PrefabFactory => instance.prefabFactory;

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

	public static string VersionGuid => versionGuid;

	public static int VersionStreamingAssets => versionStreamingAssets;

	public static bool UsingDevSessionData => customBuildSettings.ShowLogin || Application.isEditor;

	public static MVNetworkGame Game { get; private set; }

	public static GameSessionData GameSessionData => gameSessionData;

	public static MVGameMode GameMode => gameSessionData.gameMode;

	public static MVWorldObjectClientManager WOCM => Game.WorldObjectClientManager;

	public static AudioManager AudioManager { get; private set; }

	public static BrowserComm BrowserComm { get; set; }

	public static TimeReward TimeReward => timeReward;

	public static GizmoDrawer GizmoDrawer { get; private set; }

	public static LevelLoader LevelLoader { get; private set; }

	public static VersionNumber VersionNumber { get; private set; }

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

	private void Awake()
	{
		instance = this;
		GizmoDrawer = GetComponent<GizmoDrawer>();
		LevelLoader = GetComponent<LevelLoader>();
		AudioManager = GetComponent<AudioManager>();
		BrowserComm = GetComponentInChildren<BrowserComm>();
		overrideMaterials = GetComponentInChildren<OverrideMaterials>();
		timeReward = new TimeReward();
		DebugLogHandler.Init();
		CheatHandling.Init();
		AudioEventHandler.Init();
		InitVersion();
		InitUpdateController();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		Application.runInBackground = true;
	}

	private void Start()
	{
		UnityEngine.Object.Instantiate(prefabPool);
		customBuildSettings = Resources.Load("Prefabs/CustomBuildSettings", typeof(CustomBuildSettings)) as CustomBuildSettings;
		bool developmentMode = Application.isEditor || customBuildSettings.ShowLogin;
		InitStandAlone(developmentMode);
	}

	private void Update()
	{
		UpdateController.Update();
	}

	private void FixedUpdate()
	{
		UpdateController.FixedUpdate();
	}

	private void LateUpdate()
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
		CleanUp();
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
		if (Game == null)
		{
			return;
		}
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
			Debug.LogError("Exception in FixedUpdate: " + ex.ToString());
		}
	}

	public static void SetGameSessionData(GameSessionData gameSessionData)
	{
		MVGameControllerBase.gameSessionData = gameSessionData;
	}

	private void StartGame()
	{
		StatHatWrapper.Count("MVGameControllerStartGame", 1);
		Game = new MVNetworkGame();
		if (!Game.Join())
		{
			Debug.LogError("Failed to connect");
		}
	}

	protected abstract void CreateInGameController();

	public static void InitializeInGameController()
	{
		instance.CreateInGameController();
	}

	public static bool TryReauth()
	{
		if (Game != null && OkToReAuth)
		{
			Game.Peer.Disconnect();
			AsyncWWWManager.WWWRequest(new GetRequest(gameSessionData.reauthURL, instance.OnReceivedWebParametersFromHttpRequest));
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
			applicationQuitObject?.OnQuit();
			Application.Quit();
		}
	}

	public static void RegisterOverrideMaterials()
	{
		if (Application.isEditor)
		{
			overrideMaterials.Register();
		}
	}

	private void InitVersion()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Version Number")) as GameObject;
		gameObject.transform.parent = transform;
		VersionNumber = gameObject.GetComponent<VersionNumber>();
		versionGuid = VersionNumber.versionGuid;
		versionStreamingAssets = VersionNumber.versionStreamingAssets;
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
			Debug.Log(gameSessionData.pingURL);
			Debug.Log(gameSessionData.disconnectURL);
			SetGameSessionData(gameSessionData);
			StartGame();
		}
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
	}

	protected virtual void InitStandAlone(bool developmentMode)
	{
		DeleteScreenPlayerPrefs();
		BrowserComm.enableBrowserRequest = !developmentMode;
		SetPosition(0, 0, Screen.width, Screen.height);
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
		AsyncWWWManager.WWWRequest(new GetRequest(text3, OnReceivedWebParametersFromHttpRequest));
	}

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
	{
		SetWindowPos(FindWindow(null, "KoGaMa"), 0, x, y, resX, resY, (resX * resY == 0) ? 1 : 0);
	}

	private void OnReceivedWebParametersFromHttpRequest(WWW www)
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
		}
		AudioEventHandler.Update();
	}

	private void FixedUpdateGame()
	{
		Game.FixedUpdate();
	}

	protected virtual void UpdateInternal()
	{
	}

	protected void Initialize()
	{
		isInitialized = true;
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
