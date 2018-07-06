namespace UnityEngine.EventSystems;

public interface IAccessoryInventoryControl : IEventSystemHandler
{
	void DisplayPurchasableItems(bool displayShopItems);

	void RefreshItems();
}
