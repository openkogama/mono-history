using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesUI : MonoBehaviour
{
	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private Text totalGamePointAmountText;

	[SerializeField]
	private GamePassesHighScoreList highScoreListPrefab;

	[SerializeField]
	private GamePassesHighlightArrowManager highLightArrowManager;

	[SerializeField]
	private GameTierProgressBar tierProgressBar;

	[SerializeField]
	private GameTierProgressBarGainEffectController gainEffectController;

	[SerializeField]
	private GamePassesWelcomeReward welcomeRewardPopup;

	private bool isInitialized;

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			if (GamePassesManager.GamePassesActive)
			{
				totalGamePointAmountText.text = GamePassesManager.PlayerPlanetData.highScoreGamePoints.ToString();
				GamePassesShop.UpdateHighestTierRewardShown(GamePassesManager.PlayerPlanetData.gamePassTier);
			}
			tierProgressBar.Initialize();
			gainEffectController.Initialize();
		}
	}

	public void TryShowWelcomeReward()
	{
		if (ShouldShowWelcomeReward())
		{
			welcomeRewardPopup.Initialize();
		}
	}

	public void OnTier1ShopPressed()
	{
		ShowGamePassesShop(GamePassTier.Tier1);
	}

	public void OnTier2ShopPressed()
	{
		ShowGamePassesShop(GamePassTier.Tier2);
	}

	public void OnTier3ShopPressed()
	{
		ShowGamePassesShop(GamePassTier.Tier3);
	}

	public void ShowHighScore()
	{
		GamePassesHighScoreList highScoreList = UnityEngine.Object.Instantiate(highScoreListPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(highScoreList.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void ReplayGainEffect(int previousGamePointAmount, int newGamePointAmount)
	{
		tierProgressBar.ReplayGainEffect(previousGamePointAmount, newGamePointAmount);
		gainEffectController.ReplayGainEffect(previousGamePointAmount, newGamePointAmount);
	}

	private void ShowGamePassesShop(GamePassTier tierToShow)
	{
		InstantiateGamePassesShop(tierToShow);
		highLightArrowManager.OnTierBeingShown(tierToShow);
	}

	private void InstantiateGamePassesShop(GamePassTier tierToShow)
	{
		GamePassesShop gamePassesShop = UnityEngine.Object.Instantiate(gamePassesShopPrefab);
		gamePassesShop.Initialize(tierToShow);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void OnEnable()
	{
		if (GamePassesManager.GamePassesActive)
		{
			totalGamePointAmountText.text = GamePassesManager.PlayerPlanetData.highScoreGamePoints.ToString();
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		}
	}

	private void OnDisable()
	{
		if (GamePassesManager.GamePassesActive)
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		}
	}

	private bool ShouldShowWelcomeReward()
	{
		if (GamePassesManager.playerTierStateCalculator == null)
		{
			return false;
		}
		int welcomeReward = GamePassesManager.playerTierStateCalculator.welcomeReward;
		return welcomeReward > 0 && GamePassProgressionController.IsProgressionEnabled && !GamePassesManager.PlayerPlanetData.playerPlanetMetaData.DailyWelcomeRewardClaimedToday() && GamePassesManager.playerTierStateCalculator.gamePassRewardsActivated;
	}

	private void OnPlayerPlanetDataUpdated()
	{
		totalGamePointAmountText.text = GamePassesManager.PlayerPlanetData.highScoreGamePoints.ToString();
	}
}
