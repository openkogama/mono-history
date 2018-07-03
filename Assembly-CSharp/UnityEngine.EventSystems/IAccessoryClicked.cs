namespace UnityEngine.EventSystems;

public interface IAccessoryClicked : IEventSystemHandler
{
	void OpenAccessoryManagementScreen(AccessoryDataClient accessoryData);

	void OpenCategoryScreen(bool canSortByInventory);

	void UpdateHighlightedTab(AccessoryCategoryClient category);
}
