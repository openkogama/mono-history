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
	private RectTransform testToggleOffMaskTransform;

	[SerializeField]
	private RectTransform testToggleOffContentTransform;

	[SerializeField]
	private RectTransform testToggleButtonTransform;

	[SerializeField]
	private Button purchaseButton;

	[SerializeField]
	private GameObject disabledPurchaseButton;

	[SerializeField]
	private GamePassesTextBubble informationTextBubble;

	private float toggleOffOriginalPositionX;

	private float toggleButtonOriginalPositionX;

	private GamePassTier tierDisplayed;

	private float interpolateToggleMaskStartPositionX;

	private float interpolateToggleMaskNewPositionX;

	private float interpolateToggleContentStartPositionX;

	private float interpolateToggleContentNewPositionX;

	private float interpolateToggleButtonStartPositionX;

	private float interpolateToggleButtonNewPositionX;

	private float interpolationStartTime;

	private bool isToggleOn;

	private const float toggleInterpolationDuration = 0.1f;

	private const float toggleButtonMoveAmount = 131f;

	public void Initialize(GamePassTier tierToDisplay)
	{
		tierDisplayed = tierToDisplay;
		toggleOffOriginalPositionX = testToggleOffMaskTransform.localPosition.x;
		toggleButtonOriginalPositionX = testToggleButtonTransform.localPosition.x;
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
		if (flag == isToggleOn)
		{
			isToggleOn = !flag;
			interpolationStartTime = Time.time;
			interpolateToggleMaskNewPositionX = toggleOffOriginalPositionX;
			interpolateToggleMaskStartPositionX = toggleOffOriginalPositionX;
			if (isToggleOn)
			{
				interpolateToggleMaskNewPositionX += testToggleOffMaskTransform.rect.width;
			}
			else
			{
				interpolateToggleMaskStartPositionX += testToggleOffMaskTransform.rect.width;
			}
			interpolateToggleContentNewPositionX = toggleOffOriginalPositionX;
			interpolateToggleContentStartPositionX = toggleOffOriginalPositionX;
			if (isToggleOn)
			{
				interpolateToggleContentNewPositionX -= testToggleOffMaskTransform.rect.width;
			}
			else
			{
				interpolateToggleContentStartPositionX -= testToggleOffMaskTransform.rect.width;
			}
			interpolateToggleButtonNewPositionX = toggleButtonOriginalPositionX;
			interpolateToggleButtonStartPositionX = toggleButtonOriginalPositionX;
			if (isToggleOn)
			{
				interpolateToggleButtonNewPositionX += 131f;
			}
			else
			{
				interpolateToggleButtonStartPositionX += 131f;
			}
		}
	}

	public void OnDisabledButtonPressed()
	{
		informationTextBubble.Activate("You need to unlock game tier " + (int)(tierDisplayed - 1) + " first!");
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdatePriceText));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdatePriceText));
	}

	private void Update()
	{
		float t = (Time.time - interpolationStartTime) / (0.1f + Time.deltaTime);
		Vector3 localPosition = testToggleOffMaskTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleMaskStartPositionX, interpolateToggleMaskNewPositionX, t);
		testToggleOffMaskTransform.localPosition = localPosition;
		localPosition = testToggleOffMaskTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleContentStartPositionX, interpolateToggleContentNewPositionX, t);
		testToggleOffContentTransform.localPosition = localPosition;
		localPosition = testToggleButtonTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleButtonStartPositionX, interpolateToggleButtonNewPositionX, t);
		testToggleButtonTransform.localPosition = localPosition;
	}

	private void HideButton()
	{
		purchaseButton.gameObject.SetActive(value: false);
	}

	private void SetUpTestButton()
	{
		purchaseButton.gameObject.SetActive(value: false);
		testToggle.gameObject.SetActive(value: true);
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if (gamePassTier == tierDisplayed)
		{
			isToggleOn = true;
			Vector3 localPosition = testToggleOffMaskTransform.localPosition;
			localPosition.x = toggleOffOriginalPositionX + testToggleOffMaskTransform.rect.width;
			testToggleOffMaskTransform.localPosition = localPosition;
			localPosition = testToggleOffContentTransform.localPosition;
			localPosition.x = toggleOffOriginalPositionX - testToggleOffContentTransform.rect.width;
			testToggleOffContentTransform.localPosition = localPosition;
			localPosition = testToggleButtonTransform.localPosition;
			localPosition.x = toggleButtonOriginalPositionX + 131f;
			testToggleButtonTransform.localPosition = localPosition;
		}
		else
		{
			isToggleOn = false;
		}
		interpolateToggleMaskNewPositionX = testToggleOffMaskTransform.localPosition.x;
		interpolateToggleContentNewPositionX = testToggleOffContentTransform.localPosition.x;
		interpolateToggleButtonNewPositionX = testToggleButtonTransform.localPosition.x;
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

	private void UpdatePriceText()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		priceText.text = tierPricingState[tierDisplayed].remainingGoldPriceRequired.ToString();
		disabledPriceText.text = tierPricingState[tierDisplayed].remainingGoldPriceRequired.ToString();
	}
}
