using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine.Events;

public static class AccessoryDataManager
{
	public static UnityAction readyCallback;

	private static bool accessoriesRequested;

	private static bool accessoriesReady;

	private static AccessoryShopDataClient accessoryShopData;

	public static int AccessoryBundleId => accessoryShopData.accessoryBundle.accessoryBundleID;

	public static AccessoryBundleClient AccessoryBundleClient => accessoryShopData.accessoryBundle;

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

	public static void Reset()
	{
		readyCallback = null;
		accessoriesRequested = false;
		accessoriesReady = false;
		accessoryShopData = null;
	}

	public static void SetAccessoryData(string accessoryData)
	{
		MVGameControllerBase.Game.ReceivedAccessoryData -= SetAccessoryData;
		accessoryShopData = JsonConvert.DeserializeObject<AccessoryShopDataClient>(accessoryData);
		accessoriesReady = true;
		if (readyCallback != null)
		{
			readyCallback();
		}
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
			if (accessoryData.Value.aMDID == id)
			{
				return accessoryData.Value;
			}
		}
		return null;
	}

	public static Dictionary<AccessoryCategory, List<AccessoryDataClient>> GetAccessoriesCategoryMap()
	{
		Dictionary<AccessoryCategory, List<AccessoryDataClient>> dictionary = new Dictionary<AccessoryCategory, List<AccessoryDataClient>>();
		foreach (KeyValuePair<int, AccessoryDataClient> accessoryData in accessoryShopData.accessoryDatas)
		{
			AccessoryDataClient value = accessoryData.Value;
			if (value.ShowItem)
			{
				if (!dictionary.ContainsKey(value.cat))
				{
					dictionary.Add(value.cat, new List<AccessoryDataClient>());
				}
				dictionary[value.cat].Add(value);
			}
		}
		return dictionary;
	}

	public static List<AccessoryDataClient> GetAccessoriesByCategoryId(AccessoryCategory category)
	{
		List<AccessoryDataClient> list = new List<AccessoryDataClient>();
		foreach (AccessoryDataClient value in accessoryShopData.accessoryDatas.Values)
		{
			if (value.ShowItem && value.cat == category)
			{
				list.Add(value);
			}
		}
		return list;
	}
}
