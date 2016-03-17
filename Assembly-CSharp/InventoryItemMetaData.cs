using UnityEngine;

public class InventoryItemMetaData : MonoBehaviour
{
	private int slotIndex;

	public int SlotIndex => slotIndex;

	public void Initialize(int slotIndex)
	{
		this.slotIndex = slotIndex;
	}
}
