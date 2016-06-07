using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AccessoryShopController : MonoBehaviour, IEventSystemHandler, IInventoryChanged, IAttachToBody
{
	private class AccessoryData
	{
		public readonly StreamingAssetInfo streamingAssetInfo;

		public readonly ProductInventoryInfo productInventoryInfo;

		public readonly int slotIndex;

		public AccessoryData(StreamingAssetInfo streamingAssetInfo, ProductInventoryInfo productInventoryInfo, int slotIndex)
		{
			this.streamingAssetInfo = streamingAssetInfo;
			this.productInventoryInfo = productInventoryInfo;
			this.slotIndex = slotIndex;
		}
	}

	private InventoryController inventoryController;

	private Transform previewItemsRoot;

	private List<AccessoryData> slotToStreamingAssetInfoMap = new List<AccessoryData>();

	private Dictionary<int, TabState> tabs = new Dictionary<int, TabState>();

	private int selectedTab;

	private AccessoryAttacher accessoryAttacher = new AccessoryAttacher();

	private AccessoryMover accessoryMover = new AccessoryMover();

	private bool usingAccessoryMoverAndroid;

	private int currentlyAttachingID;

	private bool attachingReady = true;

	private int slotIndexOffset;

	private static Dictionary<int, AvatarAccessorySlot> categoryAvatarAccessorySlotMap = new Dictionary<int, AvatarAccessorySlot>
	{
		{
			1,
			AvatarAccessorySlot.Head
		},
		{
			2,
			AvatarAccessorySlot.Torso
		}
	};

	[SerializeField]
	private AccessoryInventoryViewItem accessoryInventoryItemPrefab;

	[SerializeField]
	private AccessoryUnEquip accessoryUnEquipPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	private UIPushOption currentlyPushOption;

	public void Awake()
	{
	}

	public void Initialize()
	{
		List<StreamingAssetInfo> list = MVGameControllerBase.Game.StreamingAssetShopInventory.Get(StreamingAssetType.AvatarAccessory).ToList();
		foreach (StreamingAssetInfo item in list)
		{
			if (!tabs.ContainsKey(item.CategoryID))
			{
				TabState value = new TabState(TM._(item.CategoryName), numberOfSlotsPrPage);
				tabs.Add(item.CategoryID, value);
			}
			tabs[item.CategoryID].highestSlotIndex++;
		}
		selectedTab = 1;
	}

	public void Activate(UIPushOption pushOption)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CEAvatarAccessoryUUI);
		});
		currentlyPushOption = pushOption;
		slotToStreamingAssetInfoMap.Clear();
		List<ProductInventoryInfo> productInventoryInfos = MVGameControllerBase.Game.StreamingAssetInventory.Get(StreamingAssetType.AvatarAccessory).ToList();
		List<StreamingAssetInfo> list = MVGameControllerBase.Game.StreamingAssetShopInventory.Get(StreamingAssetType.AvatarAccessory).ToList();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int num = 0; num < list.Count; num++)
		{
			TryGetProductInventoryInfo(out var productInventoryInfo, list[num].ProductID, productInventoryInfos);
			if (!dictionary.ContainsKey(list[num].CategoryID))
			{
				dictionary.Add(list[num].CategoryID, 0);
			}
			List<AccessoryData> list2 = slotToStreamingAssetInfoMap;
			StreamingAssetInfo streamingAssetInfo = list[num];
			ProductInventoryInfo productInventoryInfo2 = productInventoryInfo;
			Dictionary<int, int> dictionary3;
			Dictionary<int, int> dictionary2 = (dictionary3 = dictionary);
			int categoryID;
			int key = (categoryID = list[num].CategoryID);
			categoryID = dictionary3[categoryID];
			categoryID = (dictionary2[key] = categoryID + 1);
			list2.Add(new AccessoryData(streamingAssetInfo, productInventoryInfo2, categoryID));
		}
		enabled = true;
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		InventoryController inventoryController2 = this.inventoryController;
		inventoryController2.OnTabSelected = (UnityAction<int>)Delegate.Combine(inventoryController2.OnTabSelected, new UnityAction<int>(TabSelected));
		this.inventoryController.gameObject.AddComponent<AccessoryShopPreview>();
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		if (!usingAccessoryMoverAndroid)
		{
			accessoryMover.Activate();
		}
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
		UpdateContent();
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
		if (tabId != selectedTab)
		{
			selectedTab = tabId;
			UpdateContent();
		}
	}

	private void OnPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
		UnityEngine.Object.Destroy(previewItemsRoot.gameObject);
		previewItemsRoot = null;
		enabled = false;
		if (!usingAccessoryMoverAndroid)
		{
			accessoryMover.Destroy();
		}
	}

	private void UpdateContent()
	{
		if (previewItemsRoot != null)
		{
			UnityEngine.Object.Destroy(previewItemsRoot.gameObject);
		}
		previewItemsRoot = null;
		inventoryController.Clear();
		previewItemsRoot = new GameObject("Preview Root - AccessoryShopController").transform;
		TabState tabState = tabs[selectedTab];
		inventoryController.SelectTab(selectedTab, tabState.currentPage, tabState.MaxPages);
		slotIndexOffset = 0;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(CreateAccessoryUnequip);
			});
		}
		else
		{
			CreateAccessoryUnequip(MVGameControllerBase.WOCM.AvatarLocal.Body);
		}
		foreach (AccessoryData item in slotToStreamingAssetInfoMap)
		{
			if (item.streamingAssetInfo.CategoryID == selectedTab && tabState.SlotIndexIsInRange(item.slotIndex - slotIndexOffset))
			{
				AccessoryInventoryViewItem accessoryInventoryViewItem = UnityEngine.Object.Instantiate(accessoryInventoryItemPrefab);
				inventoryController.AddObject(accessoryInventoryViewItem.gameObject, (item.slotIndex - slotIndexOffset) % numberOfSlotsPrPage);
				accessoryInventoryViewItem.Initialize(item.streamingAssetInfo, item.productInventoryInfo, previewItemsRoot);
			}
		}
	}

	private void CreateAccessoryUnequip(MVBody body)
	{
		int accessoryID = body.GetAccessoryID(categoryAvatarAccessorySlotMap[selectedTab]);
		if (tabs[selectedTab].currentPage != 1)
		{
			if (accessoryID == -1)
			{
				slotIndexOffset = 1;
			}
		}
		else if (accessoryID == -1)
		{
			slotIndexOffset = 1;
		}
		else
		{
			AccessoryUnEquip accessoryUnEquip = UnityEngine.Object.Instantiate(accessoryUnEquipPrefab);
			inventoryController.AddObject(accessoryUnEquip.gameObject, 0);
			accessoryUnEquip.Initialize(categoryAvatarAccessorySlotMap[selectedTab], body);
			accessoryUnEquip.OnUnequipFinished = (UnityAction)Delegate.Combine(accessoryUnEquip.OnUnequipFinished, new UnityAction(UpdateContent));
		}
	}

	private bool TryGetProductInventoryInfo(out ProductInventoryInfo productInventoryInfo, int productID, List<ProductInventoryInfo> productInventoryInfos)
	{
		foreach (ProductInventoryInfo productInventoryInfo2 in productInventoryInfos)
		{
			if (productInventoryInfo2.ProductInfo.ProductID == productID)
			{
				productInventoryInfo = productInventoryInfo2;
				return true;
			}
		}
		productInventoryInfo = null;
		return false;
	}

	public void AttachToBody(int productId)
	{
		currentlyAttachingID = productId;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(Attach);
			});
		}
		else
		{
			Attach(MVGameControllerBase.Game.LocalPlayer.Avatar.Body);
		}
	}

	private void Attach(MVBody body)
	{
		if (attachingReady)
		{
			attachingReady = false;
			accessoryAttacher.AttachAccessory(currentlyAttachingID, body, AttacherFinished);
			UpdateContent();
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
	}

	private void Update()
	{
		if (!usingAccessoryMoverAndroid)
		{
			accessoryMover.MoveAccessory();
		}
	}
}
