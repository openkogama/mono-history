using System.Collections.Generic;
using MV.WorldObject;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableSpawnRolesInventoryHighlight : FirstTimeActivatableElementBase
{
	[SerializeField]
	private TabMenu tabGroup;

	[SerializeField]
	private InventorySlots slots;

	private bool showing;

	private InventoryCategoryType itemCategory = InventoryCategoryType.Blueprints;

	private bool haveCheckedItemAvailability;

	private bool itemAvailable;

	public override bool CanShow
	{
		get
		{
			if (!FirstTimeEventManager.HasFirstTimeEventOccured(PrerequisiteEvent))
			{
				return false;
			}
			bool flag = IsItemInShop(WorldObjectType.AvatarSpawnRoleCreator);
			if (!flag)
			{
				Register();
			}
			bool activeInHierarchy = gameObject.activeInHierarchy;
			bool flag2 = slots.transform.childCount > 0;
			return !IsBlocked && activeInHierarchy && flag2 && flag;
		}
	}

	public override void OnShow()
	{
		if (!showing)
		{
			DoShowing();
		}
	}

	private void DoShowing()
	{
		showing = true;
		ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IOpenClientShopTab x, BaseEventData y) =>
		{
			x.OpenTab(UIPushOption.Blocking, (int)itemCategory);
		});
		if (slots.transform.childCount > 0)
		{
			int itemSlot = GetItemSlot(0);
			ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IHighLightClientShopItem x, BaseEventData y) =>
			{
				x.HighlightAtCategoryWithSlot(UIPushOption.Blocking, (int)itemCategory, itemSlot);
			});
		}
	}

	protected int GetItemSlot(int iteration)
	{
		ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IOpenClientShopPage x, BaseEventData y) =>
		{
			x.OpenPage(UIPushOption.Blocking, (int)itemCategory, slots.SlotCountPerPage * iteration);
		});
		Dictionary<int, InventorySlot> dictionary = slots.GetSlots();
		if (dictionary == null || dictionary.Count == 0)
		{
			return 0;
		}
		foreach (InventorySlot value in dictionary.Values)
		{
			if (value.Item == null)
			{
				continue;
			}
			EditModeClientShopItem component = value.Item.GetComponent<EditModeClientShopItem>();
			if (!(component == null))
			{
				if (component.DocumentationType == MVWorldObjectDocumentationType.Missing)
				{
					Debug.LogWarning("item in inventory missing documentationtype, this may be uninitialized when accessed.");
				}
				else if (component.DocumentationType == MVWorldObjectDocumentationType.AvatarClass)
				{
					return value.AbsoluteSlot;
				}
			}
		}
		return GetItemSlot(iteration + 1);
	}

	private bool IsItemInShop(WorldObjectType worldObjectType)
	{
		if (!haveCheckedItemAvailability)
		{
			itemAvailable = CheckItemAvailability(worldObjectType);
			haveCheckedItemAvailability = true;
		}
		return itemAvailable;
	}

	private bool CheckItemAvailability(WorldObjectType worldObjectType)
	{
		foreach (InventoryCategoryType key in MVGameControllerBase.EditModeUI.ClientShopRepository.categories.Keys)
		{
			MVGameControllerBase.EditModeUI.ClientShopRepository.GetItemByWorldObjectTypeInCategory(key, worldObjectType, out var item);
			if (item != null)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnDestroy()
	{
		if (isRegistered)
		{
			if (FirstTimeEventManager.HasFirstTimeEventOccured(PrerequisiteEvent))
			{
				FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			}
			base.OnDestroy();
		}
	}

	protected override void OnFirstTimeState(FirstTimeState firstTimeState, FirstTimeEvent latestFirstTimeEvent)
	{
		if (!firstTimeState.HasFirstTimeEventOccured(firstTimeEvent) && !isRegistered)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.RegisterActivatableElement(this);
			});
			isRegistered = true;
		}
		else if (firstTimeState.HasFirstTimeEventOccured(firstTimeEvent) && FirstTimeEventManager.HasFirstTimeEventOccured(PrerequisiteEvent))
		{
			UnRegister();
			Object.Destroy(this);
		}
	}

	private void Register()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
		{
			x.RegisterActivatableElement(this);
		});
		isRegistered = true;
	}
}
