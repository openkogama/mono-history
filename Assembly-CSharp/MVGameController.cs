using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGameController : MonoBehaviour, IInputHandler, IUpdatecontrollerSubscriber
{
	public enum LoadMode
	{
		Overwrite,
		Additive
	}

	public delegate void OnPostGameInitDelegate();

	public OnPostGameInitDelegate OnPostGameInit;

	private static bool catchUpdateLoopExceptions;

	private static MVGameController _instance;

	private GameSessionData gameSessionData;

	private HotKeys hotkeys;

	public MVGUILoginHandler LoginForm;

	private TimeReward timeReward;

	public UpdateController UpdateController { get; private set; }

	public static MVGameController Instance
	{
		get
		{
			if ((Object)(object)_instance == (Object)null)
			{
				_instance = Object.FindObjectOfType(typeof(MVGameController)) as MVGameController;
			}
			if ((Object)(object)_instance == (Object)null)
			{
				Debug.LogWarning((object)"No instance of MVGameController found. Is the game shutting down?");
			}
			return _instance;
		}
	}

	public MVNetworkGame Game { get; private set; }

	public MVWorldObjectClientManager WOCM
	{
		get
		{
			if (Game == null)
			{
				return null;
			}
			return Game.WorldObjectClientManager;
		}
	}

	public AIngameController IngameController { get; private set; }

	public IAudioManager AudioManager { get; private set; }

	public BrowserComm BrowserComm { get; set; }

	public TimeReward TimeReward => timeReward;

	public int Priority => InputHandlerPriority.GAME;

	public bool GameJoined
	{
		get
		{
			if (Game == null)
			{
				return false;
			}
			return Game.JoinState == MVJoinState.Playing;
		}
	}

	public MVGameMode GameMode
	{
		get
		{
			if (gameSessionData == null)
			{
				Debug.LogError((object)"gammeSessionData not set yet!");
				return MVGameMode.Play;
			}
			return gameSessionData.GameMode;
		}
	}

	public int PlanetID
	{
		get
		{
			if (gameSessionData == null)
			{
				Debug.LogError((object)"gammeSessionData not set yet!");
				return -1;
			}
			return gameSessionData.PlanetID;
		}
	}

	public int ProfileID
	{
		get
		{
			if (gameSessionData == null)
			{
				Debug.LogError((object)"gammeSessionData not set yet!");
				return -1;
			}
			return gameSessionData.ProfileID;
		}
	}

	public bool IsTouristSession
	{
		get
		{
			if (gameSessionData == null)
			{
				Debug.LogError((object)"gammeSessionData not set yet!");
				return true;
			}
			return gameSessionData.ProfileID <= 0;
		}
	}

	public GameSessionData GameSessionData => gameSessionData;

	public AEditController EditController
	{
		get
		{
			if (IngameController != null && IngameController is AEditController)
			{
				return IngameController as AEditController;
			}
			return null;
		}
	}

	public CharacterEditorController CharacterEditorController
	{
		get
		{
			if (IngameController != null && GameMode == MVGameMode.CharacterEditor)
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
			if (IngameController != null && GameMode == MVGameMode.Edit)
			{
				return IngameController as EditorController;
			}
			return null;
		}
	}

	public PlayController PlayController
	{
		get
		{
			if (IngameController != null && GameMode == MVGameMode.Play)
			{
				return IngameController as PlayController;
			}
			return null;
		}
	}

	public static string GetIPFromDevServerTarget(DevServerTarget devTarget)
	{
		return devTarget switch
		{
			DevServerTarget.Dev => "95.211.176.21:5055", 
			DevServerTarget.Test => "54.228.103.157:5055", 
			DevServerTarget.RC => "95.211.176.46:5055", 
			DevServerTarget.Vault => "95.211.176.62:5055", 
			DevServerTarget.NewTest => "54.228.103.157:5055", 
			DevServerTarget.Local => "127.0.0.1:5055", 
			_ => string.Empty, 
		};
	}

	private void OnApplicationQuit()
	{
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Unregister(_instance);
		_instance = null;
	}

	private void Awake()
	{
		DebugLogHandler.Init();
		BrowserComm = UXUtils.FindObjectOfType<BrowserComm>();
		AudioEventHandler.Awake();
		timeReward = new TimeReward();
	}

	private T FindOrLogError<T>(string errorMsg) where T : MonoBehaviour
	{
		Object val = Object.FindObjectOfType(typeof(T));
		if (val == (Object)null)
		{
			Debug.LogError((object)errorMsg);
			return (T)(object)null;
		}
		return (T)(object)((val is T) ? val : null);
	}

	private void ReceivedWebParamsCallback(Dictionary<string, object> gameSessionData)
	{
		Debug.Log((object)("WEBPARAMS: " + gameSessionData));
		StartGame(new GameSessionData(gameSessionData));
	}

	public void StartGame(GameSessionData gameSessionData)
	{
		this.gameSessionData = gameSessionData;
		Localization.Instance.CultureName = gameSessionData.Language.Replace('_', '-');
		LoginForm.View.Hide();
		Game = new MVNetworkGame();
		Game.Join();
	}

	private void Start()
	{
		UpdateController = new UpdateController();
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
		AudioManager = FindOrLogError<AudioManager>("AudioManager must be present in scene!");
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		Application.runInBackground = true;
		Localization.Instance.CultureName = "en-US";
		CustomBuildSettings customBuildSettings = Resources.Load("Prefabs/CustomBuildSettings", typeof(CustomBuildSettings)) as CustomBuildSettings;
		bool showLogin = customBuildSettings.ShowLogin;
		if (Application.isEditor || showLogin)
		{
			LoginForm.View.Show();
			((Component)this).gameObject.AddComponent<DebugConsole>();
		}
		else
		{
			BrowserComm.ToWeb.ExternalCall("sendPlayerParams", ReceivedWebParamsCallback);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		Debug.Log((object)("OnLevelWasLoaded: " + level + " Application.loadedLevelName: " + Application.loadedLevelName));
		if (!(Application.loadedLevelName == "UnloadPlanet"))
		{
			return;
		}
		Debug.Log((object)"Scene loaded");
		if (!Application.isWebPlayer)
		{
			if ((Object)(object)LoginForm != (Object)null)
			{
				LoginForm.View.Show();
			}
			else
			{
				Debug.LogError((object)"loginForm was null!");
			}
		}
		else
		{
			if ((Object)(object)LoginForm != (Object)null)
			{
				LoginForm.View.Hide();
			}
			else
			{
				Debug.LogError((object)"loginForm was null!");
			}
			UXScreen uXScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
			if ((Object)(object)uXScreen != (Object)null)
			{
				if (uXScreen.Fullscreen)
				{
					uXScreen.Fullscreen = false;
				}
			}
			else
			{
				Debug.LogError((object)"UXScreen was null!");
			}
		}
		Game = null;
		GC.Collect();
	}

	public void LoadLevel(string levelName, LoadMode loadMode, Action callback)
	{
		Debug.Log((object)("Load level " + Time.frameCount));
		((MonoBehaviour)this).StartCoroutine(WaitForLoadToStart(levelName));
		Debug.Log((object)("WaitForLoadToStart " + Time.frameCount));
		((MonoBehaviour)this).StartCoroutine(WaitForLevel(levelName, loadMode, callback));
		Debug.Log((object)("WaitForLevel " + Time.frameCount));
	}

	private IEnumerator WaitForLoadToStart(string levelName)
	{
		while (!Application.CanStreamedLevelBeLoaded(levelName))
		{
			yield return null;
		}
	}

	private IEnumerator WaitForLevel(string levelName, LoadMode loadMode, Action callback)
	{
		AsyncOperation asyncOperation;
		switch (loadMode)
		{
		default:
			yield break;
		case LoadMode.Overwrite:
			asyncOperation = Application.LoadLevelAsync(levelName);
			break;
		case LoadMode.Additive:
			asyncOperation = Application.LoadLevelAdditiveAsync(levelName);
			break;
		}
		yield return asyncOperation;
		callback();
	}

	private void UpdateGame()
	{
		Game.Update();
		if (Game.JoinState == MVJoinState.Playing)
		{
			if (IngameController == null)
			{
				Debug.Log((object)("JoinState: " + Game.JoinState));
				InitializePlayingState();
				IngameController.Initialize();
				IngameController.ShowBriefing();
			}
			IngameController.Update();
		}
	}

	private void Update()
	{
		if (UpdateController != null)
		{
			UpdateController.Update();
		}
	}

	public void UpdateControllerUpdate()
	{
		if (Game != null)
		{
			if (Game.ConnState == MVConnState.Exception || Game.ConnState == MVConnState.TimeoutDisconnect || Game.ConnState == MVConnState.SendError || Game.ConnState == MVConnState.Disconnected)
			{
				Debug.Log((object)("Game.ConnState " + Game.ConnState));
				try
				{
					TextSlotIndex messageIndex = TextSlotIndex.ConnectionLostMessage;
					if (Game.ConnState == MVConnState.Exception)
					{
						messageIndex = TextSlotIndex.ConnectionExceptionMessage;
					}
					else if (Game.ConnState == MVConnState.TimeoutDisconnect)
					{
						messageIndex = TextSlotIndex.ConnectionTimeoutMessage;
					}
					else if (Game.ConnState == MVConnState.SendError)
					{
						messageIndex = TextSlotIndex.ConnectionLostMessage;
					}
					UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(messageIndex, TextSlotIndex.ErrorHeadline).AddPositiveButton(TextSlotIndex.Confirm)
						.AddNegativeButton(TextSlotIndex.Reject)
						.SetOnResultCallback(OnReconnectDialogResult)
						.Show();
				}
				catch (Exception ex)
				{
					Debug.LogError((object)ex);
				}
				Game.ConnState = MVConnState.HandlingException;
				Debug.Log((object)Game.ConnState);
				CleanUp();
			}
			else if (catchUpdateLoopExceptions)
			{
				try
				{
					UpdateGame();
				}
				catch (Exception ex2)
				{
					Debug.LogWarning((object)("Exception in Update: " + ex2.ToString()));
				}
			}
			else
			{
				UpdateGame();
			}
		}
		AudioEventHandler.Update();
	}

	public void OnReconnectDialogResult(UXDialogBox dialog)
	{
		if ((Object)(object)BrowserComm != (Object)null)
		{
			if (dialog.DialogResult == UXDialogResult.Positive)
			{
				BrowserComm.ToWeb.ExternalCall("refresh");
			}
			else
			{
				BrowserComm.ToWeb.ExternalCall("goBack");
			}
		}
		else
		{
			Debug.LogError((object)"BrowserComm is null");
		}
	}

	public bool HandleInput()
	{
		if (Game != null && Game.JoinState == MVJoinState.Playing && IngameController != null)
		{
			IngameController.HandleInput();
			hotkeys.HandleInput();
		}
		return false;
	}

	public void LateUpdate()
	{
		if (IngameController != null)
		{
			IngameController.LateUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (UpdateController != null)
		{
			UpdateController.FixedUpdate();
		}
	}

	public void UpdateControllerFixedUpdate()
	{
		if (IngameController != null)
		{
			IngameController.FixedUpdate();
		}
	}

	private void InitializePlayingState()
	{
		hotkeys = new HotKeys();
		switch (GameMode)
		{
		case MVGameMode.Play:
			IngameController = new PlayController();
			break;
		case MVGameMode.Edit:
			IngameController = new EditorController();
			break;
		case MVGameMode.CharacterEditor:
			IngameController = new CharacterEditorController();
			break;
		}
	}

	private void CleanUp()
	{
		if (IngameController != null)
		{
			IngameController.Deinitialize();
		}
		if (Game.Peer != null)
		{
			Game.Peer.StopThread();
		}
		Game.Cleanup();
		Game = null;
		IngameController = null;
		hotkeys = null;
		GC.Collect();
		Application.LoadLevel("UnloadPlanet");
		UpdateController = null;
	}

	public void LeaveGame()
	{
		WOCM.AvatarLocal.GameObject.SetActiveRecursively(false);
		Game.Leave();
	}
}
