using System;

public class PlayControllerEdit : PlayController
{
	public Action<EditModeChangeArgs> EditModeChange;

	private bool playInEditor;

	private MVGUIPlayButton playButton;

	public void Initialize(MVGUIPlayButton playButton)
	{
		base.Initialize();
		this.playButton = playButton;
	}

	public void PlayFromEdit(bool playInEditor)
	{
		this.playInEditor = playInEditor;
		menu.playersWindow.UpdateTeamLists();
		if (!playInEditor)
		{
			Hide();
			LockCursorManager.LockCursor = false;
			MVInputWrapper.ignoreAllKeys = false;
			MVInputWrapper.ignoreInGameInput = false;
			chatController.CanAutoHide = true;
			MVGameController.WOCM.AvatarLocal.Body.AccessoryMoveOverride = false;
			if (MVGameController.Game.GameCoinManager.BoostEnabled)
			{
				MVGameController.Game.SetGameCoinBoostState(gameCoinBoosterEnabled: false);
			}
		}
		else
		{
			Show();
			LockCursorManager.LockCursor = true;
			briefingWasShown = false;
			ShowBriefing();
		}
		if (EditModeChange != null)
		{
			EditModeChange(new EditModeChangeArgs(playInEditor));
		}
	}

	protected override void FocusChanged(bool hasFocus)
	{
		if (playInEditor)
		{
			base.FocusChanged(hasFocus);
		}
	}

	protected override void ShowLostFocusGUI()
	{
		if (playInEditor)
		{
			base.ShowLostFocusGUI();
			playButton.View.Show();
		}
	}

	protected override void HideLostFocusGUI()
	{
		if (playInEditor)
		{
			base.HideLostFocusGUI();
			playButton.View.Hide();
		}
	}
}
