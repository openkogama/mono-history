namespace UnityEngine.EventSystems;

public interface IAccessoryPopupHandler : IEventSystemHandler
{
	void OpenInventoryAtItem(UIPushOption pushOption, AccessoryDataClient displayShopItems);
}
