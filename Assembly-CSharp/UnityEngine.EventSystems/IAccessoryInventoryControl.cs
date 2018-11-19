namespace UnityEngine.EventSystems;

public interface IAccessoryInventoryControl : IEventSystemHandler
{
	void DisplayPurchasableItems(bool displayShopItems);

	void OpenInventoryAtItem(UIPushOption pushOption, AccessoryDataClient displayShopItems);

	void ResetAfterBundlePurchase();

	void RefreshItems();
}
