using System;
using System.Collections.Generic;
using UnityEngine;

public static class BadgeManager
{
	public const int defaultMaxFriendsLimit = 200;

	private static Dictionary<int, BadgeUrlData> badgeUrls = new Dictionary<int, BadgeUrlData>();

	private static int maxLevelBadge = 0;

	private const string fromUnityArgument = "?Unity=2";

	public static void Initialize(List<BadgeUrlData> badgeUrlDatas)
	{
		foreach (BadgeUrlData badgeUrlData in badgeUrlDatas)
		{
			if (badgeUrlData.Level > maxLevelBadge)
			{
				maxLevelBadge = badgeUrlData.Level;
			}
			if (!badgeUrls.ContainsKey(badgeUrlData.Level))
			{
				badgeUrls.Add(badgeUrlData.Level, badgeUrlData);
			}
		}
	}

	public static int GetFriendsLimit(int level)
	{
		if (!badgeUrls.ContainsKey(level))
		{
			return 200;
		}
		return badgeUrls[level].FriendsLimit;
	}

	public static void UnsubscribeGetBadgeRequest(Action<WWW> callback)
	{
		AsyncWWWManager.UnsubscribeWWWRequest(callback);
	}

	public static void GetBadgeTexture(int level, Action<WWW> callback)
	{
		if (maxLevelBadge == 0)
		{
			Debug.LogError("No badges was loaded");
			return;
		}
		if (!badgeUrls.ContainsKey(level))
		{
			Debug.LogWarning("Level exceeds defined badges. Using maxBadge");
			level = maxLevelBadge;
		}
		AsyncWWWManager.WWWRequest(new CachedGetRequest(badgeUrls[level].URL + "?Unity=2", callback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	public static void Reset()
	{
		badgeUrls = new Dictionary<int, BadgeUrlData>();
		maxLevelBadge = 0;
	}
}
