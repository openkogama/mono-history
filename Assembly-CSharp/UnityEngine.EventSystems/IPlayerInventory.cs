namespace UnityEngine.EventSystems;

public interface IPlayerInventory : IEventSystemHandler
{
	void Activate(UIPushOption options);

	void UpdateContent();

	void SetCurrentDragTarget(GameObject draggingGameObject);

	void DragFailed();
}
