using System;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public static class XPManager
{
	private static Dictionary<string, XPData> xpDatas = new Dictionary<string, XPData>();

	public static void Initialize(Dictionary<string, XPData> xpDatas)
	{
		XPManager.xpDatas = xpDatas;
	}

	public static bool TryGetXPData(string xpType, out XPData xpData)
	{
		if (!xpDatas.TryGetValue(xpType, out xpData))
		{
			Debug.LogWarning("Did not get: " + xpType);
			return false;
		}
		return true;
	}

	public static string GetXPText(byte xpId)
	{
		string xPStringById = GetXPStringById(xpId);
		return LocalizedStringEnumsXP._(xPStringById);
	}

	public static int GetXPAmount(byte xpId)
	{
		if (xpId == 0)
		{
			return 0;
		}
		return GetXPData(xpId).XPAmount;
	}

	private static string GetXPStringById(byte xpId)
	{
		Debug.LogWarning("Consider to optimize this");
		foreach (KeyValuePair<string, XPData> xpData in xpDatas)
		{
			if (xpData.Value.XPId == xpId)
			{
				return xpData.Key;
			}
		}
		throw new Exception("Failed to get xp string key");
	}

	private static XPData GetXPData(byte xpId)
	{
		foreach (XPData value in xpDatas.Values)
		{
			if (value.XPId == xpId)
			{
				return value;
			}
		}
		throw new Exception("Failed to get XPData");
	}
}
