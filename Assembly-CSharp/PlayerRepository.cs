using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;

public class PlayerRepository
{
	public delegate void OnPlayerInventoryChangeDelegate(PlayerRepository playerRepository);

	public Dictionary<int, string> ItemTypes;

	public Dictionary<int, string> PlanetOwnershipTypes;

	private Dictionary<int, MVItem> playerInventory;

	public Hashtable itemIDToInventorySlotIndex;

	public OnPlayerInventoryChangeDelegate OnPlayerRepositoryChange;

	public IDictionary<int, MVItem> PlayerInventory => playerInventory;

	public PlayerRepository()
	{
		ItemTypes = new Dictionary<int, string>();
		PlanetOwnershipTypes = new Dictionary<int, string>();
		playerInventory = new Dictionary<int, MVItem>();
		itemIDToInventorySlotIndex = new Hashtable();
	}

	public void RemoveItem(int itemId)
	{
		playerInventory.Remove(itemId);
		itemIDToInventorySlotIndex.Remove(itemId);
		NotifyPlayerInventoryChange();
	}

	public void NotifyPlayerInventoryChange()
	{
		if (OnPlayerRepositoryChange != null)
		{
			OnPlayerRepositoryChange(this);
		}
	}
}
