using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EditModeClientShopController : MonoBehaviour, IPurchaseClientShopItem, IOpenClientShop, IHighLightClientShopItem, IOpenClientShopTab, IOpenClientShopPage, IEventSystemHandler
{
	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private EditModeClientShopItem previewItemPrefab;

	private Transform previewRootTransform;

	private InventoryController inventoryController;

	private int selectedTab = 1;

	private readonly Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private readonly Dictionary<int, string> tabsNonLocalized = new Dictionary<int, string>();

	private readonly List<MVWorldObjectClient> previewedObjects = new List<MVWorldObjectClient>();

	private ClientShopRepository repository;

	private EditModeRepositoryController repositoryController;

	public void Initialize(EditModeRepositoryController repositoryController)
	{
		this.repositoryController = repositoryController;
		repository = MVGameControllerBase.EditModeUI.ClientShopRepository;
		selectedTab = 1;
		int num = 1;
		foreach (InventoryCategoryType key in repository.categories.Keys)
		{
			tabsNonLocalized[num] = repository.categories[key];
			tabs[num] = new TabState(TM._(repository.categories[key]), numberOfSlotsPrPage);
			tabs[num].highestSlotIndex = Math.Max(repository.CategoryItemCount(key), 1);
			num++;
		}
	}

	public void Activate(UIPushOption pushOption)
	{
		if (this.inventoryController != null)
		{
			return;
		}
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController.OnTabSelected, new UnityAction<int>(TabSelected));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController2.OnPageTurned, new UnityAction<int>(PageTurned));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		foreach (KeyValuePair<int, TabState> tab in tabs)
		{
			this.inventoryController.AddTab(tab.Key, tab.Value.name);
		}
		UpdateContent();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(this.inventoryController.gameObject, pushOption, OnPop, UIGroupFlags.InventoryUI);
		});
	}

	private void OnPop()
	{
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
		previewRootTransform = null;
		for (int i = 0; i < previewedObjects.Count; i++)
		{
			MVWorldObjectClient.DestroyRecursive(previewedObjects[i]);
		}
		previewedObjects.Clear();
	}

	private void TabSelected(int tab)
	{
		if (tab != selectedTab)
		{
			selectedTab = tab;
			UpdateContent();
		}
	}

	private void PageTurned(int dir)
	{
		if (tabs[selectedTab].UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	private void UpdateContent()
	{
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
		previewRootTransform = new GameObject("Preview Root - ClientShopInventory").transform;
		for (int i = 0; i < previewedObjects.Count; i++)
		{
			MVWorldObjectClient.DestroyRecursive(previewedObjects[i]);
		}
		previewedObjects.Clear();
		inventoryController.Clear();
		inventoryController.SelectTab(selectedTab, tabs[selectedTab].currentPage, tabs[selectedTab].MaxPages);
		List<ShopItem> itemsInCategorySlow = repository.GetItemsInCategorySlow(tabsNonLocalized[selectedTab]);
		for (int j = 0; j < itemsInCategorySlow.Count; j++)
		{
			if (tabs[selectedTab].SlotIndexIsInRange(itemsInCategorySlow[j].slotPosition))
			{
				EditModeClientShopItem editModeClientShopItem = UnityEngine.Object.Instantiate(previewItemPrefab);
				MVWorldObjectClient worldObjectFromItemData = GetWorldObjectFromItemData(itemsInCategorySlow[j]);
				previewedObjects.Add(worldObjectFromItemData);
				editModeClientShopItem.Initialize(previewRootTransform, itemsInCategorySlow[j], worldObjectFromItemData);
				inventoryController.AddObject(editModeClientShopItem.gameObject, itemsInCategorySlow[j].slotPosition % numberOfSlotsPrPage);
			}
		}
	}

	private static MVWorldObjectClient GetWorldObjectFromItemData(ShopItem item)
	{
		byte[] data = item.data;
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		return koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
	}

	public void PurchaseItem(ShopItem item)
	{
		repositoryController.PurchaseClientShopItem(item, UpdateContent);
	}

	public void HighlightAtCategoryWithSlot(UIPushOption options, int categoryId, int slotPosition)
	{
		Activate(options);
		string text = MVGameControllerBase.EditModeUI.PlayerInventoryRepository.categories[(InventoryCategoryType)categoryId];
		foreach (KeyValuePair<int, string> item in tabsNonLocalized)
		{
			if (text == item.Value)
			{
				selectedTab = item.Key;
				break;
			}
		}
		inventoryController.HighlightSlot(slotPosition % numberOfSlotsPrPage);
		UpdateContent();
	}

	public void OpenTab(UIPushOption options, int categoryId)
	{
		Activate(options);
		string text = MVGameControllerBase.EditModeUI.PlayerInventoryRepository.categories[(InventoryCategoryType)categoryId];
		foreach (KeyValuePair<int, string> item in tabsNonLocalized)
		{
			if (text == item.Value)
			{
				selectedTab = item.Key;
				break;
			}
		}
		int page = Mathf.CeilToInt(1f / (float)numberOfSlotsPrPage);
		tabs[selectedTab].SetPage(page);
		UpdateContent();
	}

	public void OpenPage(UIPushOption options, int categoryId, int slotPosition)
	{
		Activate(options);
		string text = MVGameControllerBase.EditModeUI.PlayerInventoryRepository.categories[(InventoryCategoryType)categoryId];
		foreach (KeyValuePair<int, string> item in tabsNonLocalized)
		{
			if (text == item.Value)
			{
				selectedTab = item.Key;
				break;
			}
		}
		int page = Mathf.CeilToInt(((float)slotPosition + 1f) / (float)numberOfSlotsPrPage);
		tabs[selectedTab].SetPage(page);
		UpdateContent();
	}
}
