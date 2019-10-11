using MV.WorldObject;
using UnityEngine.EventSystems;

public class FirstTimeActivatableTriggerAreaMessage : FirstTimeActivatableButtonPointer
{
	private bool haveCheckedItemAvailability;

	private bool itemAvailable;

	public override bool CanShow
	{
		get
		{
			bool flag = IsItemInShop(WorldObjectType.TriggerCube);
			if (!flag)
			{
				Register();
			}
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy && flag;
		}
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

	private void Register()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
		{
			x.RegisterActivatableElement(this);
		});
		isRegistered = true;
	}
}
