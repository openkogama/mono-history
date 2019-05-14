using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AccessoryShopController : MonoBehaviour, IInventoryChanged, IAttachToBody, IAccessoryInventoryControl, IEventSystemHandler
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

	private AccessoryViewController accessoryViewController;

	private UIPushOption currentlyPushOption;

	private UIPushOption pushOption;

	private AccessoryDataClient accessoryDataToShow;

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

	public void OpenInventoryAtItem(UIPushOption pushOption, AccessoryDataClient accessoryData)
	{
		accessoryDataToShow = accessoryData;
		this.pushOption = pushOption;
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Combine(AccessoryDataManager.readyCallback, new UnityAction(ReadyCallbackAccessoryView));
		AccessoryDataManager.SetReady();
	}

	public void Activate(UIPushOption pushOption, AccessoryCategoryClient category)
	{
		startingCategory = category;
		Activate(pushOption);
	}

	private void ReadyCallbackAccessoryView()
	{
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Remove(AccessoryDataManager.readyCallback, new UnityAction(ReadyCallbackAccessoryView));
		ReadyCallback();
		StartCoroutine(OpenAccessoryViewDelayed());
	}

	private IEnumerator OpenAccessoryViewDelayed()
	{
		yield return new WaitForEndOfFrame();
		accessoryViewController = inventoryController.GetComponent<AccessoryViewController>();
		if (accessoryViewController != null)
		{
			accessoryViewController.OpenAccessoryManagementScreen(accessoryDataToShow);
			accessoryDataToShow = null;
		}
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
		SetAccessoriesToSelectable(selectable: false);
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
		CreateAndAddTab(AccessoryCategoryClient.Hats, GetAccessoryDataFromCategoryType(AccessoryCategoryClient.Hats).Count);
		CreateAndAddTab(AccessoryCategoryClient.Particles, GetAccessoryDataFromCategoryType(AccessoryCategoryClient.Particles).Count);
		CreateAndAddTab(AccessoryCategoryClient.BackAccessories, GetAccessoryDataFromCategoryType(AccessoryCategoryClient.BackAccessories).Count);
		AddDynamicTab(AccessoryCategoryClient.Bundles);
		AddDynamicTab(AccessoryCategoryClient.Featured);
		CreateAndAddTab(AccessoryCategoryClient.LevelUnlocks, GetAccessoryDataFromCategoryType(AccessoryCategoryClient.LevelUnlocks).Count);
	}

	private void AddDynamicTab(AccessoryCategoryClient category)
	{
		List<AccessoryDataClient> accessoryDataFromCategoryType = GetAccessoryDataFromCategoryType(category);
		int count = accessoryDataFromCategoryType.Count;
		if (count > 0)
		{
			CreateAndAddTab(category, count);
		}
	}

	private void CreateAndAddTab(AccessoryCategoryClient category, int highestSlotIndex)
	{
		TabState tabState = new TabState(LocalizedEnums._(category), numberOfSlotsPrPage);
		tabs.Add((int)category, tabState);
		tabState.highestSlotIndex = highestSlotIndex;
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

	public void ResetAfterBundlePurchase()
	{
		startingCategory = AccessoryCategoryClient.Hats;
		TabSelected((int)startingCategory);
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
		if (!displayShopItems)
		{
			for (int num = accessoryDataFromCategoryType.Count - 1; num >= 0; num--)
			{
				AccessoryDataClient accessoryDataClient = accessoryDataFromCategoryType[num];
				if (!accessoryDataClient.owns)
				{
					accessoryDataFromCategoryType.RemoveAt(num);
				}
			}
		}
		int num2 = tabs[selectedTab].SlotRange[0];
		for (int i = num2; i < accessoryDataFromCategoryType.Count; i++)
		{
			AccessoryDataClient accessoryDataClient2 = accessoryDataFromCategoryType[i];
			if (num2 < tabs[selectedTab].SlotRange[1] && (accessoryDataClient2.GetShowInShop() || accessoryDataClient2.owns))
			{
				AccessoryInventoryViewItem accessoryInventoryViewItem = UnityEngine.Object.Instantiate(accessoryInventoryItemPrefab);
				inventoryController.AddObject(accessoryInventoryViewItem.gameObject, num2 % numberOfSlotsPrPage);
				accessoryInventoryViewItem.Initialize(accessoryDataFromCategoryType[i], previewItemsRoot, avatarBody, selectedTab == 254);
				num2++;
			}
		}
	}

	private List<AccessoryDataClient> GetAccessoryDataFromCategoryType(AccessoryCategoryClient category)
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> accessoriesCategoryMap = AccessoryDataManager.GetAccessoriesCategoryMap();
		switch (category)
		{
		case AccessoryCategoryClient.Bundles:
		{
			List<AccessoryDataClient> list3 = new List<AccessoryDataClient>();
			List<AccessoryBundleItem> accessoryBundleItems = AccessoryDataManager.AccessoryBundleClient.accessoryBundleItems;
			for (int num4 = 0; num4 < accessoryBundleItems.Count; num4++)
			{
				AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[num4].accessoryMetaDataID);
				if (accessoryDataByMetaDataId != null && !accessoryDataByMetaDataId.owns)
				{
					list3.Add(accessoryDataByMetaDataId);
				}
			}
			return list3.OrderBy((AccessoryDataClient o) => o.DiscountedPrice).ToList();
		}
		case AccessoryCategoryClient.Featured:
		{
			List<AccessoryDataClient> list = new List<AccessoryDataClient>();
			foreach (KeyValuePair<AccessoryCategory, List<AccessoryDataClient>> item in accessoriesCategoryMap)
			{
				for (int num2 = 0; num2 < item.Value.Count; num2++)
				{
					AccessoryDataClient accessoryDataClient = item.Value[num2];
					if (accessoryDataClient.iFtr && !accessoryDataClient.owns && !list.Contains(accessoryDataClient))
					{
						list.Add(accessoryDataClient);
					}
				}
			}
			return list.OrderBy((AccessoryDataClient o) => o.DiscountedPrice).ToList();
		}
		case AccessoryCategoryClient.LevelUnlocks:
		{
			List<AccessoryDataClient> list2 = new List<AccessoryDataClient>();
			foreach (KeyValuePair<AccessoryCategory, List<AccessoryDataClient>> item2 in accessoriesCategoryMap)
			{
				for (int num3 = 0; num3 < item2.Value.Count; num3++)
				{
					AccessoryDataClient accessoryDataClient2 = item2.Value[num3];
					if ((accessoryDataClient2.dsc >= 100 || accessoryDataClient2.cost == 0) && accessoryDataClient2.lvl != 0 && !list2.Contains(accessoryDataClient2))
					{
						list2.Add(accessoryDataClient2);
					}
				}
			}
			return list2.OrderBy((AccessoryDataClient o) => o.lvl).ToList();
		}
		default:
		{
			List<AccessoryDataClient> accessoriesByCategoryId = AccessoryDataManager.GetAccessoriesByCategoryId((AccessoryCategory)selectedTab);
			for (int num = accessoriesByCategoryId.Count - 1; num >= 0; num--)
			{
				if (accessoriesByCategoryId[num].DiscountedPrice == 0 && !accessoriesByCategoryId[num].owns)
				{
					accessoriesByCategoryId.RemoveAt(num);
				}
			}
			return accessoriesByCategoryId.OrderBy((AccessoryDataClient o) => o.DiscountedPrice).ToList();
		}
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
