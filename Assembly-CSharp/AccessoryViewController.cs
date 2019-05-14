using System.Collections.Generic;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryViewController : MonoBehaviour, IAccessoryClicked, IBundleController, IEventSystemHandler
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

	[SerializeField]
	private LevelProgressAccessoryShop levelProgress;

	private Color prevLight;

	private bool wasEnabled;

	private void Start()
	{
		WorldObjectClientRef<ThemeWorldObject> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<ThemeWorldObject>();
		if (singletonWorldObjectRef != null && singletonWorldObjectRef.WorldObjectClient != null)
		{
			singletonWorldObjectRef.WorldObjectClient.Visualization.Deactivate();
		}
		prevLight = RenderSettings.ambientLight;
		wasEnabled = MVGameControllerBase.SkyboxManager.enabled;
		MVGameControllerBase.SkyboxManager.enabled = false;
		float num = 0.55f;
		RenderSettings.ambientLight = Color.white * num;
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			RenderSettings.ambientLight = prevLight;
			MVGameControllerBase.SkyboxManager.enabled = wasEnabled;
			WorldObjectClientRef<ThemeWorldObject> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<ThemeWorldObject>();
			if (singletonWorldObjectRef != null && singletonWorldObjectRef.WorldObjectClient != null)
			{
				singletonWorldObjectRef.WorldObjectClient.Visualization.Activate();
			}
		}
	}

	public void UpdateHighlightedTab(AccessoryCategoryClient category)
	{
		if (tabMenuAccessoryShop.GetTabMenuButton(category) is IHighlightedElement highlightedElement)
		{
			highlightedElement.UpdateHighlightState();
		}
	}

	public void OpenAccessoryManagementScreen(AccessoryDataClient accessoryData)
	{
		if (!accessoryView.CurrentlyViewingAccessory(accessoryData))
		{
			HideScreens();
			accessoryView.Initialize(accessoryData);
			accessoryView.gameObject.SetActive(value: true);
			backbackController.SetBackpackIconIsEnabled(enable: false);
			previewer.OnRestartAnimation();
			previewer.ResetPreviewTransform();
		}
	}

	public void OpenCategoryScreen(bool canSortByInventory)
	{
		HideScreens();
		inventoryView.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(canSortByInventory);
		previewer.OnRestartAnimation();
		previewer.ResetPreviewTransform();
	}

	public void ShowBundle()
	{
		HideScreens();
		bundlePurchaseOptions.Initialize();
		bundlePurchaseOptions.gameObject.SetActive(value: true);
		inventoryView.SetActive(value: true);
		backbackController.SetBackpackIconIsEnabled(enable: false);
		previewer.ResetPreviewTransform();
	}

	private void HideScreens()
	{
		bundlePurchaseOptions.gameObject.SetActive(value: false);
		inventoryView.SetActive(value: false);
		accessoryView.gameObject.SetActive(value: false);
	}

	public void PurchasedBundle()
	{
		List<AccessoryBundleItem> accessoryBundleItems = AccessoryDataManager.AccessoryBundleClient.accessoryBundleItems;
		tabMenuAccessoryShop.DestroyTab(AccessoryCategoryClient.Bundles);
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[i].accessoryMetaDataID);
			if (accessoryDataByMetaDataId != null)
			{
				AccessoryDataManager.SetToOwns(accessoryDataByMetaDataId.sAID);
			}
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryInventoryControl x, BaseEventData y) =>
		{
			x.ResetAfterBundlePurchase();
		});
	}

	public void DisplayCategoryFeatures(AccessoryCategoryClient category)
	{
		featuredTabFlare.SetActive(category == AccessoryCategoryClient.Featured || category == AccessoryCategoryClient.Bundles);
		levelProgress.gameObject.SetActive(category == AccessoryCategoryClient.LevelUnlocks);
		TabMenuButtonBase tabMenuButton = tabMenuAccessoryShop.GetTabMenuButton(AccessoryCategoryClient.Bundles);
		if (tabMenuButton != null)
		{
			tabMenuButton.gameObject.SetActive(category != AccessoryCategoryClient.LevelUnlocks);
		}
	}
}
