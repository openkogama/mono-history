namespace UnityEngine.EventSystems;

public interface IOpenClientShopTab : IEventSystemHandler
{
	void OpenTab(UIPushOption options, int categoryId);
}
