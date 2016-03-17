namespace UnityEngine.EventSystems;

public interface IAvatarSlotClicked : IEventSystemHandler
{
	void AvatarSlotClicked(int slotIndex);
}
