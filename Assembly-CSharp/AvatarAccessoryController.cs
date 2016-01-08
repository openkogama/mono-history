using UnityEngine;

public class AvatarAccessoryController : MonoBehaviour
{
	public MVGUIAvatarAccessoryButtons AvatarAccessoryButtons;

	public MVGUIAvatarAccessoryShop AvatarAccessoryShop;

	public MVGUIAvatarAccessoryInventory AvatarAccessoryInventory;

	public MVGUIAvatarAccessoryShopButtonController AvatarAcessoryShopButton;

	public void OpenAvatarAccessoryInventory()
	{
		OpenAvatarAccessoryView(AvatarAccessoryInventory.View);
	}

	public void OpenAvatarAccessoryShop()
	{
		OpenAvatarAccessoryView(AvatarAccessoryShop.View);
	}

	private void OpenAvatarAccessoryView(UXView view)
	{
		Debug.Log("OpenAvatarAccessoryView");
		MVGameControllerLegacyUI.IngameController.ShowSingleWindow(view);
	}

	public void CloseAvatarAccessoryView()
	{
		MVGameControllerLegacyUI.IngameController.HideCurrentWindow();
	}
}
