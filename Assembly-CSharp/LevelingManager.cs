using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public static class LevelingManager
{
	public static UnityAction OnLevelingInitialized;

	public static Dictionary<int, XPLevelLimits> TestLevelToLimits = new Dictionary<int, XPLevelLimits>
	{
		{
			1,
			new XPLevelLimits(0, 100, 1)
		},
		{
			2,
			new XPLevelLimits(101, 200, 2)
		},
		{
			3,
			new XPLevelLimits(201, 300, 3)
		},
		{
			4,
			new XPLevelLimits(301, 400, 4)
		},
		{
			5,
			new XPLevelLimits(401, 500, 5)
		},
		{
			6,
			new XPLevelLimits(501, 600, 6)
		}
	};

	public static bool IsInitialized { get; private set; }

	public static void Destroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnInitialData);
	}

	public static void Initialize(int profileID)
	{
		if (MVGameControllerBase.LevelingTestMode)
		{
			Test();
		}
		else
		{
			AsyncWWWManager.WWWRequest(new GetRequest(Urls.InitialData + profileID, OnInitialData, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
	}

	public static void Test()
	{
		Notify(CreateInitialLevelData());
	}

	public static void OnInitialData(WWW result)
	{
		InitialLevelData initialLevelData = JsonConvert.DeserializeObject<InitialLevelData>(result.text);
		Notify(initialLevelData);
	}

	private static void Notify(InitialLevelData initialLevelData)
	{
		BadgeManager.Initialize(initialLevelData.BadgeUrlData);
		MVGameControllerBase.Game.LocalPlayer.InitializeLeveling(initialLevelData);
		IsInitialized = true;
		if (OnLevelingInitialized != null)
		{
			OnLevelingInitialized();
			OnLevelingInitialized = null;
		}
	}

	private static InitialLevelData CreateInitialLevelData()
	{
		InitialLevelData initialLevelData = new InitialLevelData();
		initialLevelData.BadgeUrlData = TestBadgeUrlData();
		initialLevelData.Level = 2;
		initialLevelData.XP = 120;
		initialLevelData.XPLevelLimits = TestLevelToLimits[initialLevelData.Level];
		return initialLevelData;
	}

	private static List<BadgeUrlData> TestBadgeUrlData()
	{
		List<BadgeUrlData> list = new List<BadgeUrlData>();
		list.Add(new BadgeUrlData(1, Urls.StreamingAssets + "Promotion/Promotion_01.png"));
		list.Add(new BadgeUrlData(2, Urls.StreamingAssets + "Promotion/Promotion_02.png"));
		list.Add(new BadgeUrlData(3, Urls.StreamingAssets + "Promotion/Promotion_03.png"));
		list.Add(new BadgeUrlData(4, Urls.StreamingAssets + "Promotion/Promotion_04.png"));
		return list;
	}
}
