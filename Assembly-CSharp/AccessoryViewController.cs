using System.Collections.Generic;
using MV.WorldObject.Accessories;
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

	[SerializeField]
	private GameObject featuredTabFlare;

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
		previewer.OnRestartAnimation();
	}

	public void OpenCategoryScreen(bool canSortByInventory)
	{
		HideScreens();
		inventoryView.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(canSortByInventory);
		previewer.OnRestartAnimation();
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

	public void PurchasedBundle()
	{
		List<AccessoryBundleItem> accessoryBundleItems = AccessoryDataManager.GetAccessoryBundleClient().accessoryBundleItems;
		tabMenuAccessoryShop.DestroyTab(AccessoryCategoryClient.Bundles);
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[i].accessoryMetaDataID);
			if (accessoryDataByMetaDataId != null)
			{
				Debug.Log("data owned: " + accessoryDataByMetaDataId.name);
				AccessoryDataManager.SetToOwns(accessoryDataByMetaDataId.streamingAssetID);
			}
		}
		accessoryView.RefreshGoldAmount();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryInventoryControl x, BaseEventData y) =>
		{
			x.DisplayPurchasableItems(displayShopItems: true);
		});
	}

	public void DisplayFlare(bool show)
	{
		featuredTabFlare.SetActive(show);
	}
}
