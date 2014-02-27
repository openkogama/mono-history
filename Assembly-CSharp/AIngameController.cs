using System;
using System.Collections.Generic;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class AIngameController
{
	private GameBriefingText briefingText;

	protected bool uiShown = true;

	protected ScoreManager scoreManager;

	protected MVGUIGameState gameState;

	protected MVGUIMenu menu;

	protected MVGUIPressEToUsePrompt pressEToUsePrompt;

	protected MVGUIChatWindowToggle chatWindowToggle;

	protected MVGUIGameMessages gameMessages;

	protected MVGUIChatWindow chatWindow;

	protected MVGUIGameInfo gameInfo;

	protected UXView currentView;

	private bool showingIcon;

	public MVNetworkGameStateListener GameStateListener => MVGameController.Instance.Game.NetworkGameStateListener;

	protected UXDialogFactory DialogFactory => UXUtils.FindGUIObjectOfType<UXDialogFactory>();

	public bool WindowShown => ((Object)(object)currentView != (Object)null && currentView.isVisible) || menu.View.isVisible;

	public AIngameController()
	{
		GameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
		MVRuntimeDataVariable collectibleCount = MVGameController.Instance.Game.LocalPlayer.Avatar.CollectibleCount;
		collectibleCount.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(collectibleCount.OnChange, (MVRuntimeDataVariable.OnChangeDelegate)((object collectibles) =>
		{
			ShowItemHUD();
		}));
	}

	public virtual void Initialize()
	{
		scoreManager = new ScoreManager();
		ResolveGUIElements();
		InitializeGUIElements();
		((Behaviour)UXUtils.FindGUIObjectOfType<MVGUIAdDaemon>()).enabled = false;
		((Behaviour)UXUtils.FindGUIObjectOfType<MVGUIAvatarAccessoryExpirationHandler>()).enabled = true;
		MVGameController.Instance.Game.CameraController.Init();
		MVGameController.Instance.TimeReward.Init();
		ShowItemHUD();
	}

	public virtual void Deinitialize()
	{
	}

	public virtual void Update()
	{
		SetGameMsg();
		MVInputWrapper.Update();
		AwayMonitor.Update();
		if (briefingText != null && !briefingText.Update())
		{
			briefingText = null;
		}
	}

	public virtual void HandleInput()
	{
		MVGameController.Instance.Game.CameraController.HandleInput();
	}

	public virtual void FixedUpdate()
	{
		MVGameController.Instance.Game.CameraController.UpdateCamera();
	}

	public virtual void LateUpdate()
	{
		MVGameController.Instance.Game.World.WorldInventory.LateUpdate();
	}

	protected virtual void ResolveGUIElements()
	{
		gameState = UXUtils.FindGUIObjectOfType<MVGUIGameState>();
		chatWindow = UXUtils.FindGUIObjectOfType<MVGUIChatWindow>();
		chatWindowToggle = UXUtils.FindGUIObjectOfType<MVGUIChatWindowToggle>();
		gameMessages = UXUtils.FindGUIObjectOfType<MVGUIGameMessages>();
		gameInfo = UXUtils.FindGUIObjectOfType<MVGUIGameInfo>();
		menu = UXUtils.FindGUIObjectOfType<MVGUIMenu>();
		pressEToUsePrompt = UXUtils.FindGUIObjectOfType<MVGUIPressEToUsePrompt>();
	}

	private void InitializeGUIElements()
	{
		InitializeSocialUI();
		gameState.View.Show();
		gameState.gameMsgs.Text = string.Empty;
	}

	private void InitializeSocialUI()
	{
		gameInfo.View.Show();
		gameMessages.View.Show();
		chatWindow.InitializeChat();
	}

	public void ShowEUseIcon()
	{
		if (!showingIcon)
		{
			pressEToUsePrompt.View.Show();
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

	public virtual void ToggleShowUI()
	{
		uiShown = !uiShown;
		if (uiShown)
		{
			gameMessages.View.Show();
			gameInfo.View.Show();
			return;
		}
		menu.View.Hide();
		gameInfo.View.Hide();
		HideChat();
		gameMessages.View.Hide();
	}

	public virtual void RemoveUI()
	{
		menu.View.Hide();
		gameInfo.View.Hide();
		HideChat();
		gameMessages.View.Hide();
	}

	public void ShowRegisterPopup()
	{
		DialogFactory.CreateDialog(TextSlotIndex.PleaseRegisterMessage, TextSlotIndex.RegisterHeadline).AddPositiveButton(TextSlotIndex.RegisterHeadline).AddNegativeButton(TextSlotIndex.Continue)
			.SetOnResultCallback(OnRegisterPopupResult)
			.Show();
	}

	private void OnRegisterPopupResult(UXDialogBox box)
	{
		if (box.DialogResult == UXDialogResult.Positive)
		{
			Application.ExternalCall("gotoRegisterForm", new object[1] { "play" });
		}
	}

	public virtual void ToggleMenu()
	{
		if (menu.View.isVisible)
		{
			menu.View.Hide();
		}
		else
		{
			menu.View.Show();
		}
	}

	public virtual void ShowPlayersWindow(bool show)
	{
		menu.ShowOnShortcut(show);
	}

	public virtual void ShowChat(bool fromShortcut)
	{
		chatWindow.ShowChat(fromShortcut);
		if (fromShortcut)
		{
			chatWindow.TakeFocus();
		}
	}

	public virtual void HideChat()
	{
		chatWindow.HideChat();
	}

	public bool IsChatShown()
	{
		return chatWindow.View.isVisible;
	}

	public bool IsMenuShown()
	{
		return menu.View.isVisible;
	}

	public bool ChatHasFocus()
	{
		return chatWindow.chatField.HasFocus;
	}

	public void ToggleFullScreen()
	{
		UXScreen uXScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
		uXScreen.Fullscreen = !uXScreen.Fullscreen;
	}

	public virtual void RespawnAvatar()
	{
		if (MVGameController.Instance.Game.IsPlaying)
		{
			MVGameController.Instance.WOCM.AvatarLocal.Suicide();
		}
		else
		{
			MVGameController.Instance.WOCM.AvatarLocal.Respawn();
		}
	}

	protected void ShowSingleWindow(UXView view)
	{
		bool flag = (Object)(object)currentView != (Object)null && currentView.isVisible;
		if (flag)
		{
			HideCurrentWindow();
		}
		if ((Object)(object)view != (Object)(object)currentView || !flag)
		{
			currentView = view;
			currentView.Show();
		}
		else
		{
			currentView = null;
		}
	}

	public void HideCurrentWindow()
	{
		currentView.Hide();
	}

	public void SetIgnoreKeyInput(bool ignore)
	{
		MVInputWrapper.ignoreAllKeys = ignore;
		MVGameController.Instance.Game.CameraController.IgnoreInput = ignore;
	}

	private void GameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			break;
		case MVGameStateType.Round:
			if (DialogFactory.CurrentDialogBox is MVGUIWinnerDialog)
			{
				DialogFactory.CloseDialog();
			}
			MVGameController.Instance.Game.LocalPlayer.Reset();
			ShowBriefing();
			ShowItemHUD();
			break;
		case MVGameStateType.RoundEnded:
			break;
		}
	}

	public virtual void OnWinnerReportReceived(WinnerReportBase report)
	{
		if (MVGameController.Instance.GameMode == MVGameMode.Play || (MVGameController.Instance.GameMode == MVGameMode.Edit && MVGameController.Instance.EditorController.PlayInEditor))
		{
			menu.View.Hide();
			if (IsChatShown())
			{
				HideChat();
			}
			if ((Object)(object)DialogFactory.CurrentDialogBox != (Object)null)
			{
				DialogFactory.CloseDialog();
			}
			DialogFactory.CreateCustomDialog("Prefabs/GUI/WinnerScreen/WinnerDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true, canClose: false);
			(DialogFactory.CurrentlyBuildingDialogBox as MVGUIWinnerDialog).BuildWinnerDialog(report);
			DialogFactory.Show();
		}
	}

	public virtual void SetGameMsg()
	{
		if (MVGameController.Instance.GameMode == MVGameMode.Play || (MVGameController.Instance.GameMode == MVGameMode.Edit && MVGameController.Instance.EditorController.PlayInEditor))
		{
			switch (GameStateListener.CurrentGameState)
			{
			case MVGameStateType.PrepareRound:
				break;
			case MVGameStateType.Round:
				if (GameStateListener.TimeLeftMS > 0)
				{
					int num = (int)((float)GameStateListener.TimeLeftMS / 1000f) + 1;
					gameState.gameMsgs.Text = $"{num / 60:00}:{num % 60:00}";
					if (!gameState.timeIcon.active)
					{
						gameState.timeIcon.SetActiveRecursively(true);
					}
				}
				else
				{
					gameState.gameMsgs.Text = string.Empty;
					if (gameState.timeIcon.active)
					{
						gameState.timeIcon.SetActiveRecursively(false);
					}
				}
				break;
			case MVGameStateType.TTRoundEnding:
				if (GameStateListener.TimeLeftMS > 0)
				{
					int num2 = (int)((float)GameStateListener.TimeLeftMS / 1000f) + 1;
					gameState.gameMsgs.Text = $"{num2 / 60:00}:{num2 % 60:00}";
					if (!gameState.timeIcon.active)
					{
						gameState.timeIcon.SetActiveRecursively(true);
					}
				}
				else
				{
					gameState.gameMsgs.Text = string.Empty;
					if (gameState.timeIcon.active)
					{
						gameState.timeIcon.SetActiveRecursively(false);
					}
				}
				break;
			case MVGameStateType.RoundEnded:
				gameState.gameMsgs.Text = string.Empty;
				if (gameState.timeIcon.active)
				{
					gameState.timeIcon.SetActiveRecursively(false);
				}
				break;
			}
		}
		else
		{
			gameState.gameMsgs.Text = string.Empty;
			if (gameState.timeIcon.active)
			{
				gameState.timeIcon.SetActiveRecursively(false);
			}
		}
	}

	public void ClearGameMsg()
	{
		gameState.gameMsgs.Text = string.Empty;
		gameState.briefingMsg.Text = string.Empty;
		if (briefingText != null)
		{
			briefingText.Reset();
			briefingText = null;
		}
		gameState.itemMsg.Text = string.Empty;
		if (gameState.timeIcon.active)
		{
			gameState.timeIcon.SetActiveRecursively(false);
		}
		if (gameState.itemIcon.active)
		{
			gameState.itemIcon.SetActiveRecursively(false);
		}
	}

	public void ShowBriefing()
	{
		if (MVGameController.Instance.GameMode == MVGameMode.Play || (MVGameController.Instance.GameMode == MVGameMode.Edit && MVGameController.Instance.EditorController.PlayInEditor))
		{
			List<MVWorldObjectClient> worldObjectsByType = MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.RoundCube);
			if (worldObjectsByType.Count > 0)
			{
				string text = string.Empty;
				switch ((MVWinningCondition)(int)worldObjectsByType[0].Data["winningCondition"])
				{
				case MVWinningCondition.FindAllCollectibles:
					text = Localization.Instance.GetText(TextSlotIndex.FindAllCollectiblesBriefing);
					break;
				case MVWinningCondition.HighestAltitude:
					text = Localization.Instance.GetText(TextSlotIndex.HighestAltitudeBriefing);
					break;
				case MVWinningCondition.LowestAltitude:
					text = Localization.Instance.GetText(TextSlotIndex.LowestAltitudeBriefing);
					break;
				case MVWinningCondition.MostKills:
					text = Localization.Instance.GetText(TextSlotIndex.MostKillsBriefing);
					break;
				case MVWinningCondition.ReachTheFlagFirst:
					text = Localization.Instance.GetText(TextSlotIndex.ReachTheFlagFirstBriefing);
					break;
				}
				briefingText = new GameBriefingText(gameState.briefingMsg, 6.5f, text);
			}
		}
		else
		{
			ClearGameMsg();
		}
	}

	public void ShowItemHUD()
	{
		if (MVGameController.Instance.GameMode != MVGameMode.Play && (MVGameController.Instance.GameMode != MVGameMode.Edit || !MVGameController.Instance.EditorController.PlayInEditor))
		{
			return;
		}
		int count = MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.CollectibleItem).Count;
		if (count > 0)
		{
			gameState.itemMsg.Text = string.Concat(MVGameController.Instance.Game.LocalPlayer.Avatar.CollectibleCount.Value, "/", count);
			if (!gameState.itemIcon.active)
			{
				gameState.itemIcon.SetActiveRecursively(true);
			}
		}
		else
		{
			gameState.itemMsg.Text = string.Empty;
			if (gameState.itemIcon.active)
			{
				gameState.itemIcon.SetActiveRecursively(false);
			}
		}
	}
}
