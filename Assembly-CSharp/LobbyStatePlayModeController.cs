using System;
using MV.Common;
using UnityEngine;

public class LobbyStatePlayModeController : MonoBehaviour
{
	private bool isInLobbyState = true;

	private bool wantsToEnterPlayState;

	private bool shouldOpenLobbyMenu = true;

	private DesktopInGameGUIController inGameController;

	private RectTransform lobbyState;

	private InGameMenu inGameMenu;

	private ChatControllerUGUI chatController;

	public bool IsInLobbyState
	{
		get
		{
			return isInLobbyState;
		}
		set
		{
			wantsToEnterPlayState = !value;
			Update();
		}
	}

	public void Initialize(DesktopInGameGUIController inGameController, RectTransform lobbyState, InGameMenu inGameMenu, ChatControllerUGUI chatController)
	{
		ILockCursorManager lockCursorManager = MVGameControllerDesktop.LockCursorManager;
		lockCursorManager.OnCursorLockChanged = (Action<bool>)Delegate.Combine(lockCursorManager.OnCursorLockChanged, new Action<bool>(OnCursorLockChanged));
		GameEventManager.GameStateManager gameState = MVGameControllerBase.GameEventManager.GameState;
		gameState.OnEnableLobbyState = (Action)Delegate.Combine(gameState.OnEnableLobbyState, new Action(EnableLobbyState));
		this.inGameController = inGameController;
		this.lobbyState = lobbyState;
		this.inGameMenu = inGameMenu;
		this.chatController = chatController;
		SetObjectToLobbyState(isInLobbyState);
		shouldOpenLobbyMenu = false;
	}

	private void OnCursorLockChanged(bool cursorLocked)
	{
		Debug.Log("On cursor change " + cursorLocked);
		wantsToEnterPlayState = cursorLocked;
	}

	private void Update()
	{
		bool flag = wantsToEnterPlayState;
		if (flag && isInLobbyState)
		{
			Debug.Log("To play state");
			SetObjectToLobbyState(isInLobbyState: false);
			if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
			{
				Debug.Log("MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing)");
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
			}
		}
		else if (!flag && !isInLobbyState)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			SetObjectToLobbyState(isInLobbyState: true);
		}
	}

	private void SetObjectToLobbyState(bool isInLobbyState)
	{
		if (isInLobbyState)
		{
			ActivateLobbyState();
		}
		else
		{
			DeactivateLobbyState();
		}
		this.isInLobbyState = isInLobbyState;
	}

	private void ActivateLobbyState()
	{
		lobbyState.gameObject.SetActive(shouldOpenLobbyMenu);
		inGameMenu.gameObject.SetActive(!shouldOpenLobbyMenu);
		inGameController.gameObject.SetActive(value: false);
		chatController.OnLobbyStateChange(cursorLocked: false);
		shouldOpenLobbyMenu = false;
	}

	private void DeactivateLobbyState()
	{
		lobbyState.gameObject.SetActive(value: false);
		inGameMenu.gameObject.SetActive(value: false);
		inGameController.gameObject.SetActive(value: true);
		chatController.OnLobbyStateChange(cursorLocked: true);
	}

	private void EnableLobbyState()
	{
		if (!lobbyState.gameObject.activeSelf)
		{
			shouldOpenLobbyMenu = true;
		}
	}
}
