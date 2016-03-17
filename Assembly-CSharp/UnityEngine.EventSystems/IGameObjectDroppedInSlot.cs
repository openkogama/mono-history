namespace UnityEngine.EventSystems;

public interface IGameObjectDroppedInSlot : IEventSystemHandler
{
	void SlotChanged(GameObject gameObject, int toSlotIndex);
}
