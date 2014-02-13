using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryExpirationChecker
{
	private Dictionary<int, InventoryExpirationInfo> expirationInfoMap = new Dictionary<int, InventoryExpirationInfo>();

	private HashSet<int> expiredUnrenewedIDs = new HashSet<int>();

	private HashSet<int> expiredIDs = new HashSet<int>();

	private HashSet<InventoryExpirationInfo> expiredUnrenewed = new HashSet<InventoryExpirationInfo>();

	private HashSet<InventoryExpirationInfo> expired = new HashSet<InventoryExpirationInfo>();

	public bool Contains(int inventoryID)
	{
		return expirationInfoMap.ContainsKey(inventoryID);
	}

	public bool ContainsExpired()
	{
		return expiredIDs.Count != 0;
	}

	public void AddExpirationInfo(InventoryExpirationInfo expInfo)
	{
		expirationInfoMap.Add(expInfo.InventoryID, expInfo);
		if (expInfo.ExpirationState == ProductExpirationState.Expired)
		{
			expiredIDs.Add(expInfo.InventoryID);
			expired.Add(expInfo);
		}
		if (expInfo.ExpirationState == ProductExpirationState.ExpiredNotRenewed)
		{
			expiredUnrenewedIDs.Add(expInfo.InventoryID);
			expiredUnrenewed.Add(expInfo);
		}
	}

	public InventoryExpirationInfo GetExpirationInfo(int inventoryID)
	{
		InventoryExpirationInfo value = null;
		expirationInfoMap.TryGetValue(inventoryID, out value);
		return value;
	}

	public void CheckExpiration()
	{
		foreach (InventoryExpirationInfo value in expirationInfoMap.Values)
		{
			if (value.IsExpiring)
			{
				value.CheckExpiration();
			}
			if (value.ExpirationState == ProductExpirationState.Expired)
			{
				expiredIDs.Add(value.InventoryID);
				expired.Add(value);
			}
			if (value.IsExpiredNotRenewed)
			{
				Debug.LogWarning((object)("ExpInfo " + value.InventoryID + " expired, not renewed"));
				expiredIDs.Remove(value.InventoryID);
				expired.Remove(value);
				expiredUnrenewedIDs.Add(value.InventoryID);
				expiredUnrenewed.Add(value);
			}
		}
		foreach (int expiredUnrenewedID in expiredUnrenewedIDs)
		{
			expirationInfoMap.Remove(expiredUnrenewedID);
		}
	}

	public IEnumerable<InventoryExpirationInfo> GetExpired()
	{
		return expired.AsEnumerable();
	}

	public List<InventoryExpirationInfo> GetInfos(ProductExpirationState state)
	{
		List<InventoryExpirationInfo> list = new List<InventoryExpirationInfo>();
		foreach (InventoryExpirationInfo value in expirationInfoMap.Values)
		{
			if (value.ExpirationState == state)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<InventoryExpirationInfo> GetExpiringInfos(TimeSpan expireTimeThershold)
	{
		List<InventoryExpirationInfo> list = new List<InventoryExpirationInfo>();
		foreach (InventoryExpirationInfo value in expirationInfoMap.Values)
		{
			if (value.TimeBeforeExpire < expireTimeThershold)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public HashSet<int> GetExpiringInfoIDs(TimeSpan expireTimeThershold)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (InventoryExpirationInfo value in expirationInfoMap.Values)
		{
			if (value.TimeBeforeExpire < expireTimeThershold)
			{
				hashSet.Add(value.InventoryID);
			}
		}
		return hashSet;
	}

	public List<InventoryExpirationInfo> GetExpiringInfos(HashSet<int> checkSet, TimeSpan minExpiration, TimeSpan range)
	{
		List<InventoryExpirationInfo> list = new List<InventoryExpirationInfo>();
		List<InventoryExpirationInfo> list2 = new List<InventoryExpirationInfo>();
		foreach (int item in checkSet)
		{
			InventoryExpirationInfo value = null;
			expirationInfoMap.TryGetValue(item, out value);
			if (value == null)
			{
				continue;
			}
			TimeSpan timeBeforeExpire = value.TimeBeforeExpire;
			if (timeBeforeExpire < range)
			{
				list2.Add(value);
				if (timeBeforeExpire < minExpiration)
				{
					list.Add(value);
				}
			}
		}
		return (list.Count != 0) ? list2 : list;
	}

	public HashSet<int> GetExpiringInfoIDs(HashSet<int> checkSet, TimeSpan minExpiration, TimeSpan range)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		foreach (int item in checkSet)
		{
			InventoryExpirationInfo value = null;
			expirationInfoMap.TryGetValue(item, out value);
			if (value == null)
			{
				continue;
			}
			TimeSpan timeBeforeExpire = value.TimeBeforeExpire;
			if (timeBeforeExpire < range)
			{
				hashSet2.Add(value.InventoryID);
				if (timeBeforeExpire < minExpiration)
				{
					hashSet.Add(value.InventoryID);
				}
			}
		}
		return (hashSet.Count != 0) ? hashSet2 : hashSet;
	}

	public void LogExpirations(string prependMessage)
	{
		expirationInfoMap.LogRecursive(prependMessage);
	}
}
