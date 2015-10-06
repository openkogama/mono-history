using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PlayControllerBase : AIngameController
{
	private bool showingIcon;

	private MVGUIPressEToUsePrompt pressEToUsePrompt;

	private MVGUIGameInfo gameInfo;

	private MVGUIWinningConditionBriefingView winningConditionBriefingView;

	private MVGUIWinningConditionDebriefingView winningConditionDebriefingView;

	protected MVGUIBottomCenterToggles bottomCenterToggles;

	private MVGUILevel level;

	private MVGUIResume resume;

	private MVGUIFullscreenToggle fullscreenToggle;

	private MVGUIMuteToggle muteToggle;

	private GameMetersController gameMetersController;

	protected MVGUIMenu menu;

	protected LockCursorManager lockCursorManager;

	protected bool briefingWasShown;

	private MVNetworkGameStateListener GameStateListener => MVGameController.Game.NetworkGameStateListener;

	public override bool WindowShown => (currentView != null && currentView.isVisible) || menu.View.isVisible;

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIPlayModeBase>().gameObject;
		gameInfo = AIngameController.FindGUIObjectOfType<MVGUIGameInfo>(gameObject);
		winningConditionBriefingView = AIngameController.FindGUIObjectOfType<MVGUIWinningConditionBriefingView>(gameObject);
		winningConditionDebriefingView = AIngameController.FindGUIObjectOfType<MVGUIWinningConditionDebriefingView>(gameObject);
		pressEToUsePrompt = AIngameController.FindGUIObjectOfType<MVGUIPressEToUsePrompt>(gameObject);
		menu = AIngameController.FindGUIObjectOfType<MVGUIMenu>(gameObject);
		level = AIngameController.FindGUIObjectOfType<MVGUILevel>(gameObject);
		resume = AIngameController.FindGUIObjectOfType<MVGUIResume>(gameObject);
		lockCursorManager = AIngameController.FindGUIObjectOfType<LockCursorManager>(gameObject);
		fullscreenToggle = AIngameController.FindGUIObjectOfType<MVGUIFullscreenToggle>(gameObject);
		muteToggle = AIngameController.FindGUIObjectOfType<MVGUIMuteToggle>(gameObject);
		gameMetersController = AIngameController.FindGUIObjectOfType<GameMetersController>(gameObject);
	}

	protected void Show()
	{
		gameInfo.View.Show();
		bottomCenterToggles.View.Show();
		level.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
		gameMetersController.View.Show();
		winningConditionDebriefingView.View.Show();
	}

	protected virtual void Hide()
	{
		gameInfo.View.Hide();
		bottomCenterToggles.View.Hide();
		ClearGameMsg();
		winningConditionDebriefingView.View.Hide();
		level.View.Hide();
		resume.View.Hide();
		fullscreenToggle.View.Hide();
		muteToggle.View.Hide();
		gameMetersController.View.Hide();
	}

	public override void HandleInput()
	{
		base.HandleInput();
		HandlePlayModeHotKeys();
	}

	private void HandlePlayModeHotKeys()
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.Respawn))
		{
			RespawnAvatar();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.DropCurrentItem))
		{
			MVEquipable component = MVGameController.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Equip(AvatarItemType.Hand, AvatarEquipableType.Weapon, null);
			}
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowPlayerWindow))
		{
			ShowPlayersWindow(show: false);
		}
		else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ShowPlayerWindow))
		{
			ShowPlayersWindow(show: true);
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowChat) && (MVClientSettings.TouristChatAllowed || !MVGameController.Game.IsTouristSession))
		{
			chatController.ShowChat(takeControl: true, retainControlAfterMessageSend: false);
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.LobbyMenu))
		{
			LockCursorManager.LockCursor = false;
		}
	}

	private void InitializeResumeButton()
	{
		LockCursorManager.OnCursorLockChanged = (Action<bool>)Delegate.Combine(LockCursorManager.OnCursorLockChanged, new Action<bool>(FocusChanged));
		resume.button.OnClick = () =>
		{
			LockCursorManager.LockCursor = true;
			ShowBriefing();
			Debug.Log("Briefing shown");
		};
		FocusChanged(LockCursorManager.HasFocusAndLockCursor);
	}

	protected virtual void FocusChanged(bool hasFocus)
	{
		if (hasFocus)
		{
			HideLostFocusGUI();
		}
		else
		{
			ShowLostFocusGUI();
		}
		MVInputWrapper.ignoreInGameInput = !hasFocus;
	}

	protected virtual void ShowLostFocusGUI()
	{
		resume.View.Show();
		bottomCenterToggles.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
		winningConditionDebriefingView.View.Hide();
		ClearGameMsg();
		gameMetersController.View.Hide();
		MVGameController.Game.CameraController.PushCamera(CameraType.LobbyState);
	}

	protected virtual void HideLostFocusGUI()
	{
		resume.View.Hide();
		bottomCenterToggles.View.Hide();
		fullscreenToggle.View.Hide();
		muteToggle.View.Hide();
		gameMetersController.View.Show();
		winningConditionDebriefingView.View.Show();
		winningConditionDebriefingView.group.SetVisible(visible: true);
		MVGameController.Game.CameraController.RemoveCamera(CameraType.LobbyState);
	}

	public void ShowEUseIcon(ShowUseOption option = ShowUseOption.Normal)
	{
		if (!showingIcon)
		{
			pressEToUsePrompt.Show(option);
			showingIcon = true;
		}
	}

	public void HideEUseIcon()
	{
		if (showingIcon)
		{
			pressEToUsePrompt.View.Hide();
			showingIcon = false;
		}
	}

	public bool IsMenuShown()
	{
		return menu.View.isVisible;
	}

	public void ShowPlayersWindow(bool show)
	{
		menu.ShowOnShortcut(show);
	}

	public override void Initialize()
	{
		base.Initialize();
		GameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
		Show();
		InitializeResumeButton();
	}

	public void ClearGameMsg()
	{
		winningConditionBriefingView.Clear();
	}

	public void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		if (MVGameController.GameMode == MVGameMode.Play || (MVGameController.GameMode == MVGameMode.Edit && MVGameController.EditorController.PlayInEditor))
		{
			menu.View.Hide();
			if (UXUtils.UXDialogFactory.CurrentDialogBox != null)
			{
				UXUtils.UXDialogFactory.CloseDialog();
			}
			UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/WinnerScreen/WinnerDialog", string.Empty, noButtons: true, stackDialog: true, canClose: false);
			ClearGameMsg();
			GenerateDebriefing(winningCondition);
			HandleXp(winningCondition);
		}
	}

	public void ShowBriefing()
	{
		if (!briefingWasShown)
		{
			GenerateBriefing();
			briefingWasShown = true;
		}
	}

	private void GenerateDebriefing(IWinningCondition winningCondition)
	{
		if (winningCondition is IWinningConditionBriefing)
		{
			((IWinningConditionBriefing)winningCondition).GetDebriefing(winningConditionDebriefingView);
			winningConditionDebriefingView.group.SetVisible(LockCursorManager.LockCursor);
		}
	}

	private void GenerateBriefing()
	{
		winningConditionBriefingView.Clear();
		List<IWinningCondition> winnerConditions = new List<IWinningCondition>();
		MVGameController.Game.WinningConditionManager.Traverse((IWinningCondition winnerCondition) =>
		{
			if (winnerCondition.IsBriefingNode)
			{
				winnerConditions.Add(winnerCondition);
			}
			return false;
		});
		bool flag = true;
		foreach (IWinningCondition item in winnerConditions)
		{
			if (flag)
			{
				flag = false;
			}
			else if (!(item.Parent is WinningConditionOr) && !(item.Parent is WinningConditionAnd))
			{
			}
			string message = $"No localized briefing for: {item.GetType()}";
			if (item is IWinningConditionBriefing)
			{
				((IWinningConditionBriefing)item).GetBriefing(winningConditionBriefingView);
			}
			else
			{
				Debug.LogWarning(message);
			}
		}
		winningConditionBriefingView.View.Show();
	}

	private void GameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			break;
		case MVGameStateType.Round:
			winningConditionDebriefingView.Clear();
			LockCursorManager.LockCursor = false;
			break;
		case MVGameStateType.RoundEnded:
			break;
		}
	}

	private void HandleXp(IWinningCondition winningCondition)
	{
		if (!winningCondition.IsTeamMode)
		{
			List<ScoreActorEntry> list = winningCondition.HighScores.GenerateActorScores();
			if (list.Count > 0 && list[0].actorNumber == MVGameController.Game.LocalPlayer.ActorNr)
			{
				GameSessionCounters.Increment(GameSessionCounterType.GameWon);
			}
		}
	}

	private void HideDebriefing()
	{
		winningConditionDebriefingView.Clear();
	}
}
