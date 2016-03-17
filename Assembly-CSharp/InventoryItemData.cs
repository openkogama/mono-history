public class InventoryItemData
{
	public readonly int id;

	public readonly string name;

	public readonly int categoryId;

	public int slotIndex;

	public InventoryItemData(int id, int slotIndex, int categoryId, string name)
	{
		this.slotIndex = slotIndex;
		this.categoryId = categoryId;
		this.name = name;
		this.id = id;
	}

	public override string ToString()
	{
		return name;
	}
}
