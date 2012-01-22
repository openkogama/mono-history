using System;
using System.Collections;
using UnityEngine;

public class MVGameController : MonoBehaviour, IInputHandler
{
	private static MVGameController instance;

	private ILogger logger;

	public string ip = "127.0.0.1";

	public int portDev = 5056;

	public int portTest = 5055;

	public string username = "TestUser";

	public string password = "TestUser";

	public string planetName = "TestPlanet";

	private string gameName = string.Empty;

	private string sessionLocatorUrlBase;

	private string gameSessionLocatorUrlExt = "/PHP/GameSessionLocator.php?";

	private string editSessionLocatorUrlExt = "/PHP/EditSessionLocator.php?";

	private bool playInEditor;

	private bool returnToEditor;

	private MVNetworkGame game;

	private IIngameController ingameController;

	private HotKeys hotkeys;

	public MVGUIManager guiManager;

	private MVWorldObjectClientManager worldObjectClientManager;

	public bool fixedToYPlane = true;

	public MVNetworkGame Game => game;

	public MVWorldObjectClientManager WOCM => worldObjectClientManager;

	public IIngameController IngameController => ingameController;

	public EditorController EditorController
	{
		get
		{
			if (ingameController != null && ingameController is EditorController)
			{
				return ingameController as EditorController;
			}
			return null;
		}
	}

	public static MVGameController Instance
	{
		get
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected Obj, but got Unknown
			if ((Object)(object)instance == (Object)null)
			{
				instance = Object.FindObjectOfType(typeof(MVGameController)) as MVGameController;
				instance.logger = LoggerManager.Instance.GetLogger(typeof(MVGameController));
			}
			if ((Object)(object)instance == (Object)null)
			{
				GameObject val = new GameObject("AManager");
				instance = val.AddComponent(typeof(MVGameController)) as MVGameController;
				Debug.LogWarning((object)"This should not be called before start");
			}
			return instance;
		}
	}

	public int Priority => 2000;

	private void OnApplicationQuit()
	{
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Unregister(instance);
		instance = null;
	}

	private void Awake()
	{
		AudioEventHandler.Awake();
	}

	private void Start()
	{
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		Application.runInBackground = true;
		string[] array = Application.srcValue.Split(new char[1] { '?' });
		bool flag = false;
		if (array.Length > 1)
		{
			string text = array[1];
			if (text.Length > 0)
			{
				Debug.Log((object)("Web initparams: " + text));
				string[] array2 = text.Split(new char[1] { '&' });
				string[] array3 = array2;
				foreach (string text2 in array3)
				{
					Debug.Log((object)("webParam: " + text2));
					string[] array4 = text2.Split(new char[1] { '=' });
					switch (array4[0])
					{
					case "Username":
						username = (string)array4[1].Clone();
						if (username.Length == 0)
						{
							username = "Anonymous";
						}
						Debug.Log((object)("Web setting username = " + username));
						break;
					case "Password":
						password = (string)array4[1].Clone();
						Debug.Log((object)("Web setting password = " + password));
						break;
					case "PlanetName":
						planetName = (string)array4[1].Clone();
						Debug.Log((object)("Web setting planetName = " + planetName));
						break;
					case "EditMode":
						flag = array4[1].ToLower() == "true";
						break;
					}
				}
			}
			if (Application.absoluteURL.Length > 0)
			{
				sessionLocatorUrlBase = Application.absoluteURL.Substring(0, Application.absoluteURL.LastIndexOf("WebPlayer.unity3d") - 1);
				sessionLocatorUrlBase = sessionLocatorUrlBase.Substring(0, sessionLocatorUrlBase.LastIndexOf('/'));
			}
			guiManager.HideLoginView();
			if (flag)
			{
				EditPlanet(fromWeb: true);
			}
			else
			{
				JoinPlanet(fromWeb: true);
			}
		}
		else
		{
			guiManager.ShowLoginView();
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		if (Application.loadedLevelName == "LoadPlanet")
		{
			guiManager.ShowInGameMenu();
		}
		else
		{
			if (!(Application.loadedLevelName == "UnloadPlanet"))
			{
				return;
			}
			if (playInEditor)
			{
				JoinPlanetInPlayTest();
				return;
			}
			if (returnToEditor)
			{
				EditPlanet(Application.isWebPlayer);
				return;
			}
			Debug.Log((object)"Scene loaded");
			if (!Application.isWebPlayer)
			{
				guiManager.ShowLoginView();
			}
			else
			{
				guiManager.HideLoginView();
				UXScreen uXScreen = Object.FindObjectOfType(typeof(UXScreen)) as UXScreen;
				if (uXScreen.Fullscreen)
				{
					uXScreen.Fullscreen = false;
				}
			}
			guiManager.HideGameHUD();
			game = null;
			GC.Collect();
		}
	}

	private void Update()
	{
		if (game != null)
		{
			if (game.ConnState == MVConnState.Exception)
			{
				guiManager.ShowReconnectDialog("Connection Exception\nReconnect?", OnReconnectFromDialog, OnDisconnectFromDialog);
				game.ConnState = MVConnState.HandlingException;
			}
			else if (game.ConnState == MVConnState.TimeoutDisconnect)
			{
				guiManager.ShowReconnectDialog("Connection Timeout.\nReconnect?", OnReconnectFromDialog, OnDisconnectFromDialog);
				game.ConnState = MVConnState.HandlingException;
			}
			else if (game.ConnState == MVConnState.SendError)
			{
				guiManager.ShowReconnectDialog("Connection Lost.\nReconnect?", OnReconnectFromDialog, OnDisconnectFromDialog);
				game.ConnState = MVConnState.HandlingException;
			}
			else if (game.ConnState == MVConnState.Disconnected)
			{
				Debug.Log((object)"MVGame disconnected - destroy game session");
				CleanUp();
			}
			else
			{
				try
				{
					game.Update();
					if (game.JoinState == MVJoinState.Playing)
					{
						if (ingameController == null)
						{
							InitializePlayingState();
							ingameController.Initialize();
						}
						ingameController.Update();
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("Exception in Update: " + ex.ToString()));
				}
			}
		}
		AudioEventHandler.Update();
	}

	public void OnReconnectFromDialog()
	{
		Debug.Log((object)"Reconnect from Reconnect-dialog");
		string text = game.PlanetName;
		string text2 = game.GameName;
		string text3 = game.Ip;
		int port = game.Port;
		bool editorMode = game.EditorMode;
		game = null;
		worldObjectClientManager.Cleanup();
		worldObjectClientManager = null;
		ingameController = null;
		GC.Collect();
		game = new MVNetworkGame(text, text2, text3, port, editorMode, guiManager);
		worldObjectClientManager = new MVWorldObjectClientManager();
		if (!game.Join(username, password))
		{
			guiManager.ShowLoginView();
			Debug.Log((object)("Unable to connect to game on address: " + text3 + ":" + portTest));
			game = null;
			worldObjectClientManager = null;
		}
	}

	public void OnDisconnectFromDialog()
	{
		Debug.Log((object)"Disconnect from Reconnect-dialog");
		CleanUp();
		Application.Quit();
	}

	public bool HandleInput()
	{
		if (game != null && game.JoinState == MVJoinState.Playing && ingameController != null)
		{
			ingameController.HandleInput();
			hotkeys.HandleInput();
		}
		return false;
	}

	public void LateUpdate()
	{
		if (ingameController != null)
		{
			ingameController.LateUpdate();
		}
	}

	public void FixedUpdate()
	{
		if (ingameController != null)
		{
			ingameController.FixedUpdate();
		}
	}

	private void InitializePlayingState()
	{
		hotkeys = new HotKeys();
		if (game.EditorMode)
		{
			ingameController = new EditorController();
			return;
		}
		ingameController = new PlayController();
		Camera camera = ((Component)Instance.WOCM.WeCamera).camera;
		if ((camera.cullingMask & LayerMask.NameToLayer("Logic")) == LayerMask.NameToLayer("Logic"))
		{
			camera.cullingMask -= 1 << (LayerMask.NameToLayer("Logic") & 0x1F);
		}
	}

	private void CleanUp()
	{
		if (ingameController != null)
		{
			ingameController.Deinitialize();
		}
		if (game.Peer != null)
		{
			game.Peer.StopThread();
		}
		game = null;
		ingameController = null;
		hotkeys = null;
		worldObjectClientManager.Cleanup();
		worldObjectClientManager = null;
		GC.Collect();
		Application.LoadLevel("UnloadPlanet");
	}

	private IEnumerator JoinFromWeb()
	{
		string gameSessionLocatorUrl = sessionLocatorUrlBase + gameSessionLocatorUrlExt + "ProfileID=0&PlanetName=" + planetName;
		Debug.Log((object)gameSessionLocatorUrl);
		WWW www = new WWW(gameSessionLocatorUrl);
		yield return www;
		string returnFromPHP = www.text;
		Debug.Log((object)returnFromPHP);
		int indexStatusText = returnFromPHP.IndexOf("STATUS:");
		int indexGSText = returnFromPHP.IndexOf("GAMESERVERIP:");
		int indexGNText = returnFromPHP.IndexOf("GAMENAME:");
		int statusValueStartIndex = returnFromPHP.IndexOf("'", indexStatusText) + 1;
		int statusValueEndIndex = returnFromPHP.IndexOf("'", statusValueStartIndex);
		int ipValueStartIndex = returnFromPHP.IndexOf("'", indexGSText) + 1;
		int ipValueEndIndex = returnFromPHP.IndexOf("'", ipValueStartIndex);
		int gnValueStartIndex = returnFromPHP.IndexOf("'", indexGNText) + 1;
		int gnValueEndIndex = returnFromPHP.IndexOf("'", gnValueStartIndex);
		int status = Convert.ToInt32(returnFromPHP.Substring(statusValueStartIndex, statusValueEndIndex - statusValueStartIndex));
		ip = returnFromPHP.Substring(ipValueStartIndex, ipValueEndIndex - ipValueStartIndex);
		gameName = returnFromPHP.Substring(gnValueStartIndex, gnValueEndIndex - gnValueStartIndex);
		Debug.Log((object)("Status:" + status));
		Debug.Log((object)("ServerIP: " + ip));
		Debug.Log((object)("GameName: " + gameName));
		Debug.Log((object)("GameName length: " + gameName.Length));
		if (gameName == "null")
		{
			gameName = planetName;
		}
		switch (status)
		{
		case 1:
			guiManager.ShowMessageBox("The Planet Name '" + planetName + "'\ndoes not exist in the database");
			break;
		case 2:
			guiManager.HideLoginView();
			game = new MVNetworkGame(planetName, gameName, ip, portTest, editorMode: false, guiManager);
			worldObjectClientManager = new MVWorldObjectClientManager();
			if (!game.Join(username, password))
			{
				guiManager.ShowLoginView();
				Debug.Log((object)("Unable to connect to game on address: " + ip + ":" + portTest));
				game = null;
				worldObjectClientManager = null;
			}
			break;
		case 3:
			guiManager.ShowMessageBox("No game servers found!");
			break;
		case 4:
			guiManager.HideLoginView();
			game = new MVNetworkGame(planetName, gameName, ip, portTest, editorMode: false, guiManager);
			worldObjectClientManager = new MVWorldObjectClientManager();
			if (!game.Join(username, password))
			{
				guiManager.ShowLoginView();
				Debug.Log((object)("Unable to connect to game on address: " + ip + ":" + portTest));
				game = null;
				worldObjectClientManager = null;
			}
			break;
		}
		www.Dispose();
	}

	public void JoinPlanet(bool fromWeb)
	{
		if (fromWeb)
		{
			((MonoBehaviour)this).StartCoroutine(JoinFromWeb());
			return;
		}
		guiManager.HideLoginView();
		game = new MVNetworkGame(planetName, planetName, ip, portDev, editorMode: false, guiManager);
		worldObjectClientManager = new MVWorldObjectClientManager();
		if (!game.Join(username, password))
		{
			guiManager.ShowLoginView();
			Debug.Log((object)("Unable to connect to game on address: " + ip + ":" + portDev));
			game = null;
			worldObjectClientManager = null;
		}
	}

	public void JoinPlanetInPlayTest()
	{
		game = new MVNetworkGame(planetName, planetName, ip, portDev, editorMode: false, guiManager);
		worldObjectClientManager = new MVWorldObjectClientManager();
		playInEditor = false;
		returnToEditor = true;
		if (!game.Join(username, password))
		{
			Debug.Log((object)("Unable to connect to game on address: " + ip + ":" + portDev));
			game = null;
			worldObjectClientManager = null;
		}
	}

	private IEnumerator EditFromWeb()
	{
		string editSessionLocatorUrl = sessionLocatorUrlBase + editSessionLocatorUrlExt + "PlanetName=" + planetName;
		Debug.Log((object)editSessionLocatorUrl);
		WWW www = new WWW(editSessionLocatorUrl);
		yield return www;
		string returnFromPHP = www.text;
		Debug.Log((object)returnFromPHP);
		int indexStatusText = returnFromPHP.IndexOf("STATUS:");
		int indexGSText = returnFromPHP.IndexOf("GAMESERVERIP:");
		int statusValueStartIndex = returnFromPHP.IndexOf("'", indexStatusText) + 1;
		int statusValueEndIndex = returnFromPHP.IndexOf("'", statusValueStartIndex);
		int ipValueStartIndex = returnFromPHP.IndexOf("'", indexGSText) + 1;
		int ipValueEndIndex = returnFromPHP.IndexOf("'", ipValueStartIndex);
		int status = Convert.ToInt32(returnFromPHP.Substring(statusValueStartIndex, statusValueEndIndex - statusValueStartIndex));
		ip = returnFromPHP.Substring(ipValueStartIndex, ipValueEndIndex - ipValueStartIndex);
		Debug.Log((object)("Status:" + status));
		Debug.Log((object)("ServerIP: " + ip));
		gameName = planetName;
		switch (status)
		{
		case 1:
			guiManager.ShowMessageBox("Project named '" + planetName + "' does not exist");
			break;
		case 2:
			guiManager.ShowMessageBox("Error with the Internet and/or database connection...");
			break;
		case 3:
			guiManager.HideLoginView();
			game = new MVNetworkGame(planetName, gameName, ip, portTest, editorMode: true, guiManager);
			worldObjectClientManager = new MVWorldObjectClientManager();
			if (!game.Join(username, password))
			{
				guiManager.ShowLoginView();
				Debug.Log((object)("Unable to connect to project on address: " + ip + ":" + portTest));
				game = null;
				worldObjectClientManager = null;
			}
			break;
		}
	}

	public void EditPlanet(bool fromWeb)
	{
		if (fromWeb)
		{
			((MonoBehaviour)this).StartCoroutine(EditFromWeb());
			return;
		}
		guiManager.HideLoginView();
		game = new MVNetworkGame(planetName, planetName, ip, portDev, editorMode: true, guiManager);
		worldObjectClientManager = new MVWorldObjectClientManager();
		if (!game.Join(username, password))
		{
			guiManager.ShowLoginView();
			Debug.Log((object)("Unable to connect to game on address: " + ip + ":" + portDev));
			guiManager.ShowMessageBox("Unable to connect to the game.\nPlease check your Internet connection.");
			game = null;
			worldObjectClientManager = null;
		}
	}

	public void PlayInEditor()
	{
		Debug.Log((object)"PlayInEditor... (not yet implemented)");
		playInEditor = true;
		LeaveGame();
	}

	public void LeaveGame()
	{
		WOCM.WoAvatar.GameObject.SetActiveRecursively(false);
		game.Leave();
	}

	public void ToggleEditMode()
	{
	}

	public void ToggleGrid()
	{
		if (ingameController is EditorController)
		{
			(ingameController as EditorController).ToggleGrid();
		}
	}

	public void ToggleWorkPlane()
	{
		if (ingameController is EditorController)
		{
			(ingameController as EditorController).ToggleWorkPlane();
		}
	}

	public void ToggleLogicRendering()
	{
		if (ingameController is EditorController)
		{
			(ingameController as EditorController).ToggleLogicRendering();
		}
	}
}
