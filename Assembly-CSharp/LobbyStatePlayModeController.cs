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
		GameEventManager.GameStateManager gameState2 = MVGameControllerBase.GameEventManager.GameState;
		gameState2.OnDisableLobbyState = (Action)Delegate.Combine(gameState2.OnDisableLobbyState, new Action(DisableLobbyState));
		this.inGameController = inGameController;
		this.lobbyState = lobbyState;
		this.inGameMenu = inGameMenu;
		this.chatController = chatController;
		SetObjectToLobbyState(isInLobbyState);
		shouldOpenLobbyMenu = false;
	}

	private void OnCursorLockChanged(bool cursorLocked)
	{
		wantsToEnterPlayState = cursorLocked;
	}

	private void Update()
	{
		bool flag = wantsToEnterPlayState;
		if (flag && isInLobbyState)
		{
			SetObjectToLobbyState(isInLobbyState: false);
			if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
			}
		}
		else if (!flag && !isInLobbyState)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			SetObjectToLobbyState(isInLobbyState: true);
		}
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode == SpawnRoleModeType.Playing && !MVGameControllerDesktop.LockCursorManager.CursorLock && !MVGameControllerBase.PlayModeUI.InLobbyState)
		{
			Debug.Log("This happens when playmode avatar is in lobby mode while edit mode avatar tries to spawn as a spawn role when a time attack flag is present");
			Debug.LogError("Bad state");
			MVGameControllerBase.PlayModeUI.InLobbyState = true;
		}
	}

	private void SetObjectToLobbyState(bool isInLobbyState)
	{
		this.isInLobbyState = isInLobbyState;
		if (isInLobbyState)
		{
			ActivateLobbyState();
		}
		else
		{
			DeactivateLobbyState();
		}
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

	private void DisableLobbyState()
	{
		shouldOpenLobbyMenu = false;
		if (lobbyState.gameObject.activeSelf)
		{
			lobbyState.gameObject.SetActive(value: false);
			inGameMenu.gameObject.SetActive(value: true);
		}
	}
}
