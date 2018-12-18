using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;

public static class GamePassesManager
{
	private static bool showGamePassDataInConsole;

	public static ProgressionTierThresholdsManager progressionTierThresholdsManager;

	public static PlayerPlanetData playerPlanetData;

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

	public static void UpdatePlayerPlanetData(PlayerPlanetData playerPlanetData)
	{
		GamePassesManager.playerPlanetData = playerPlanetData;
		if (!ShowGamePassDataInConsole)
		{
			return;
		}
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, playerPlanetData.ToString());
		Dictionary<GamePassTier, TierState> tierPricingState = progressionTierThresholdsManager.GetTierPricingState(playerPlanetData.gamePoints, playerPlanetData.gamePassTier);
		foreach (KeyValuePair<GamePassTier, TierState> item in tierPricingState)
		{
			if (item.Value.tierLockState == TierLockState.PurchaseUnlock)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"{item.Key}. {item.Value}");
				break;
			}
		}
	}

	public static void SendCompleteStatus()
	{
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "/rgp for reset player data\n");
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, playerPlanetData.ToString());
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, progressionTierThresholdsManager.ToString());
	}
}
