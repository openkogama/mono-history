using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public static class AccessoryDataManager
{
	private static bool accessoriesRequested;

	private static bool accessoriesReady;

	private static AccessoryShopDataClient accessoryShopData;

	public static UnityAction readyCallback;

	public static void SetReady()
	{
		if (accessoriesReady)
		{
			if (readyCallback != null)
			{
				readyCallback();
			}
		}
		else if (!accessoriesRequested)
		{
			MVGameControllerBase.OperationRequests.RequestAccessoryData();
			accessoriesRequested = true;
		}
	}

	public static int GetAccessoryBundleId()
	{
		return accessoryShopData.accessoryBundle.accessoryBundleID;
	}

	public static AccessoryBundleClient GetAccessoryBundleClient()
	{
		return accessoryShopData.accessoryBundle;
	}

	public static void SetToOwns(int streamingAssetId)
	{
		accessoryShopData.accessoryDatas[streamingAssetId].owns = true;
	}

	public static AccessoryDataClient GetAccessoryDataByStreamingAssetId(int id)
	{
		if (accessoryShopData.accessoryDatas.ContainsKey(id))
		{
			return accessoryShopData.accessoryDatas[id];
		}
		return null;
	}

	public static AccessoryDataClient GetAccessoryDataByMetaDataId(int id)
	{
		foreach (KeyValuePair<int, AccessoryDataClient> accessoryData in accessoryShopData.accessoryDatas)
		{
			if (accessoryData.Value.accessoryMetaDataID == id)
			{
				return accessoryData.Value;
			}
		}
		return null;
	}

	public static void SetAccessoryData(string accessoryData)
	{
		accessoryShopData = JsonConvert.DeserializeObject<AccessoryShopDataClient>(accessoryData);
		Debug.Log("accessoryShopData.accessoryBundle.accessoryBundleItems.Count: " + accessoryShopData.accessoryBundle.accessoryBundleItems.Count);
		accessoriesReady = true;
		if (readyCallback != null)
		{
			readyCallback();
		}
	}

	public static Dictionary<AccessoryCategory, List<AccessoryDataClient>> GetAccessoriesCategoryMap()
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> dictionary = new Dictionary<AccessoryCategory, List<AccessoryDataClient>>();
		foreach (KeyValuePair<int, AccessoryDataClient> accessoryData in accessoryShopData.accessoryDatas)
		{
			AccessoryDataClient value = accessoryData.Value;
			if (value.ShowItem)
			{
				if (!dictionary.ContainsKey(value.category))
				{
					dictionary.Add(value.category, new List<AccessoryDataClient>());
				}
				dictionary[value.category].Add(value);
			}
		}
		return dictionary;
	}

	public static List<AccessoryDataClient> GetAccessoriesByCategoryId(AccessoryCategory category)
	{
		List<AccessoryDataClient> list = new List<AccessoryDataClient>();
		foreach (AccessoryDataClient value in accessoryShopData.accessoryDatas.Values)
		{
			if (value.ShowItem && value.category == category)
			{
				list.Add(value);
			}
		}
		return list;
	}
}
