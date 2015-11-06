using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public static class LevelingManager
{
	public delegate void OnlevelingInitializedDelegate();

	public delegate void OnPlayModeLevelingEnabledChangedDelegate(bool enabled);

	private static ObscuredBool playModeLevelingEnabled;

	private static ObscuredInt playModeMinPlayers = 1;

	public static OnlevelingInitializedDelegate OnLevelingInitialized;

	public static OnPlayModeLevelingEnabledChangedDelegate OnPlayModeLevelingEnabledChanged;

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

	public static int PlayModeMinPlayers => playModeMinPlayers;

	public static bool IsInitialized { get; private set; }

	public static bool LevelingEnabled
	{
		get
		{
			if (!IsInitialized)
			{
				return false;
			}
			if (MVGameControllerBase.GameMode == MVGameMode.Edit || MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				return true;
			}
			if (MVGameControllerBase.GameMode == MVGameMode.Play && MVGameControllerBase.Game.Players.Count >= (int)playModeMinPlayers)
			{
				return true;
			}
			return false;
		}
	}

	public static void Initialize(int profileID)
	{
		if (MVGameControllerBase.LevelingTestMode)
		{
			Test();
		}
		else
		{
			AsyncWWWManager.WWWRequest(new GetRequest(Urls.InitialData + profileID, OnInitialData));
		}
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(OnPlayerListChanged));
	}

	public static void AddXPToLocalPlayer(string xpType, MVGameMode gameMode)
	{
		MVGameControllerBase.Game.LocalPlayer.AddXp(xpType, gameMode);
	}

	private static void OnPlayerListChanged()
	{
		if ((bool)playModeLevelingEnabled != LevelingEnabled && MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			playModeLevelingEnabled = LevelingEnabled;
			if (OnPlayModeLevelingEnabledChanged != null)
			{
				OnPlayModeLevelingEnabledChanged(playModeLevelingEnabled);
			}
		}
	}

	public static void Test()
	{
		Notify(CreateInitialLevelData());
	}

	public static void OnInitialData(WWW result)
	{
		InitialLevelData initialLevelData = JsonConvert.DeserializeObject<InitialLevelData>(result.text);
		playModeMinPlayers = initialLevelData.MinPlayersActivateXP;
		Notify(initialLevelData);
	}

	private static void Notify(InitialLevelData initialLevelData)
	{
		BadgeManager.Initialize(initialLevelData.BadgeUrlData);
		XPManager.Initialize(initialLevelData.XPManagerData);
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
		initialLevelData.XPManagerData = TestXPData();
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

	private static Dictionary<string, XPData> TestXPData()
	{
		Dictionary<string, XPData> dictionary = new Dictionary<string, XPData>();
		dictionary.Add("KilledSentryTower", new XPData(0, 20));
		dictionary.Add("CollectiblePickup", new XPData(1, 17));
		return dictionary;
	}
}
