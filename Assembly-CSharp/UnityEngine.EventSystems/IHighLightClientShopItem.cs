namespace UnityEngine.EventSystems;

public interface IHighLightClientShopItem : IEventSystemHandler
{
	void HighlightAtCategoryWithSlot(UIPushOption options, int categoryId, int slotPosition);
}
