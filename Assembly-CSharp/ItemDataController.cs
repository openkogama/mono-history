using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemDataController : MonoBehaviour
{
	private readonly int defaultCategoryID = 1;

	private int currentTab = -1;

	private List<InventoryItemData> inventoryItemDatas = new List<InventoryItemData>();

	private Dictionary<int, TabState> categories = new Dictionary<int, TabState>();

	[SerializeField]
	private InventoryController inventoryController;

	[SerializeField]
	private PreviewObject previewObject;

	[SerializeField]
	private TestItem testItemPrefab;

	[SerializeField]
	private int numberOfSlots;

	private void Start()
	{
		this.inventoryController.Initialize(numberOfSlots);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController2.OnTabSelected, new UnityAction<int>(TabSelected));
		InventoryController inventoryController3 = this.inventoryController;
		inventoryController3.OnSlotChanged = (UnityAction<int, int>)Delegate.Combine(inventoryController3.OnSlotChanged, new UnityAction<int, int>(SlotChanged));
		InitializeTestData();
		TabSelected(defaultCategoryID);
	}

	public void SlotChanged(int fromSlotIndex, int toSlotIndex)
	{
		InventoryItemData itemData = GetItemData(fromSlotIndex);
		InventoryItemData itemData2 = GetItemData(toSlotIndex);
		itemData.slotIndex = toSlotIndex;
		if (itemData2 != null)
		{
			itemData2.slotIndex = fromSlotIndex;
		}
	}

	private void PageTurned(int dir)
	{
		TabState tabState = categories[currentTab];
		if (tabState.UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	private void TabSelected(int tabId)
	{
		if (tabId != currentTab)
		{
			currentTab = tabId;
			UpdateContent();
		}
	}

	private void UpdateContent()
	{
		TabState tabState = categories[currentTab];
		inventoryController.Clear();
		inventoryController.SelectTab(currentTab, tabState.currentPage, tabState.MaxPages);
		foreach (InventoryItemData inventoryItemData in inventoryItemDatas)
		{
			if (inventoryItemData.categoryId == currentTab && tabState.SlotIndexIsInRange(inventoryItemData.slotIndex))
			{
				TestItem testItem = UnityEngine.Object.Instantiate(testItemPrefab);
				testItem.Initialize(inventoryItemData.name, inventoryItemData.slotIndex);
				inventoryController.AddObject(testItem.gameObject, inventoryItemData.slotIndex % numberOfSlots);
			}
		}
	}

	private void InitializeTestData()
	{
		for (int i = 0; i < 20; i++)
		{
			int num = i % 3;
			InventoryItemData item = new InventoryItemData(i, i, num, $"ItemId: {i}. SlotIndex: {i}. Category: {num}.");
			inventoryItemDatas.Add(item);
			if (!categories.ContainsKey(num))
			{
				categories.Add(num, new TabState("Category " + num, numberOfSlots));
			}
			if (categories[num].highestSlotIndex < i)
			{
				categories[num].highestSlotIndex = i;
			}
		}
		foreach (KeyValuePair<int, TabState> category in categories)
		{
			inventoryController.AddTab(category.Key, category.Value.name);
		}
	}

	private InventoryItemData GetItemData(int slot)
	{
		foreach (InventoryItemData inventoryItemData in inventoryItemDatas)
		{
			if (inventoryItemData.slotIndex == slot)
			{
				return inventoryItemData;
			}
		}
		return null;
	}
}
