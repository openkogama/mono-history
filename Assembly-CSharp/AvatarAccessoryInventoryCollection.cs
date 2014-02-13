using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class AvatarAccessoryInventoryCollection : IUXCollection
{
	private StreamingAssetInventory streamingAssetInventory;

	private IUXCollectionItem[] cache = new IUXCollectionItem[0];

	public OnCollectionChangeDelegate OnCollectionChange { get; set; }

	public int Count => GetAvatarAccessoryIds().Count;

	public int CountIncludingEquipped => GetAvatarAccessoryIds(removeEquipped: false).Count;

	public AvatarAccessoryInventoryCollection(StreamingAssetInventory streamingAssetInventory)
	{
		this.streamingAssetInventory = streamingAssetInventory;
		StreamingAssetInventory streamingAssetInventory2 = this.streamingAssetInventory;
		streamingAssetInventory2.OnProductInventoryChange = (ProductInventory<StreamingAssetInfo>.OnProductInventoryChangeDelegate)Delegate.Combine(streamingAssetInventory2.OnProductInventoryChange, new ProductInventory<StreamingAssetInfo>.OnProductInventoryChangeDelegate(HandleStreamingAssetInventoryChange));
		HandleStreamingAssetInventoryChange(streamingAssetInventory);
	}

	private List<int> GetAvatarAccessoryIds(bool removeEquipped = true)
	{
		List<ProductInventoryInfo<StreamingAssetInfo>> list = streamingAssetInventory.Get(StreamingAssetType.AvatarAccessory).ToList();
		if (removeEquipped)
		{
			list.RemoveAll((ProductInventoryInfo<StreamingAssetInfo> aa) => aa.IsEquipped);
		}
		List<int> list2 = new List<int>();
		foreach (ProductInventoryInfo<StreamingAssetInfo> item in list)
		{
			list2.Add(item.InventoryID);
		}
		return list2;
	}

	public IUXCollectionItem GetItem(int index)
	{
		return cache[index];
	}

	private void HandleStreamingAssetInventoryChange(ProductInventory<StreamingAssetInfo> productInventory)
	{
		streamingAssetInventory = (StreamingAssetInventory)productInventory;
		List<int> avatarAccessoryIds = GetAvatarAccessoryIds();
		cache = new IUXCollectionItem[avatarAccessoryIds.Count];
		int num = 0;
		foreach (int item in avatarAccessoryIds)
		{
			cache[num] = new DefaultCollectionItem(num, item);
			num++;
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
