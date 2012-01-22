using MV.Common;
using UnityEngine;

public class PlayController : IIngameController
{
	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(PlayController));

	private Chat chat;

	private MVGUIRespawnButton respawnButton;

	private MVGUISocialWindowToggle socialWindowToggle;

	private MVGUISocialWindow socialWindow;

	private MVGUIGameHUD gameHud;

	private MVGUIGameState gameState;

	public MVNetworkGame Game => MVGameController.Instance.Game;

	public MVNetworkGameStateListener GameStateListener => MVGameController.Instance.Game.NetworkGameStateListener;

	public PlayController()
	{
		GameObject gameObject = ((Component)(MVCameraController)(object)Object.FindObjectOfType(typeof(MVCameraController))).gameObject;
		MVGameController.Instance.WOCM.WeCamera = gameObject.GetComponent<MVCameraController>();
		MVGameController.Instance.WOCM.WeCamera.Init();
		MVGameController.Instance.WOCM.WeCamera.SetCamera(CameraType.ThirdPerson);
		GameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
		logger.Log("Initialized");
	}

	private void GameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			break;
		case MVGameStateType.Round:
			MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.Respawn();
			break;
		case MVGameStateType.RoundEnded:
			break;
		}
	}

	public override void Initialize()
	{
		InitializeChat();
		gameHud = Object.FindObjectOfType(typeof(MVGUIGameHUD)) as MVGUIGameHUD;
		gameHud.View.Show();
		gameHud.planetNameText.Text = MVGameController.Instance.planetName;
		gameState = Object.FindObjectOfType(typeof(MVGUIGameState)) as MVGUIGameState;
		gameState.View.Show();
		gameState.gameMsgs.Text = string.Empty;
		respawnButton = Object.FindObjectOfType(typeof(MVGUIRespawnButton)) as MVGUIRespawnButton;
		respawnButton.View.Show();
		socialWindow = Object.FindObjectOfType(typeof(MVGUISocialWindow)) as MVGUISocialWindow;
		socialWindowToggle = Object.FindObjectOfType(typeof(MVGUISocialWindowToggle)) as MVGUISocialWindowToggle;
		socialWindowToggle.View.Show();
		socialWindow.InitializeListeners();
	}

	public void InitializeChat()
	{
		chat = new Chat();
		chat.Show();
	}

	public override void Deinitialize()
	{
	}

	public override void Update()
	{
		SetGameMsg();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}

	public override void FixedUpdate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (GameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVGameController.Instance.WOCM.WeCamera.UpdateCamera();
		}
		else
		{
			((Component)MVGameController.Instance.WOCM.WeCamera).transform.Rotate(Vector3.up, 0.1f);
		}
	}

	public override void HandleInput()
	{
		if (MVInputWrapper.GetKeyDown((KeyCode)116))
		{
			chat.Activate();
		}
		if (GameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVGameController.Instance.WOCM.WeCamera.HandleInput();
		}
	}

	public void SetGameMsg()
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Editing)
		{
			return;
		}
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			gameState.gameMsgs.Text = "New Game starting in: " + Mathf.Ceil((float)(GameStateListener.TimeLeftMS / 1000));
			break;
		case MVGameStateType.Round:
			gameState.gameMsgs.Text = string.Empty;
			break;
		case MVGameStateType.RoundEnded:
		{
			string text = string.Empty;
			if (GameStateListener.LastReason == MVGameStateReason.Timeout)
			{
				text = "ROUND ENDS - NO WINNER!";
			}
			else if (GameStateListener.LastReason == MVGameStateReason.FlagCaptured)
			{
				if (GameStateListener.LastInstigatorActorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
				{
					text = "You WON the Game!";
				}
				else
				{
					text = ((!MVGameController.Instance.WOCM.Players.ContainsKey(GameStateListener.LastInstigatorActorNr)) ? "Game won by...unknown" : ("Game won by " + MVGameController.Instance.WOCM.Players[GameStateListener.LastInstigatorActorNr].Username));
				}
			}
			gameState.gameMsgs.Text = text;
			break;
		}
		}
	}
}
