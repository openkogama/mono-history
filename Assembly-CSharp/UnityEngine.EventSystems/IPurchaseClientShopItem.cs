namespace UnityEngine.EventSystems;

public interface IPurchaseClientShopItem : IEventSystemHandler
{
	void PurchaseItem(ShopItem item);
}
