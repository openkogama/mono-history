using System;
using MV.Common;
using UnityEngine;

public class LobbyStatePlayModeController : MonoBehaviour
{
	private bool isInLobbyState = true;

	private bool wantsToEnterPlayState;

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
		this.inGameController = inGameController;
		this.lobbyState = lobbyState;
		this.inGameMenu = inGameMenu;
		this.chatController = chatController;
		SetObjectToLobbyState(isInLobbyState);
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
			if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
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
		if (MVGameControllerBase.WOCM.AvatarLocal.CurrentState == AvatarRuntimeState.Hidden)
		{
			lobbyState.gameObject.SetActive(value: true);
			inGameMenu.gameObject.SetActive(value: false);
			inGameController.gameObject.SetActive(value: false);
			chatController.OnLobbyStateChange(cursorLocked: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.CurrentState != AvatarRuntimeState.Edit)
		{
			lobbyState.gameObject.SetActive(value: false);
			inGameMenu.gameObject.SetActive(value: true);
			inGameController.gameObject.SetActive(value: false);
			chatController.OnLobbyStateChange(cursorLocked: false);
		}
	}

	private void DeactivateLobbyState()
	{
		lobbyState.gameObject.SetActive(value: false);
		inGameMenu.gameObject.SetActive(value: false);
		inGameController.gameObject.SetActive(value: true);
		chatController.OnLobbyStateChange(cursorLocked: true);
	}
}
