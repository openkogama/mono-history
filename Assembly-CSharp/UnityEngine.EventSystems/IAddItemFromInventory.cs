namespace UnityEngine.EventSystems;

public interface IAddItemFromInventory : IEventSystemHandler
{
	void OnAddItemFromInventory(InventoryItem item);
}
