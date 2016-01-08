using UnityEngine;

public class PlayController : PlayControllerBase
{
	private AvatarAccessoryControllerPlayMode avatarAccessoryController;

	private GameCoinButtonController gameCoinsController;

	public override void Initialize()
	{
		base.Initialize();
		avatarAccessoryController.Initialize();
		gameCoinsController.Initialize();
	}

	public override void HandleInput()
	{
		base.HandleInput();
		avatarAccessoryController.HandleInput();
	}

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIPlayMode>().gameObject;
		avatarAccessoryController = AIngameController.FindGUIObjectOfType<AvatarAccessoryControllerPlayMode>(gameObject);
		gameCoinsController = AIngameController.FindGUIObjectOfType<GameCoinButtonController>(gameObject);
		bottomCenterToggles = AIngameController.FindGUIObjectOfType<MVGUIBottomCenterToggles>(gameObject);
	}

	protected override void Hide()
	{
		base.Hide();
		gameCoinsController.View.Hide();
		avatarAccessoryController.AvatarAcessoryShopButton.View.Hide();
		avatarAccessoryController.AvatarAccessoryButtons.View.Hide();
		avatarAccessoryController.CloseAvatarAccessoryView();
	}

	protected override void Show()
	{
		base.Show();
		avatarAccessoryController.AvatarAccessoryButtons.View.Show();
	}

	protected override void ShowLostFocusGUI()
	{
		base.ShowLostFocusGUI();
		avatarAccessoryController.AvatarAccessoryButtons.View.Show();
		chatController.ShowChat(takeControl: false, retainControlAfterMessageSend: true);
		chatController.CanAutoHide = false;
		avatarAccessoryController.AccessoryMoveOverride = true;
		gameCoinsController.View.Show();
		avatarAccessoryController.AvatarAcessoryShopButton.View.Show();
	}

	protected override void HideLostFocusGUI()
	{
		base.HideLostFocusGUI();
		avatarAccessoryController.AvatarAccessoryButtons.View.Hide();
		avatarAccessoryController.AccessoryMoveOverride = false;
		avatarAccessoryController.CloseAvatarAccessoryView();
		chatController.ShowChat(takeControl: false, retainControlAfterMessageSend: false);
		chatController.CanAutoHide = true;
		gameCoinsController.View.Hide();
		avatarAccessoryController.AvatarAcessoryShopButton.View.Hide();
	}
}
