using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPurchasePopup : MonoBehaviour
{
	[SerializeField]
	private Text cost;

	[SerializeField]
	private Text itemName;

	[SerializeField]
	private RawImage itemImage;

	private ShopItem item;

	public void Initialize(RawImage image, ShopItem item)
	{
		this.item = item;
		itemName.text = item.name;
		cost.text = item.priceGold.ToString();
		itemImage.texture = image.texture;
	}

	public void OnPurchaseClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPurchaseClientShopItem x, BaseEventData y) =>
		{
			x.PurchaseItem(item);
		});
	}
}
