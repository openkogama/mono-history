using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerInventoryController : MonoBehaviour, IPlayerInventory, IEventSystemHandler
{
	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private PlayerInventoryPreviewItem previewItemPrefab;

	private Transform previewRootTransform;

	private Transform tempPreviewRoot;

	private InventoryController inventoryController;

	private int selectedTab = 1;

	private readonly Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private readonly Dictionary<int, string> tabsNonLocalized = new Dictionary<int, string>();

	private readonly List<MVWorldObjectClient> previewedObjects = new List<MVWorldObjectClient>();

	private PlayerInventoryRepository repository;

	private List<InventoryItem> items = new List<InventoryItem>();

	private PlayerInventoryPreviewItem draggedPreviewItem;

	private MVWorldObjectClient worldObjectDataCopy;

	private InventoryItemPreviewer draggedPreview;

	public void Initialize()
	{
		repository = MVGameControllerBase.EditModeUI.PlayerInventoryRepository;
		selectedTab = 1;
		int num = 1;
		foreach (InventoryCategoryType key in repository.categories.Keys)
		{
			tabsNonLocalized[num] = repository.categories[key];
			tabs[num] = new TabState(TM._(repository.categories[key]), numberOfSlotsPrPage);
			tabs[num].highestSlotIndex = repository.HighestSlotIndex(key);
			num++;
		}
		tabs[1].highestSlotIndex += numberOfSlotsPrPage;
	}

	private void InventoryChanged()
	{
		UpdatePageCount();
		UpdateContent();
	}

	private void UpdatePageCount()
	{
		int num = 1;
		foreach (InventoryCategoryType key in repository.categories.Keys)
		{
			tabs[num].highestSlotIndex = repository.HighestSlotIndex(key) + numberOfSlotsPrPage;
			num++;
		}
	}

	public void Activate(UIPushOption options)
	{
		if (this.inventoryController != null)
		{
			return;
		}
		UpdatePageCount();
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController.OnTabSelected, new UnityAction<int>(TabSelected));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController2.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController3 = this.inventoryController;
		inventoryController3.OnSlotChanged = (UnityAction<int, int>)Delegate.Combine(inventoryController3.OnSlotChanged, new UnityAction<int, int>(SlotChanged));
		PlayerInventoryRepository playerInventoryRepository = repository;
		playerInventoryRepository.OnInventoryChanged = (Action)Delegate.Combine(playerInventoryRepository.OnInventoryChanged, new Action(InventoryChanged));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		foreach (KeyValuePair<int, TabState> tab in tabs)
		{
			this.inventoryController.AddTab(tab.Key, tab.Value.name);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(this.inventoryController.gameObject, options, OnPop, UIGroupFlags.InventoryUI);
		});
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

	public void ActivateAtCategoryWithSlot(UIPushOption options, int categoryId, int slotPosition)
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
		inventoryController.HighlightSlot(slotPosition % numberOfSlotsPrPage);
		UpdateContent();
	}

	private void OnPop()
	{
		if (tempPreviewRoot != null)
		{
			UnityEngine.Object.Destroy(tempPreviewRoot.gameObject);
		}
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
		PlayerInventoryRepository playerInventoryRepository = repository;
		playerInventoryRepository.OnInventoryChanged = (Action)Delegate.Remove(playerInventoryRepository.OnInventoryChanged, new Action(InventoryChanged));
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
			if (tempPreviewRoot != null)
			{
				UnityEngine.Object.Destroy(tempPreviewRoot.gameObject);
			}
			tempPreviewRoot = new GameObject("temp Root - PlayerInventory").transform;
			if (InventoryItemDragHandler.Dragging)
			{
				PreserveDraggedItemAcrossPages();
			}
			UpdateContent();
		}
	}

	private void PreserveDraggedItemAcrossPages()
	{
		worldObjectDataCopy = GetWorldObjectFromItemData(draggedPreviewItem.GetItem());
		draggedPreview = draggedPreviewItem.GetPreviewer();
		for (int i = 0; i < previewedObjects.Count; i++)
		{
			if (previewedObjects[i].GameObjectID == draggedPreview.PreviewGameObject.GetInstanceID())
			{
				previewedObjects[i].Transform.SetParent(tempPreviewRoot);
				draggedPreview.previewCam.transform.SetParent(tempPreviewRoot);
				MVWorldObjectClient.DestroyRecursive(worldObjectDataCopy);
				worldObjectDataCopy = previewedObjects[i];
				previewedObjects.RemoveAt(i);
				return;
			}
		}
		MVWorldObjectClient.DestroyRecursive(worldObjectDataCopy);
	}

	public void UpdateContent()
	{
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
		previewRootTransform = new GameObject("Preview Root - PlayerInventory").transform;
		for (int i = 0; i < previewedObjects.Count; i++)
		{
			MVWorldObjectClient.DestroyRecursive(previewedObjects[i]);
		}
		previewedObjects.Clear();
		if (InventoryItemDragHandler.Dragging)
		{
			previewedObjects.Add(worldObjectDataCopy);
			worldObjectDataCopy.Transform.SetParent(previewRootTransform);
			draggedPreview.previewCam.transform.SetParent(previewRootTransform);
		}
		inventoryController.Clear();
		inventoryController.SelectTab(selectedTab, tabs[selectedTab].currentPage, tabs[selectedTab].MaxPages);
		items = repository.GetItemsInCategorySlow(tabsNonLocalized[selectedTab]);
		for (int j = 0; j < items.Count; j++)
		{
			if (tabs[selectedTab].SlotIndexIsInRange(items[j].slotPosition))
			{
				if (draggedPreviewItem != null && items[j].itemID == draggedPreviewItem.GetItem().itemID)
				{
					Debug.Log("Drag preview item found");
					continue;
				}
				PlayerInventoryPreviewItem playerInventoryPreviewItem = UnityEngine.Object.Instantiate(previewItemPrefab);
				MVWorldObjectClient worldObjectFromItemData = GetWorldObjectFromItemData(items[j]);
				previewedObjects.Add(worldObjectFromItemData);
				playerInventoryPreviewItem.Initialize(previewRootTransform, items[j], worldObjectFromItemData, selectedTab == 1 && !items[j].isDefaultInvItem);
				inventoryController.AddObject(playerInventoryPreviewItem.gameObject, items[j].slotPosition % numberOfSlotsPrPage);
			}
		}
	}

	private void SlotChanged(int from, int to)
	{
		if (from == to)
		{
			return;
		}
		InventoryItem inventoryItem = null;
		InventoryItem inventoryItem2 = null;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].slotPosition == from)
			{
				inventoryItem = items[i];
			}
			else if (items[i].slotPosition == to)
			{
				inventoryItem2 = items[i];
			}
		}
		if (inventoryItem2 == null)
		{
			inventoryItem.slotPosition = to;
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add(inventoryItem.itemID, to);
			MVGameControllerBase.OperationRequests.UpdateInventorySlots(dictionary);
		}
		else if (inventoryItem2.isDefaultInvItem)
		{
			Debug.Log("destination.isDefaultInvItem");
			InventoryItemDragHandler.dragRejected = true;
		}
		else
		{
			repository.SwapItemSlotPositions(inventoryItem, inventoryItem2);
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			dictionary2.Add(inventoryItem.itemID, to);
			dictionary2.Add(inventoryItem2.itemID, from);
			MVGameControllerBase.OperationRequests.UpdateInventorySlots(dictionary2);
		}
	}

	private static MVWorldObjectClient GetWorldObjectFromItemData(InventoryItem item)
	{
		byte[] data = item.data;
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		return koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
	}

	public void SetCurrentDragTarget(GameObject draggingGameObject)
	{
		draggedPreviewItem = draggingGameObject.GetComponent<PlayerInventoryPreviewItem>();
	}

	public void DragFailed()
	{
		draggedPreviewItem = null;
		UpdateContent();
	}
}
