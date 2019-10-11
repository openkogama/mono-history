using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.UI;

public class GamePassesPurchaseButton : MonoBehaviour
{
	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text disabledPriceText;

	[SerializeField]
	private GameObject testToggle;

	[SerializeField]
	private GameObject freeTryButton;

	[SerializeField]
	private GameObject freeTryActivated;

	[SerializeField]
	private ToggleButtonAnimation toggleButton;

	[SerializeField]
	private Button purchaseButton;

	[SerializeField]
	private GameObject disabledPurchaseButton;

	[SerializeField]
	private GamePassesTextBubble informationTextBubble;

	private GamePassTier tierDisplayed;

	public void Initialize(GamePassTier tierToDisplay)
	{
		tierDisplayed = tierToDisplay;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		TierLockState tierLockState = tierPricingState[tierToDisplay].tierLockState;
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			SetUpTestButton();
			return;
		}
		switch (tierLockState)
		{
		case TierLockState.Unlocked:
			HideButton();
			break;
		case TierLockState.PurchaseUnlock:
			SetUpPurchaseButton(tierToDisplay, tierPricingState);
			SetupFreeTryButton();
			break;
		case TierLockState.Locked:
			SetUpDisableButton(tierToDisplay, tierPricingState);
			break;
		}
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdatePriceText));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdatePriceText));
	}

	public void OnTestButtonPressed()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		bool flag = gamePassTier == tierDisplayed;
		if (flag == toggleButton.IsToggleOn)
		{
			toggleButton.Toggle();
		}
	}

	public void OnDisabledButtonPressed()
	{
		informationTextBubble.Activate("You need to unlock game tier " + (int)(tierDisplayed - 1) + " first!");
	}

	public void SetFreeTryActivated(bool isActive)
	{
		freeTryButton.SetActive(!isActive);
		freeTryActivated.SetActive(isActive);
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdatePriceText));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdatePriceText));
	}

	private void HideButton()
	{
		purchaseButton.gameObject.SetActive(value: false);
	}

	private void SetUpTestButton()
	{
		purchaseButton.gameObject.SetActive(value: false);
		testToggle.gameObject.SetActive(value: true);
		toggleButton.Initialize();
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if (gamePassTier == tierDisplayed)
		{
			toggleButton.SetToggleOnWithoutInterpolation();
		}
	}

	private void SetUpPurchaseButton(GamePassTier tierToDisplay, Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus)
	{
		priceText.text = gameTierShopStatus[tierToDisplay].remainingGoldPriceRequired.ToString();
	}

	private void SetUpDisableButton(GamePassTier tierToDisplay, Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus)
	{
		purchaseButton.gameObject.SetActive(value: false);
		disabledPurchaseButton.gameObject.SetActive(value: true);
		disabledPriceText.text = gameTierShopStatus[tierToDisplay].remainingGoldPriceRequired.ToString();
	}

	private void SetupFreeTryButton()
	{
		freeTryButton.SetActive(value: true);
	}

	private void UpdatePriceText()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		priceText.text = tierPricingState[tierDisplayed].remainingGoldPriceRequired.ToString();
		disabledPriceText.text = tierPricingState[tierDisplayed].remainingGoldPriceRequired.ToString();
	}
}
