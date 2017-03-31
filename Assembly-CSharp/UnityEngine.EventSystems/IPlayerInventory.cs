namespace UnityEngine.EventSystems;

public interface IPlayerInventory : IEventSystemHandler
{
	void Activate(UIPushOption options);

	void ActivateAtCategoryWithSlot(UIPushOption options, int categoryId, int slotPosition);

	void UpdateContent();

	void SetCurrentDragTarget(GameObject draggingGameObject);

	void DragFailed();
}
