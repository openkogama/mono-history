public class PlayControllerEdit : PlayController
{
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
			MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = false;
			if (MVGameControllerBase.Game.GameCoinManager.BoostEnabled)
			{
				MVGameControllerBase.Game.SetGameCoinBoostState(gameCoinBoosterEnabled: false);
			}
		}
		else
		{
			Show();
			LockCursorManager.LockCursor = true;
			briefingWasShown = false;
			ShowBriefing();
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
