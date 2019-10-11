namespace UnityEngine.EventSystems;

public interface IOpenClientShopPage : IEventSystemHandler
{
	void OpenPage(UIPushOption options, int categoryId, int slotPosition);
}
