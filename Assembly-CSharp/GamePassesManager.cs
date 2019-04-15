using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;

public static class GamePassesManager
{
	private static bool showGamePassDataInConsole;

	public static PlayerTierStateCalculator playerTierStateCalculator;

	public static Action OnPlayerPlanetDataUpdated;

	private static PlayerPlanetData playerPlanetData;

	public static bool ShowGamePassDataInConsole
	{
		get
		{
			return showGamePassDataInConsole;
		}
		set
		{
			if (!showGamePassDataInConsole && value)
			{
				SendCompleteStatus();
			}
			showGamePassDataInConsole = true;
		}
	}

	public static bool GamePassesActive { get; set; }

	public static PlayerPlanetData PlayerPlanetData
	{
		get
		{
			return playerPlanetData;
		}
		set
		{
			playerPlanetData = value;
			GamePassesActive = playerPlanetData != null;
			if (MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled))
			{
				GamePassesActive = false;
			}
		}
	}

	public static void UpdatePlayerPlanetData(PlayerPlanetData playerPlanetData)
	{
		HandleNewTierUnlocked(playerPlanetData);
		GamePassesManager.playerPlanetData = playerPlanetData;
		if (ShowGamePassDataInConsole)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, playerPlanetData.ToString());
			Dictionary<GamePassTier, PlayerTierState> tierPricingState = playerTierStateCalculator.GetTierPricingState(playerPlanetData.progressionGamePoints, playerPlanetData.gamePassTier);
			foreach (KeyValuePair<GamePassTier, PlayerTierState> item in tierPricingState)
			{
				if (item.Value.tierLockState == TierLockState.PurchaseUnlock)
				{
					MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"{item.Key}. {item.Value}");
					break;
				}
			}
		}
		if (OnPlayerPlanetDataUpdated != null)
		{
			OnPlayerPlanetDataUpdated();
		}
	}

	public static void SendCompleteStatus()
	{
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "/rgp for reset player data\n");
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, playerPlanetData.ToString());
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, playerTierStateCalculator.ToString());
	}

	private static void HandleNewTierUnlocked(PlayerPlanetData newPlayerPlanetData)
	{
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
		{
			GamePassTier gamePassTier = newPlayerPlanetData.gamePassTier;
			GamePassTier gamePassTier2 = playerPlanetData.gamePassTier;
			if ((int)gamePassTier > (int)gamePassTier2)
			{
				SendTierUnlockedNotification(gamePassTier);
			}
		}
	}

	private static void SendTierUnlockedNotification(GamePassTier unlockedTier)
	{
		if (GamePassesActive)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)4, (int)unlockedTier);
			NotificationController.PushNotification(NotificationType.TierUnlocked, dictionary);
		}
	}
}
