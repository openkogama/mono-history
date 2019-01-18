using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPurchasePopup : MonoBehaviour
{
	[SerializeField]
	private Text cost;

	[SerializeField]
	private Text nonSubscriberCost;

	[SerializeField]
	private Text itemName;

	[SerializeField]
	private RawImage itemImage;

	[SerializeField]
	private Text description;

	[SerializeField]
	private ItemPurchaseConfirmationPopup confirmationPopup;

	private ShopItem item;

	public void Initialize(RawImage image, ShopItem item)
	{
		this.item = item;
		itemName.text = item.name;
		cost.text = item.priceGold.ToString();
		nonSubscriberCost.text = item.priceGold.ToString();
		itemImage.texture = image.texture;
		description.text = item.description;
	}

	public void OnPurchaseClicked()
	{
		if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.FreeBuildingGameObjects))
		{
			ItemPurchaseConfirmationPopup popup = Object.Instantiate(confirmationPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			popup.Initialize(ConfirmationCallback);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPurchaseClientShopItem x, BaseEventData y) =>
			{
				x.PurchaseItem(item);
			});
		}
	}

	private void ConfirmationCallback(bool result)
	{
		if (result)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPurchaseClientShopItem x, BaseEventData y) =>
			{
				x.PurchaseItem(item);
			});
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.Popup);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
	}
}
