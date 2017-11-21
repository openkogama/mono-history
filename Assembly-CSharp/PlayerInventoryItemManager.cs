using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventoryItemManager : MonoBehaviour, IEventSystemHandler, ITabSelected
{
	private class ItemManagePageDef
	{
		public readonly TabMenuButton Button;

		public readonly ManageItemPage PageItemPrefab;

		public ItemManagePageDef(TabMenuButton button, ManageItemPage pageItemPrefab)
		{
			Button = button;
			PageItemPrefab = pageItemPrefab;
		}
	}

	[SerializeField]
	private TabMenuButton tabMenuButtonPrefab;

	[SerializeField]
	private RectTransform tabsRoot;

	[SerializeField]
	private RectTransform pageContentRoot;

	[SerializeField]
	private ItemInfoTab infoTabPrefab;

	[SerializeField]
	private InventoryItemPreviewSell itemSellTabPrefab;

	[SerializeField]
	private ItemInventoryDeleteTab ItemRemovalPrefab;

	private readonly List<ItemManagePageDef> tabList = new List<ItemManagePageDef>();

	private int currentTab;

	private InventoryItem previewedItem;

	private ManageItemPage currentManageItemPage;

	private RawImage itemImage;

	public void Initialize(InventoryItem item, RawImage itemPreview)
	{
		previewedItem = item;
		itemImage = itemPreview;
		AddTabMenuButton(1, "Info", infoTabPrefab);
		if (item.resellable && !item.isDefaultInvItem)
		{
			if (item.itemCategoryID != 1)
			{
				int num = MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.CountItemsWithOriginalID(item);
				if (num > 1)
				{
					AddTabMenuButton(2, "Sell", itemSellTabPrefab);
					AddTabMenuButton(3, "Remove", ItemRemovalPrefab);
				}
			}
			else
			{
				AddTabMenuButton(2, "Sell", itemSellTabPrefab);
				AddTabMenuButton(3, "Remove", ItemRemovalPrefab);
			}
		}
		foreach (ItemManagePageDef tab in tabList)
		{
			tab.Button.SetAsDeselected();
		}
		tabList[currentTab].Button.SetAsSelected();
		UpdateContent();
	}

	private void AddTabMenuButton(int categoryIndex, string categoryName, ManageItemPage pageItem)
	{
		TabMenuButton tabMenuButton = Object.Instantiate(tabMenuButtonPrefab);
		tabMenuButton.Initialize(categoryIndex, categoryName);
		tabMenuButton.transform.SetParent(tabsRoot, worldPositionStays: false);
		ItemManagePageDef item = new ItemManagePageDef(tabMenuButton, pageItem);
		tabList.Add(item);
	}

	public void TabSelected(int tabId)
	{
		int num = tabId - 1;
		if (currentTab == num)
		{
			return;
		}
		currentTab = num;
		foreach (ItemManagePageDef tab in tabList)
		{
			tab.Button.SetAsDeselected();
		}
		tabList[currentTab].Button.SetAsSelected();
		UpdateContent();
	}

	public void UpdateContent()
	{
		if (currentManageItemPage != null)
		{
			Object.Destroy(currentManageItemPage.gameObject);
		}
		currentManageItemPage = Object.Instantiate(tabList[currentTab].PageItemPrefab);
		currentManageItemPage.transform.SetParent(pageContentRoot, worldPositionStays: false);
		currentManageItemPage.Initialize(itemImage, previewedItem);
	}
}
