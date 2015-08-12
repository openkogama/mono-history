using UnityEngine;

public class AvatarAccessoryController : MonoBehaviour
{
	public MVGUIAvatarAccessoryButtons AvatarAccessoryButtons;

	public MVGUIAvatarAccessoryShop AvatarAccessoryShop;

	public MVGUIAvatarAccessoryInventory AvatarAccessoryInventory;

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
		MVGameController.IngameController.ShowSingleWindow(view);
	}

	public void CloseAvatarAccessoryView()
	{
		MVGameController.IngameController.HideCurrentWindow();
	}
}
