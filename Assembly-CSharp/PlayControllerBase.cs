using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PlayControllerBase : AIngameController, IPlayModeUI
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

	protected ILockCursorManager lockCursorManager;

	protected bool briefingWasShown;

	private MVGUICrossHairLegacy guiCrossHairLegacy = new MVGUICrossHairLegacy();

	private MVNetworkGameStateListener GameStateListener => MVGameControllerBase.Game.NetworkGameStateListener;

	public override bool WindowShown => (currentView != null && currentView.isVisible) || menu.View.isVisible;

	public bool InLobbyState
	{
		get
		{
			return !lockCursorManager.LockCursor;
		}
		set
		{
			lockCursorManager.LockCursor = !value;
		}
	}

	public PlayControllerBase()
	{
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIRoot>().gameObject;
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			lockCursorManager = AIngameController.FindGUIObjectOfType<LockCursorManagerPlatformer>(gameObject);
			((LockCursorManagerPlatformer)lockCursorManager).gameObject.SetActive(value: true);
		}
		else
		{
			lockCursorManager = AIngameController.FindGUIObjectOfType<LockCursorManager>(gameObject);
			((LockCursorManager)lockCursorManager).gameObject.SetActive(value: true);
		}
	}

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
		fullscreenToggle = AIngameController.FindGUIObjectOfType<MVGUIFullscreenToggle>(gameObject);
		muteToggle = AIngameController.FindGUIObjectOfType<MVGUIMuteToggle>(gameObject);
		gameMetersController = AIngameController.FindGUIObjectOfType<GameMetersController>(gameObject);
	}

	protected virtual void Show()
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
			MVEquipable component = MVGameControllerBase.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
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
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowChat) && (MVClientSettings.TouristChatAllowed || !MVGameControllerBase.IsTouristSession))
		{
			chatController.ShowChat(takeControl: true, retainControlAfterMessageSend: false);
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.LobbyMenu))
		{
			lockCursorManager.LockCursor = false;
		}
	}

	private void InitializeResumeButton()
	{
		ILockCursorManager lockCursorManager = this.lockCursorManager;
		lockCursorManager.OnCursorLockChanged = (Action<bool>)Delegate.Combine(lockCursorManager.OnCursorLockChanged, new Action<bool>(FocusChanged));
		UXBaseButton button = resume.button;
		button.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(button.OnClick, new UXBaseButton.OnClickDelegate(Resume));
		FocusChanged(this.lockCursorManager.HasFocusAndLockCursor);
	}

	private void Resume()
	{
		lockCursorManager.LockCursor = true;
		if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState == AvatarRuntimeState.Hidden)
		{
			ShowBriefing();
			Debug.Log("Briefing shown");
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
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
		level.View.Show();
		gameInfo.View.Show();
		resume.View.Show();
		bottomCenterToggles.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
		winningConditionDebriefingView.View.Hide();
		ClearGameMsg();
		gameMetersController.View.Hide();
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
	}

	public void ShowEUseIcon(ShowUseOption option = ShowUseOption.Normal, int woID = 0)
	{
		if (!showingIcon)
		{
			pressEToUsePrompt.Show(option, woID);
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

	public IGUICrossHair GetCrossHair()
	{
		return guiCrossHairLegacy;
	}

	public bool IsMenuShown()
	{
		return menu.View.isVisible;
	}

	private void ShowPlayersWindow(bool show)
	{
		menu.ShowOnShortcut(show);
	}

	public override void Initialize()
	{
		base.Initialize();
		GameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningCondition = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningCondition, new Action<IWinningCondition>(OnWinningConditionReceived));
		InitializeResumeButton();
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1 && MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			ShowTeamSelectDialog();
		}
	}

	public void ShowTeamSelectDialog()
	{
		Hide();
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/TeamSelect/TeamSelectDialog", string.Empty, noButtons: true, stackDialog: false, canClose: false).SetOnResultCallback(TeamSelectCallBack).Show();
		}
		else
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("Only one team in world."), TM._("Change Team")).Show();
		}
	}

	private void TeamSelectCallBack(UXDialogBox dialog)
	{
		Show();
		MVTeam team = (MVTeam)(int)dialog.GetResult();
		MVGameControllerBase.Game.SetTeam(team);
		FocusChanged(hasFocus: false);
	}

	private void ClearGameMsg()
	{
		winningConditionBriefingView.Clear();
	}

	public void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.Play || (MVGameControllerBase.GameMode == MVGameMode.Edit && MVGameControllerLegacyUI.EditorController.PlayInEditor))
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
			winningConditionDebriefingView.group.SetVisible(lockCursorManager.LockCursor);
		}
	}

	private void GenerateBriefing()
	{
		winningConditionBriefingView.Clear();
		List<IWinningCondition> winnerConditions = new List<IWinningCondition>();
		MVGameControllerBase.Game.WinningConditionManager.Traverse((IWinningCondition winnerCondition) =>
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
			lockCursorManager.LockCursor = false;
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
			if (list.Count > 0 && list[0].actorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr)
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
