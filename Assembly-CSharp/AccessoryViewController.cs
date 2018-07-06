using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryViewController : MonoBehaviour, IEventSystemHandler, IAccessoryClicked, IBundleController
{
	[SerializeField]
	private AvatarAccessoryPreviewer previewer;

	[SerializeField]
	private AccessoryView accessoryView;

	[SerializeField]
	private GameObject inventoryView;

	[SerializeField]
	private BundleView bundlePurchaseOptions;

	[SerializeField]
	private AccessoryShopToggleInventory backbackController;

	[SerializeField]
	private TabMenuAccessoryShop tabMenuAccessoryShop;

	public void UpdateHighlightedTab(AccessoryCategoryClient category)
	{
		if (tabMenuAccessoryShop.GetTabMenuButton(category) is IHighlightedElement highlightedElement)
		{
			highlightedElement.UpdateHighlightState();
		}
	}

	public void OpenAccessoryManagementScreen(AccessoryDataClient accessoryData)
	{
		HideScreens();
		accessoryView.Initialize(accessoryData);
		accessoryView.gameObject.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(enable: false);
		previewer.OnAccessoryPreviewEnter();
	}

	public void OpenCategoryScreen(bool canSortByInventory)
	{
		HideScreens();
		inventoryView.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(canSortByInventory);
	}

	public void ShowBundle()
	{
		HideScreens();
		bundlePurchaseOptions.Initialize();
		bundlePurchaseOptions.gameObject.SetActive(value: true);
		inventoryView.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(enable: false);
	}

	private void HideScreens()
	{
		bundlePurchaseOptions.gameObject.SetActive(value: false);
		inventoryView.SetActive(value: false);
		accessoryView.gameObject.SetActive(value: false);
	}
}
