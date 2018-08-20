using System;
using MV.Common;
using UnityEngine;

public class LobbyStatePlayModeController : MonoBehaviour
{
	private bool isInLobbyState = true;

	private bool wantsToEnterPlayState;

	private DesktopInGameGUIController inGameController;

	private RectTransform lobbyState;

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

	public void Initialize(DesktopInGameGUIController inGameController, RectTransform lobbyState, ChatControllerUGUI chatController)
	{
		ILockCursorManager lockCursorManager = MVGameControllerDesktop.LockCursorManager;
		lockCursorManager.OnCursorLockChanged = (Action<bool>)Delegate.Combine(lockCursorManager.OnCursorLockChanged, new Action<bool>(OnCursorLockChanged));
		this.inGameController = inGameController;
		this.lobbyState = lobbyState;
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
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round && wantsToEnterPlayState;
		if (flag && isInLobbyState)
		{
			Debug.Log("To play state");
			if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			}
			SetObjectToLobbyState(isInLobbyState: false);
		}
		else if (!flag && !isInLobbyState)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			SetObjectToLobbyState(isInLobbyState: true);
		}
	}

	private void SetObjectToLobbyState(bool isInLobbyState)
	{
		this.isInLobbyState = isInLobbyState;
		lobbyState.gameObject.SetActive(isInLobbyState);
		inGameController.gameObject.SetActive(!isInLobbyState);
		chatController.OnLobbyStateChange(!isInLobbyState);
	}
}
