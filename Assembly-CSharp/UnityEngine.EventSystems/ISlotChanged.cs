namespace UnityEngine.EventSystems;

public interface ISlotChanged : IEventSystemHandler
{
	void SlotChanged(int fromSlotIndex, int toSlotIndex);
}
