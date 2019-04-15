using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesShop : MonoBehaviour
{
	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Text progressHeader;

	[SerializeField]
	private Text progressText;

	[SerializeField]
	private ProgressBar progressBar;

	[SerializeField]
	private ProgressBar endResultProgressBar;

	[SerializeField]
	private ProgressBar disabledProgressBar;

	[SerializeField]
	private GameObject progressBarDivider;

	[SerializeField]
	private Text gameTierIconText;

	[SerializeField]
	private GameObject tierCheckmarkIcon;

	[SerializeField]
	private GameObject unlockedText;

	[SerializeField]
	private RectTransform tierListContainer;

	[SerializeField]
	private RectTransform tierList;

	[SerializeField]
	private SpawnPointInfo spawnPointInfoPrefab;

	[SerializeField]
	private GamePassesXpRewardInfo xpRewardInfoPrefab;

	[SerializeField]
	private GamePassesPurchaseButton purchaseButton;

	[SerializeField]
	private GameObject purchaseButtonObject;

	[SerializeField]
	private Text unlockTimeText;

	[SerializeField]
	private Text unlockPriceText;

	[SerializeField]
	private Text crystalAmount;

	[SerializeField]
	private GameObject editModeInformationObject;

	[SerializeField]
	private GameObject gameTierProgressObject;

	[SerializeField]
	private GameObject statusFooterObject;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	private TierPurchasePopup tierPurchasePopupPrefab;

	[SerializeField]
	private TierPurchaseNotEnoughGoldErrorPopup tierPurchaseGoldErrorPopupPrefab;

	[SerializeField]
	private GamePassesTouristInformationPopup touristInformationPopupPrefab;

	[SerializeField]
	private GameObject gamePassesShopInformationPrefab;

	private GamePassTier gamePassTierDisplayed;

	private float lerpStartTime;

	private int oldGamePointValue;

	private int newGamePointValue;

	private bool shouldLerp;

	private static GamePassTier highestTierRewardShown;

	private static bool haveInitializedHighestTierRewardShown;

	public static void UpdateHighestTierRewardShown(GamePassTier newHighestTierRewardShown)
	{
		if (!haveInitializedHighestTierRewardShown)
		{
			highestTierRewardShown = newHighestTierRewardShown;
			haveInitializedHighestTierRewardShown = true;
		}
	}

	public void Initialize(GamePassTier gamePassTierToDisplay, List<int> tierContent)
	{
		headerText.text = "Game Tier " + (int)gamePassTierToDisplay;
		gamePassTierDisplayed = gamePassTierToDisplay;
		crystalAmount.text = GamePointAmountManager.GetTotalGamePointAmount().ToString();
		UpdateProgressBar(gamePassTierToDisplay);
		AddTierContent(tierContent, gamePassTierToDisplay);
		purchaseButton.Initialize(gamePassTierToDisplay);
		HandlePurchaseButtonVisibility(gamePassTierDisplayed);
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			HandleEditModeUI();
		}
		if (ShouldShowTierReward(gamePassTierToDisplay))
		{
			ShowTierUnlockedPopup(wasPurchased: false);
		}
		else if (MVGameControllerBase.IsTouristSession)
		{
			ShowTouristInformationPopup();
		}
	}

	private void Start()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdateTierCostTets));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdateTierCostTets));
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdateTierCostTets));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdateTierCostTets));
	}

	private TierLockState GetTierLockState(GamePassTier gamePassTierToDisplay)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		return tierPricingState[gamePassTierToDisplay].tierLockState;
	}

	private void HandlePurchaseButtonVisibility(GamePassTier gamePassTierToDisplay)
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			purchaseButtonObject.gameObject.SetActive(value: false);
			UpdateTierCostTets();
		}
	}

	private void HandleEditModeUI()
	{
		editModeInformationObject.SetActive(value: true);
		gameTierProgressObject.SetActive(value: false);
		statusFooterObject.SetActive(value: false);
	}

	private void Update()
	{
		if (shouldLerp)
		{
			float num = Time.time - lerpStartTime;
			int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
			GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
			Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
			int gamePointRequirementBase = tierPricingState[gamePassTierDisplayed].gamePointRequirementBase;
			int num2 = ReduceGamePointsWithPreviousTierRequirements(gamePassTierDisplayed, oldGamePointValue, tierPricingState);
			int num3 = ReduceGamePointsWithPreviousTierRequirements(gamePassTierDisplayed, newGamePointValue, tierPricingState);
			float num4 = Mathf.Lerp((float)num2 / (float)gamePointRequirementBase, (float)num3 / (float)gamePointRequirementBase, num);
			progressBar.Progress = num4;
			disabledProgressBar.Progress = num4;
			if (num4 >= 1f)
			{
				ActivateBar();
			}
			else
			{
				DeactivateBar();
			}
			string text = (progressBar.Progress * (float)gamePointRequirementBase / (float)gamePointRequirementBase * 100f).ToString("0.00") + "%";
			progressText.text = text;
			disabledProgressBar.Progress = Mathf.Lerp((float)num2 / (float)gamePointRequirementBase, (float)num3 / (float)gamePointRequirementBase, num);
			if (!progressBarDivider.activeSelf && num4 > 0f)
			{
				progressBarDivider.SetActive(value: true);
			}
			if (num > 1f)
			{
				shouldLerp = false;
			}
		}
	}

	private void UpdateProgressBar(GamePassTier gamePassTierToDisplay)
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			return;
		}
		Text text = gameTierIconText;
		int num = (int)gamePassTierToDisplay;
		text.text = num.ToString();
		if (IsProgressBarEnabled() && GamePassProgressionController.IsProgressionEnabled)
		{
			progressHeader.text = "Unlock Progress";
		}
		else
		{
			progressHeader.text = "Progress Locked";
			progressBar.gameObject.SetActive(value: false);
			disabledProgressBar.gameObject.SetActive(value: true);
			progressText.gameObject.SetActive(value: false);
		}
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		if (IsProgressBarEnabled() && GamePassesManager.GamePassesActive)
		{
			int num2 = ReduceGamePointsWithPreviousTierRequirements(gamePassTierToDisplay, progressionGamePoints, tierPricingState);
			if ((float)num2 < 0f)
			{
				num2 = 0;
			}
			int gamePointRequirementBase = tierPricingState[gamePassTierToDisplay].gamePointRequirementBase;
			if (num2 > gamePointRequirementBase)
			{
				num2 = gamePointRequirementBase;
			}
			float progress = (float)num2 / (float)gamePointRequirementBase;
			progressBar.Progress = progress;
			disabledProgressBar.Progress = progress;
			string text2 = ((float)num2 / (float)gamePointRequirementBase * 100f).ToString("0.00") + "%";
			progressText.text = text2;
			if (progressBarDivider.activeSelf && progressBar.Progress <= 0f)
			{
				progressBarDivider.SetActive(value: false);
			}
		}
		if (tierPricingState[gamePassTierToDisplay].tierLockState == TierLockState.Unlocked)
		{
			ActivateBar();
		}
	}

	private void ActivateBar()
	{
		if (!tierCheckmarkIcon.activeSelf)
		{
			tierCheckmarkIcon.SetActive(value: true);
		}
		if (gameTierIconText.gameObject.activeSelf)
		{
			gameTierIconText.gameObject.SetActive(value: false);
		}
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit && !unlockedText.activeSelf)
		{
			unlockedText.SetActive(value: true);
		}
		progressHeader.text = string.Empty;
		progressBar.Progress = 1f;
		disabledProgressBar.Progress = 1f;
		if (purchaseButtonObject.activeSelf)
		{
			purchaseButtonObject.SetActive(value: false);
		}
	}

	private void DeactivateBar()
	{
		if (!IsProgressBarEnabled())
		{
			progressHeader.text = "Progress Locked";
			if (progressBar.gameObject.activeSelf)
			{
				progressBar.gameObject.SetActive(value: false);
			}
			if (!disabledProgressBar.gameObject.activeSelf)
			{
				disabledProgressBar.gameObject.SetActive(value: true);
			}
			if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit && unlockedText.activeSelf)
			{
				unlockedText.SetActive(value: false);
			}
		}
		if (tierCheckmarkIcon.activeSelf)
		{
			tierCheckmarkIcon.SetActive(value: false);
		}
		if (!gameTierIconText.gameObject.activeSelf)
		{
			gameTierIconText.gameObject.SetActive(value: true);
		}
	}

	private void UpdateTierCostTets()
	{
		unlockPriceText.text = GamePassesManager.playerTierStateCalculator.progressionThresholds[gamePassTierDisplayed].goldPriceRequirement.ToString();
		unlockTimeText.text = GamePassesManager.playerTierStateCalculator.progressionThresholds[gamePassTierDisplayed].estimatedRequiredPlaytime.Minutes + " min.";
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

	private void AddTierContent(List<int> tierContent, GamePassTier gamePassTierToDisplay)
	{
		CreateXPRewardInfo(gamePassTierToDisplay);
		for (int i = 0; i < tierContent.Count; i++)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(tierContent[i]);
			if (worldObjectClient != null)
			{
				switch (worldObjectClient.WorldObjectType)
				{
				case WorldObjectType.SpawnPointBlue:
					CreateSpawnPointInfo(MVTeam.Blue);
					break;
				case WorldObjectType.SpawnPointRed:
					CreateSpawnPointInfo(MVTeam.Red);
					break;
				case WorldObjectType.SpawnPointGreen:
					CreateSpawnPointInfo(MVTeam.Green);
					break;
				case WorldObjectType.SpawnPointYellow:
					CreateSpawnPointInfo(MVTeam.Yellow);
					break;
				case WorldObjectType.SpawnPoint:
					CreateSpawnPointInfo(MVTeam.None);
					break;
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(tierListContainer);
		if (tierList.rect.width < tierListContainer.rect.width)
		{
			tierListContainer.pivot = new Vector2(0f, 0.5f);
		}
	}

	private void CreateSpawnPointInfo(MVTeam team)
	{
		SpawnPointInfo spawnPointInfo = UnityEngine.Object.Instantiate(spawnPointInfoPrefab);
		spawnPointInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
		spawnPointInfo.Initialize(team);
	}

	private void CreateXPRewardInfo(GamePassTier tier)
	{
		GamePassesXpRewardInfo gamePassesXpRewardInfo = UnityEngine.Object.Instantiate(xpRewardInfoPrefab);
		gamePassesXpRewardInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
		gamePassesXpRewardInfo.Initialize(tier);
	}

	private void ShowTierUnlockedPopup(bool wasPurchased)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(gamePassTierDisplayed, wasPurchased);
		if ((int)highestTierRewardShown < (int)gamePassTierDisplayed)
		{
			highestTierRewardShown = gamePassTierDisplayed;
		}
	}

	private void ShowTouristInformationPopup()
	{
		GamePassesTouristInformationPopup touristInformationPopup = UnityEngine.Object.Instantiate(touristInformationPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(touristInformationPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void ShowPurchaseConfirmPopup(int price)
	{
		TierPurchasePopup tierPurchasePopup = UnityEngine.Object.Instantiate(tierPurchasePopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierPurchasePopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierPurchasePopup.Initialize(gamePassTierDisplayed, price, OnSuccessfulPurchase);
	}

	private void ShowPurchaseGoldErrorPopup()
	{
		TierPurchaseNotEnoughGoldErrorPopup tierErrorPopup = UnityEngine.Object.Instantiate(tierPurchaseGoldErrorPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierErrorPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierErrorPopup.Initialize(gamePassTierDisplayed);
	}

	private void HandlePurchase()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int remainingGoldPriceRequired = tierPricingState[gamePassTierDisplayed].remainingGoldPriceRequired;
		int gold = MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold;
		if (remainingGoldPriceRequired <= gold)
		{
			ShowPurchaseConfirmPopup(remainingGoldPriceRequired);
		}
		else if (MVGameControllerBase.IsTouristSession)
		{
			ShowTouristInformationPopup();
		}
		else
		{
			ShowPurchaseGoldErrorPopup();
		}
	}

	private void OnSuccessfulPurchase()
	{
		ShowTierUnlockedPopup(wasPurchased: true);
		GamePassesHighlightArrowManager.IncreaseHighestTierRewardShown();
		purchaseButtonObject.gameObject.SetActive(value: false);
		progressBar.Progress = 1f;
		disabledProgressBar.Progress = 1f;
		ActivateBar();
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(0, gamePassTier);
		int gamePointRequirementBase = tierPricingState[gamePassTierDisplayed].gamePointRequirementBase;
		string text = ((float)gamePointRequirementBase / (float)gamePointRequirementBase * 100f).ToString("0.00") + "%";
		progressText.text = text;
		GamePointGainEffectManager.HaveShownGamePointGainEffect(GetTotalGamePointRequirementForTier(gamePassTierDisplayed));
	}

	private void TestTier()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int num = 0;
		int num2 = 0;
		for (int i = 1; i <= (int)gamePassTierDisplayed; i++)
		{
			if (i != (int)gamePassTierDisplayed)
			{
				num2 += tierPricingState[(GamePassTier)i].gamePointRequirementBase;
			}
			num += tierPricingState[(GamePassTier)i].gamePointRequirementBase;
		}
		if (gamePassTier == gamePassTierDisplayed)
		{
			MVGameControllerBase.OperationRequests.SetTier(GamePassTier.Tier0);
			newGamePointValue = num2;
		}
		else
		{
			newGamePointValue = num;
			MVGameControllerBase.OperationRequests.SetTier(gamePassTierDisplayed);
		}
		shouldLerp = true;
		lerpStartTime = Time.time;
		oldGamePointValue = Mathf.FloorToInt(progressBar.Progress * (float)(num - num2) + (float)num2);
	}

	private bool ShouldShowTierReward(GamePassTier tierToShow)
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			return false;
		}
		bool flag = (int)highestTierRewardShown < (int)GamePassesManager.PlayerPlanetData.gamePassTier && (int)highestTierRewardShown < (int)tierToShow && (int)tierToShow <= (int)GamePassesManager.PlayerPlanetData.gamePassTier;
		if (flag)
		{
			highestTierRewardShown = tierToShow;
		}
		return flag;
	}

	private bool IsProgressBarEnabled()
	{
		if (GamePassesManager.playerTierStateCalculator != null && GamePassesManager.playerTierStateCalculator.gamePassRewardsActivated)
		{
			return true;
		}
		return false;
	}

	private int GetTotalGamePointRequirementForTier(GamePassTier tier)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int num = 0;
		for (int num2 = (int)tier; num2 > 0; num2--)
		{
			GamePassTier key = (GamePassTier)num2;
			num += tierPricingState[key].gamePointRequirementBase;
		}
		return num;
	}

	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void OnButtonPress()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		TierLockState tierLockState = tierPricingState[gamePassTierDisplayed].tierLockState;
		if (MVGameControllerBase.EditModeUI != null && MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			TestTier();
		}
		else if (tierLockState == TierLockState.PurchaseUnlock)
		{
			HandlePurchase();
		}
	}

	public void OnTestTierPress()
	{
		TestTier();
	}

	public void ShowGamePassShopInformationPopup()
	{
		GameObject gamePassesShopInformation = UnityEngine.Object.Instantiate(gamePassesShopInformationPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShopInformation, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
