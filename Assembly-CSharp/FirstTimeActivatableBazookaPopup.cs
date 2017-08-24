using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableBazookaPopup : FirstTimeActivatableElementBase
{
	[SerializeField]
	private TabMenu tabGroup;

	[SerializeField]
	private InventorySlots slots;

	private bool showing;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
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
		PlayerInventoryRepository playerInventoryRepository = MVGameControllerBase.IEditModeUI.PlayerInventoryRepository;
		string category = playerInventoryRepository.categories[7];
		List<InventoryItem> itemsInCategory = playerInventoryRepository.GetItemsInCategory(category);
		InventoryItem bazooka = null;
		for (int i = 0; i < itemsInCategory.Count; i++)
		{
			if (itemsInCategory[i].name == InventoryItem.localItemDescriptionOverride[MVWorldObjectDocumentationType.Bazooka].Name)
			{
				bazooka = itemsInCategory[i];
				break;
			}
		}
		ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.ActivateAtCategoryWithSlot(UIPushOption.Blocking, bazooka.itemCategoryID, bazooka.slotPosition);
		});
	}

	protected override void OnDestroy()
	{
		if (isRegistered)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			base.OnDestroy();
		}
	}
}
