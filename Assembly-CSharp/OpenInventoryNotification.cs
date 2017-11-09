using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenInventoryNotification : Notification
{
	[SerializeField]
	private NotificationFade fader;

	private NotificationLifetime lifeTime;

	private int category;

	private int slot;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		lifeTime = (NotificationLifetime)(int)data[(byte)2];
		category = (int)data[(byte)13];
		slot = (int)data[(byte)14];
		fader.Activate();
	}

	public void NotificationClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.ActivateAtCategoryWithSlot(UIPushOption.Blocking, category, slot);
		});
		Close();
	}
}
