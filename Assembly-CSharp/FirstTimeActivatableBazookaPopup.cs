using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatableBazookaPopup : FirstTimeActivatableElementBase
{
	private Dictionary<MVWorldObjectDocumentationType, int> priorityDictionary = new Dictionary<MVWorldObjectDocumentationType, int>
	{
		{
			MVWorldObjectDocumentationType.Bazooka,
			10000
		},
		{
			MVWorldObjectDocumentationType.Centergun,
			500
		},
		{
			MVWorldObjectDocumentationType.DoubleSixShooter,
			300
		},
		{
			MVWorldObjectDocumentationType.ImpulseGun,
			200
		},
		{
			MVWorldObjectDocumentationType.Shotgun,
			100
		}
	};

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
			bool flag = slots.transform.childCount > 0;
			return !isBlocked && activeInHierarchy && flag;
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
		ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.OpenTab(UIPushOption.Blocking, 7);
		});
		if (slots.transform.childCount <= 0)
		{
			return;
		}
		InventoryItem prioritizedItem = null;
		int num = 0;
		Dictionary<int, InventorySlot> dictionary = slots.GetSlots();
		foreach (InventorySlot value in dictionary.Values)
		{
			PlayerInventoryPreviewItem component = value.Item.GetComponent<PlayerInventoryPreviewItem>();
			if (component.DocumentationType == MVWorldObjectDocumentationType.Missing)
			{
				Debug.LogWarning("item in inventory missing documentationtype, this may be uninitialized when accessed.");
				continue;
			}
			int num2 = 0;
			if (priorityDictionary.ContainsKey(component.DocumentationType))
			{
				num2 = priorityDictionary[component.DocumentationType];
			}
			if (num2 > num)
			{
				prioritizedItem = component.GetItem();
				num = num2;
			}
		}
		if (prioritizedItem == null)
		{
			Debug.LogError("prioritizedItem in first time player inventory was not found.");
			return;
		}
		ExecuteEvents.ExecuteHierarchy(tabGroup.gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.ActivateAtCategoryWithSlot(UIPushOption.Blocking, prioritizedItem.itemCategoryID, prioritizedItem.slotPosition);
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
