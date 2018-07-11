using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AccessoryShopController : MonoBehaviour, IEventSystemHandler, IInventoryChanged, IAttachToBody, IAccessoryInventoryControl
{
	private InventoryController inventoryController;

	private Transform previewItemsRoot;

	private Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private int selectedTab;

	private AccessoryAttacher accessoryAttacher = new AccessoryAttacher();

	private int currentlyAttachingID;

	private bool attachingReady = true;

	private AccessoryCategoryClient startingCategory = AccessoryCategoryClient.Hats;

	private bool displayShopItems = true;

	[SerializeField]
	private AccessoryInventoryViewItem accessoryInventoryItemPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	private UIPushOption currentlyPushOption;

	private UIPushOption pushOption;

	private bool firstTimeSetup = true;

	private void Setup()
	{
		DisplayAllItems();
	}

	public void Activate(UIPushOption pushOption)
	{
		this.pushOption = pushOption;
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Combine(AccessoryDataManager.readyCallback, new UnityAction(ReadyCallback));
		AccessoryDataManager.SetReady();
	}

	public void Activate(UIPushOption pushOption, AccessoryCategoryClient category)
	{
		startingCategory = category;
		Activate(pushOption);
	}

	public void DisplayPurchasableItems(bool displayShopItems)
	{
		this.displayShopItems = displayShopItems;
		ClearShop();
		if (displayShopItems)
		{
			DisplayAllItems();
		}
		else
		{
			DisplayOwnedItems();
		}
		if (!tabs.ContainsKey(selectedTab))
		{
			selectedTab = (int)startingCategory;
		}
		tabs[selectedTab].SetPage(1);
		UpdateContent();
	}

	public void RefreshItems()
	{
		if (!tabs.ContainsKey(selectedTab))
		{
			TabSelected((int)startingCategory);
		}
		int currentPage = tabs[selectedTab].currentPage;
		ClearShop();
		if (displayShopItems)
		{
			DisplayAllItems();
		}
		else
		{
			DisplayOwnedItems();
		}
		if (!tabs.ContainsKey(selectedTab))
		{
			TabSelected((int)startingCategory);
		}
		tabs[selectedTab].SetPage(currentPage);
		UpdateContent();
	}

	private void ReadyCallback()
	{
		Debug.Log("ReadyCallback");
		selectedTab = (int)startingCategory;
		if (firstTimeSetup)
		{
			Setup();
			firstTimeSetup = false;
		}
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Remove(AccessoryDataManager.readyCallback, new UnityAction(ReadyCallback));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CEAvatarAccessoryUUI);
		});
		currentlyPushOption = pushOption;
		enabled = true;
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController2.OnTabSelected, new UnityAction<int>(TabSelected));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		displayShopItems = true;
		ClearShop();
		DisplayAllItems();
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
			x.Push(this.inventoryController.gameObject, pushOption, OnPop, UIGroupFlags.InventoryUI);
		});
		SetAccessoriesToSelectable(selectable: true);
		if (tabs.ContainsKey(255) && startingCategory != AccessoryCategoryClient.Bundles)
		{
			TabSelected(255);
		}
		else
		{
			UpdateContent();
		}
	}

	private void ClearShop()
	{
		tabs.Clear();
		inventoryController.Clear();
	}

	private void DisplayAllItems()
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> accessoriesCategoryMap = AccessoryDataManager.GetAccessoriesCategoryMap();
		foreach (KeyValuePair<AccessoryCategory, List<AccessoryDataClient>> item in accessoriesCategoryMap)
		{
			TabState tabState = new TabState(LocalizedEnums._((AccessoryCategoryClient)item.Key), numberOfSlotsPrPage);
			tabState.highestSlotIndex = item.Value.Count;
			tabs.Add((int)item.Key, tabState);
		}
		List<AccessoryDataClient> accessoryDataFromCategoryType = GetAccessoryDataFromCategoryType(AccessoryCategoryClient.Bundles);
		int count = accessoryDataFromCategoryType.Count;
		if (count > 0)
		{
			TabState tabState2 = new TabState(LocalizedEnums._(AccessoryCategoryClient.Bundles), numberOfSlotsPrPage);
			tabs.Add(254, tabState2);
			tabState2.highestSlotIndex = count;
		}
		List<AccessoryDataClient> accessoryDataFromCategoryType2 = GetAccessoryDataFromCategoryType(AccessoryCategoryClient.Featured);
		int count2 = accessoryDataFromCategoryType2.Count;
		if (count2 > 0)
		{
			TabState tabState3 = new TabState(LocalizedEnums._(AccessoryCategoryClient.Featured), numberOfSlotsPrPage);
			tabs.Add(255, tabState3);
			tabState3.highestSlotIndex = count2;
		}
	}

	private void DisplayOwnedItems()
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> accessoriesCategoryMap = AccessoryDataManager.GetAccessoriesCategoryMap();
		foreach (KeyValuePair<AccessoryCategory, List<AccessoryDataClient>> item in accessoriesCategoryMap)
		{
			TabState tabState = new TabState(LocalizedEnums._((AccessoryCategoryClient)item.Key), numberOfSlotsPrPage);
			int ownedAmount = GetOwnedAmount(item.Value);
			tabState.highestSlotIndex = ownedAmount;
			tabs.Add((int)item.Key, tabState);
		}
	}

	private int GetOwnedAmount(List<AccessoryDataClient> accessoryList)
	{
		int num = 0;
		for (int i = 0; i < accessoryList.Count; i++)
		{
			if (accessoryList[i].owns)
			{
				num++;
			}
		}
		return num;
	}

	private void SetAccessoriesToSelectable(bool selectable)
	{
		MVBody currentBody = null;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody((MVBody body) =>
				{
					currentBody = body;
				});
			});
		}
		else
		{
			currentBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
		}
		currentBody.AccessoryMoveOverride = selectable;
	}

	public void InventoryChanged()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		Activate(currentlyPushOption);
	}

	private void PageTurned(int dir)
	{
		if (tabs[selectedTab].UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	private void TabSelected(int tabId)
	{
		selectedTab = tabId;
		if (tabId == 255 || tabId == 254)
		{
			bool flag = displayShopItems;
			DisplayPurchasableItems(displayShopItems: true);
			displayShopItems = flag;
		}
		else
		{
			DisplayPurchasableItems(displayShopItems);
		}
		inventoryController.SetHeaderText(LocalizedEnums._((AccessoryCategoryClient)tabId));
	}

	private void OnPop()
	{
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Remove(AccessoryDataManager.readyCallback, new UnityAction(ReadyCallback));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
		if (previewItemsRoot != null)
		{
			UnityEngine.Object.Destroy(previewItemsRoot.gameObject);
		}
		previewItemsRoot = null;
		enabled = false;
		SetAccessoriesToSelectable(selectable: false);
	}

	private void UpdateContent()
	{
		if (previewItemsRoot != null)
		{
			UnityEngine.Object.Destroy(previewItemsRoot.gameObject);
		}
		previewItemsRoot = new GameObject("Preview Root - AccessoryShopController").transform;
		inventoryController.Clear();
		if (!tabs.ContainsKey(selectedTab))
		{
			selectedTab = (int)startingCategory;
		}
		TabState tabState = tabs[selectedTab];
		inventoryController.SelectTab(selectedTab, tabState.currentPage, tabState.MaxPages);
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(UpdateContentWithBody);
			});
		}
		else
		{
			UpdateContentWithBody(MVGameControllerBase.Game.LocalPlayer.Avatar.Body);
		}
	}

	private void UpdateContentWithBody(MVBody avatarBody)
	{
		List<AccessoryDataClient> accessoryDataFromCategoryType = GetAccessoryDataFromCategoryType((AccessoryCategoryClient)selectedTab);
		int num = tabs[selectedTab].SlotRange[0];
		for (int i = num; i < accessoryDataFromCategoryType.Count; i++)
		{
			AccessoryDataClient accessoryDataClient = accessoryDataFromCategoryType[i];
			if ((displayShopItems || accessoryDataClient.owns) && num < tabs[selectedTab].SlotRange[1] && accessoryDataClient.GetShowInShop())
			{
				AccessoryInventoryViewItem accessoryInventoryViewItem = UnityEngine.Object.Instantiate(accessoryInventoryItemPrefab);
				inventoryController.AddObject(accessoryInventoryViewItem.gameObject, num % numberOfSlotsPrPage);
				accessoryInventoryViewItem.Initialize(accessoryDataFromCategoryType[i], previewItemsRoot, avatarBody, selectedTab == 254);
				num++;
			}
		}
	}

	private List<AccessoryDataClient> GetAccessoryDataFromCategoryType(AccessoryCategoryClient category)
	{
		switch (category)
		{
		case AccessoryCategoryClient.Bundles:
		{
			List<AccessoryDataClient> list2 = new List<AccessoryDataClient>();
			List<AccessoryBundleItem> accessoryBundleItems = AccessoryDataManager.GetAccessoryBundleClient().accessoryBundleItems;
			for (int j = 0; j < accessoryBundleItems.Count; j++)
			{
				AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[j].accessoryMetaDataID);
				if (accessoryDataByMetaDataId != null && !accessoryDataByMetaDataId.owns)
				{
					list2.Add(accessoryDataByMetaDataId);
				}
			}
			return list2;
		}
		case AccessoryCategoryClient.Featured:
		{
			List<AccessoryDataClient> list = new List<AccessoryDataClient>();
			Dictionary<AccessoryCategory, List<AccessoryDataClient>> accessoriesCategoryMap = AccessoryDataManager.GetAccessoriesCategoryMap();
			{
				foreach (KeyValuePair<AccessoryCategory, List<AccessoryDataClient>> item in accessoriesCategoryMap)
				{
					for (int i = 0; i < item.Value.Count; i++)
					{
						AccessoryDataClient accessoryDataClient = item.Value[i];
						if (accessoryDataClient.isFeatured && !accessoryDataClient.owns && !list.Contains(accessoryDataClient))
						{
							list.Add(accessoryDataClient);
						}
					}
				}
				return list;
			}
		}
		default:
			return AccessoryDataManager.GetAccessoriesByCategoryId((AccessoryCategory)selectedTab);
		}
	}

	public void AttachToBody(int productId, float offset, float scale)
	{
		currentlyAttachingID = productId;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody((MVBody body) =>
				{
					Attach(body, offset, scale);
				});
			});
		}
		else
		{
			Attach(MVGameControllerBase.Game.LocalPlayer.Avatar.Body, offset, scale);
		}
	}

	private void Attach(MVBody body, float offset, float scale)
	{
		if (attachingReady)
		{
			attachingReady = false;
			accessoryAttacher.AttachAccessory(currentlyAttachingID, body, offset, scale, AttacherFinished);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
		}
	}

	private void AttacherFinished()
	{
		attachingReady = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(inventoryController.gameObject, null, (IAccessoryChanged x, BaseEventData y) =>
		{
			x.AccessoryChanged();
		});
		UpdateContent();
	}
}
