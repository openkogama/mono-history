using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierLockedDetailsPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Text lockedDescriptionText;

	[SerializeField]
	private Text lockedButtonText;

	[SerializeField]
	private ProgressBar tierProgressBar;

	[SerializeField]
	private Text progressText;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private TierUnlockDetailsPopup tierUnlockDetailsPrefab;

	private GamePassTier tierToPurchase;

	private UnityAction onPurchaseSuccessful;

	public void Initialize(GamePassTier tierToPurchase, UnityAction OnPurchaseSuccessful)
	{
		this.tierToPurchase = tierToPurchase;
		onPurchaseSuccessful = OnPurchaseSuccessful;
		Text text = tierText;
		int num = (int)tierToPurchase;
		text.text = num.ToString();
		UpdateLockedText();
		if (GamePassesManager.GamePassesActive)
		{
			UpdateTierProgressBar();
		}
	}

	public void ShowTier()
	{
		GamePassesShop gamePassesShop = Object.Instantiate(gamePassesShopPrefab);
		gamePassesShop.Initialize(tierToPurchase);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void ShowLockedTier()
	{
		GamePassesShop gamePassesShop = Object.Instantiate(gamePassesShopPrefab);
		gamePassesShop.Initialize(GetLockedTier());
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void UpdateLockedText()
	{
		headerText.text = GetHeaderText();
		lockedButtonText.text = GetLockedButtonText();
		lockedDescriptionText.text = GetLockedDescription();
	}

	private string GetHeaderText()
	{
		string key = "Game Tier {0}: Locked";
		string format = TM._(key);
		return string.Format(format, (int)tierToPurchase);
	}

	private string GetLockedButtonText()
	{
		GamePassTier lockedTier = GetLockedTier();
		string key = "GO TO TIER {0}";
		string format = TM._(key);
		return string.Format(format, (int)lockedTier);
	}

	private string GetLockedDescription()
	{
		GamePassTier lockedTier = GetLockedTier();
		if (tierToPurchase - lockedTier > 1)
		{
			string key = "You need to unlock Tier {0} and {1} before you can unlock this Tier.";
			string format = TM._(key);
			return string.Format(format, (int)lockedTier, (int)(lockedTier + 1));
		}
		string key2 = "You need to unlock Tier {0} before you can unlock this Tier.";
		string format2 = TM._(key2);
		return string.Format(format2, (int)lockedTier);
	}

	private GamePassTier GetLockedTier()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		return gamePassTier + 1;
	}

	private void UpdateTierProgressBar()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int num = ReduceGamePointsWithPreviousTierRequirements(tierToPurchase, progressionGamePoints, tierPricingState);
		if ((float)num < 0f)
		{
			num = 0;
		}
		int gamePointRequirementBase = tierPricingState[tierToPurchase].gamePointRequirementBase;
		if (num > gamePointRequirementBase)
		{
			num = gamePointRequirementBase;
		}
		float progress = (float)num / (float)gamePointRequirementBase;
		tierProgressBar.Progress = progress;
		string text = num.ToString() + " / " + gamePointRequirementBase;
		progressText.text = text;
	}

	private int ReduceGamePointsWithPreviousTierRequirements(GamePassTier gamePassTierToDisplay, int gamePoints, Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus)
	{
		for (int num = (int)(gamePassTierToDisplay - 1); num > 0; num--)
		{
			GamePassTier key = (GamePassTier)num;
			gamePoints -= gameTierShopStatus[key].gamePointRequirementBase;
		}
		return gamePoints;
	}

	private void OnEnable()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if ((int)gamePassTier < (int)(tierToPurchase - 1))
		{
			UpdateLockedText();
			UpdateTierProgressBar();
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ShowPurchasePopup();
	}

	private void ShowPurchasePopup()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int remainingGoldPriceRequired = tierPricingState[tierToPurchase].remainingGoldPriceRequired;
		TierUnlockDetailsPopup tierPurchasePopup = Object.Instantiate(tierUnlockDetailsPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierPurchasePopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierPurchasePopup.Initialize(tierToPurchase, remainingGoldPriceRequired, onPurchaseSuccessful);
	}
}
