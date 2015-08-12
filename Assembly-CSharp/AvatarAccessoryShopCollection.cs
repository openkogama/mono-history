using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class AvatarAccessoryShopCollection : IUXCollection
{
	private StreamingAssetShopInventory streamingAssetShopInventory;

	private IUXCollectionItem[] cache = new IUXCollectionItem[0];

	public OnCollectionChangeDelegate OnCollectionChange { get; set; }

	public int Count => GetAvatarAccessories().Count;

	public AvatarAccessoryShopCollection(StreamingAssetShopInventory streamingAssetShopInventory)
	{
		this.streamingAssetShopInventory = streamingAssetShopInventory;
		StreamingAssetShopInventory streamingAssetShopInventory2 = this.streamingAssetShopInventory;
		streamingAssetShopInventory2.OnProductShopInventoryChange = (ProductShopInventory.OnProductShopInventoryChangeDelegate)Delegate.Combine(streamingAssetShopInventory2.OnProductShopInventoryChange, new ProductShopInventory.OnProductShopInventoryChangeDelegate(HandleStreamingAssetShopInventoryChange));
		HandleStreamingAssetShopInventoryChange(streamingAssetShopInventory);
	}

	private List<StreamingAssetInfo> GetAvatarAccessories()
	{
		return streamingAssetShopInventory.Get(StreamingAssetType.AvatarAccessory).ToList();
	}

	public IUXCollectionItem GetItem(int index)
	{
		return cache[index];
	}

	private void HandleStreamingAssetShopInventoryChange(ProductShopInventory productShopInventory)
	{
		streamingAssetShopInventory = (StreamingAssetShopInventory)productShopInventory;
		List<StreamingAssetInfo> avatarAccessories = GetAvatarAccessories();
		cache = new IUXCollectionItem[avatarAccessories.Count];
		for (int i = 0; i < avatarAccessories.Count; i++)
		{
			int num = avatarAccessories.Count - 1 - i;
			cache[num] = new DefaultCollectionItem(num, avatarAccessories[i]);
		}
		NotifyOnCollectionChange();
	}

	private void NotifyOnCollectionChange()
	{
		if (OnCollectionChange != null)
		{
			OnCollectionChange();
		}
	}
}
