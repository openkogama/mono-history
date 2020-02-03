using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.GamePassSystem;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
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
	private TierUnlockedItemsRewardInfo tierUnlockedItemsRewardInfoPrefab;

	[SerializeField]
	private TierUnlockedAccessItemsRewardInfo tierUnlockedAccessItemsRewardInfoPrefab;

	[SerializeField]
	private GamePassesSpawnRoleRewardInfo spawnRoleRewardInfoPrefab;

	[SerializeField]
	private GamePassesPurchaseButton purchaseButton;

	[SerializeField]
	private GameObject purchaseButtonObject;

	[SerializeField]
	private GameObject freeTryUI;

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
	private GamePassesShopContentCuller contentCuller;

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

	[SerializeField]
	private Image buttonAdImage;

	private GamePassTier gamePassTierDisplayed;

	private float lerpStartTime;

	private int oldGamePointValue;

	private int newGamePointValue;

	private bool shouldLerp;

	private bool isWaitingForFreeTryTier;

	private bool haveShownFreeTryUnlock;

	private static bool haveInitializedHighestTierRewardShown;

	public static void UpdateHighestTierRewardShown(GamePassTier newHighestTierRewardShown)
	{
		if (!haveInitializedHighestTierRewardShown)
		{
			TierUnlockedPopupController.HighestTierRewardShown = newHighestTierRewardShown;
			haveInitializedHighestTierRewardShown = true;
		}
	}

	public void Initialize(GamePassTier gamePassTierToDisplay)
	{
		headerText.text = "Game Tier " + (int)gamePassTierToDisplay;
		gamePassTierDisplayed = gamePassTierToDisplay;
		crystalAmount.text = GamePointAmountManager.GetTotalGamePointAmount().ToString();
		UpdateProgressBar(gamePassTierToDisplay);
		AddTierContent(gamePassTierToDisplay);
		contentCuller.Initialize();
		purchaseButton.Initialize(gamePassTierToDisplay);
		HandlePurchaseButtonVisibility(gamePassTierDisplayed);
		UpdateFreeTryUI();
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			HandleEditModeUI();
		}
		if (ShouldShowTierReward(gamePassTierToDisplay))
		{
			ShowTierUnlockedPopup(wasPurchased: false, wasTempUnlocked: false);
			return;
		}
		if (MVGameControllerBase.IsTouristSession)
		{
			ShowTouristInformationPopup();
			return;
		}
		buttonAdImage.enabled = !GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable;
		statusFooterObject.SetActive(value: false);
	}

	private void Start()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdateUI));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(UpdateTierCostTets));
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		LayoutRebuilder.ForceRebuildLayoutImmediate(tierListContainer);
		if (tierList.rect.width > tierListContainer.rect.width)
		{
			tierListContainer.pivot = new Vector2(0.5f, 0.5f);
		}
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(UpdateUI));
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
			freeTryUI.gameObject.SetActive(value: false);
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
			string text = Mathf.Floor(progressBar.Progress * (float)gamePointRequirementBase) + " / " + gamePointRequirementBase;
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
		if (GamePassesManager.GamePassesActive)
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
			string text2 = num2.ToString() + " / " + gamePointRequirementBase;
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
		if (freeTryUI.activeSelf)
		{
			freeTryUI.SetActive(value: false);
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

	private void UpdateUI()
	{
		UpdateTierCostTets();
		UpdateFreeTryUI();
		if (isWaitingForFreeTryTier)
		{
			OnPlayerPlanetDataUpdated();
		}
	}

	private void UpdateTierCostTets()
	{
		unlockPriceText.text = GamePassesManager.playerTierStateCalculator.progressionThresholds[gamePassTierDisplayed].goldPriceRequirement.ToString();
		int minutes = GamePassesManager.playerTierStateCalculator.progressionThresholds[gamePassTierDisplayed].estimatedRequiredPlaytime.Minutes;
		minutes += Mathf.FloorToInt((float)GamePassesManager.playerTierStateCalculator.progressionThresholds[gamePassTierDisplayed].estimatedRequiredPlaytime.Hours * 60f);
		unlockTimeText.text = minutes + " min.";
	}

	private void UpdateFreeTryUI()
	{
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		purchaseButton.SetFreeTryActivated(previewGamePassTier == gamePassTierDisplayed);
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		bool flag = MVGameControllerBase.GameMode == MVGameMode.Edit;
		bool flag2 = MVGameControllerBase.Game.GameTierShopRepository.GetTierItemData(gamePassTierDisplayed) != null;
		bool active = (int)gamePassTier < 3 && gamePassTierDisplayed == gamePassTier + 1 && !flag && flag2 && MVClientSettings.RewardedAdsEnabled;
		freeTryUI.SetActive(active);
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

	private void AddTierContent(GamePassTier gamePassTierToDisplay)
	{
		CreateSpawnRoleContent(gamePassTierDisplayed);
		CreateXPRewardInfo(gamePassTierToDisplay);
	}

	private void CreateSpawnPointInfo(MVTeam team)
	{
		SpawnPointInfo spawnPointInfo = UnityEngine.Object.Instantiate(spawnPointInfoPrefab);
		spawnPointInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
		spawnPointInfo.Initialize(team);
		contentCuller.AddContentElement(spawnPointInfo);
	}

	private void CreateXPRewardInfo(GamePassTier tier)
	{
		GamePassesXpRewardInfo gamePassesXpRewardInfo = UnityEngine.Object.Instantiate(xpRewardInfoPrefab);
		gamePassesXpRewardInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
		gamePassesXpRewardInfo.Initialize(tier);
		contentCuller.AddContentElement(gamePassesXpRewardInfo);
	}

	private void CreateUnlockedItemsInfo(GamePassTier tier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = SortOutNonLootItemsInShopData(tierShopData);
		if (dictionary.Count > 0)
		{
			TierUnlockedItemsRewardInfo tierUnlockedItemsRewardInfo = UnityEngine.Object.Instantiate(tierUnlockedItemsRewardInfoPrefab);
			tierUnlockedItemsRewardInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
			tierUnlockedItemsRewardInfo.Initialize(tier, dictionary);
			contentCuller.AddContentElement(tierUnlockedItemsRewardInfo);
		}
	}

	private Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> SortOutNonLootItemsInShopData(Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopDatum in tierShopData)
		{
			if (tierShopDatum.Value.Count > 0 && IsTierItemALootItem(tierShopDatum.Value[0]))
			{
				dictionary.Add(tierShopDatum.Key, tierShopDatum.Value);
			}
		}
		return dictionary;
	}

	private bool IsTierItemALootItem(MVWorldObjectClient item)
	{
		return item is MVPickupItemBase || item is MVWorldObjectSpawnerVehicle;
	}

	private void CreateUnlockedAccessItemsInfo(GamePassTier tier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = SortOutNonAccessItemsInShopData(tierShopData);
		if (dictionary.Count > 0)
		{
			TierUnlockedAccessItemsRewardInfo tierUnlockedAccessItemsRewardInfo = UnityEngine.Object.Instantiate(tierUnlockedAccessItemsRewardInfoPrefab);
			tierUnlockedAccessItemsRewardInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
			tierUnlockedAccessItemsRewardInfo.Initialize(tier, dictionary);
			contentCuller.AddContentElement(tierUnlockedAccessItemsRewardInfo);
		}
	}

	private Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> SortOutNonAccessItemsInShopData(Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopDatum in tierShopData)
		{
			List<MVWorldObjectClient> value = tierShopDatum.Value;
			if (IsTierItemAnAccessItem(tierShopDatum.Key, value[0]))
			{
				dictionary.Add(tierShopDatum.Key, value);
			}
		}
		return dictionary;
	}

	private bool IsTierItemAnAccessItem(MVWorldObjectDocumentationType worldObjectType, MVWorldObjectClient item)
	{
		return worldObjectType == MVWorldObjectDocumentationType.Lever || worldObjectType == MVWorldObjectDocumentationType.PressurePlate || item is MVTeleporter;
	}

	private void CreateSpawnRoleContent(GamePassTier gamePassTierToDisplay)
	{
		List<MVAvatarSpawnRoleCreator> sortedSpawnRoles = GetSortedSpawnRoles(gamePassTierDisplayed);
		for (int i = 0; i < sortedSpawnRoles.Count; i++)
		{
			CreateSpawnRoleInfo(i, sortedSpawnRoles[i], gamePassTierToDisplay);
		}
	}

	private List<MVAvatarSpawnRoleCreator> GetSortedSpawnRoles(GamePassTier gamePassTierToDisplay)
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		List<MVAvatarSpawnRoleCreator> list = new List<MVAvatarSpawnRoleCreator>();
		List<MVAvatarSpawnRoleCreator> list2 = new List<MVAvatarSpawnRoleCreator>();
		for (int i = 0; i < worldObjectsByType.Count; i++)
		{
			if (worldObjectsByType[i] is MVAvatarSpawnRoleCreator && ((MVAvatarSpawnRoleCreator)worldObjectsByType[i]).Tier == gamePassTierToDisplay)
			{
				list.Add((MVAvatarSpawnRoleCreator)worldObjectsByType[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			bool flag = false;
			for (int k = 0; k < list2.Count; k++)
			{
				if (list[j].Team < list2[k].Team)
				{
					list2.Insert(k, list[j]);
					flag = true;
					break;
				}
				if (list[j].Team == list2[k].Team && CalculateTotalSpawnRoleCost(list[j]) > CalculateTotalSpawnRoleCost(list2[k]))
				{
					list2.Insert(k, list[j]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list2.Add(list[j]);
			}
		}
		return list2;
	}

	private int CalculateTotalSpawnRoleCost(MVAvatarSpawnRoleCreator spawnRoleCreator)
	{
		AttributeSettingsManager attributeSettingsManagerAvatar = spawnRoleCreator.AttributeSettingsManagerAvatar;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)attributeSettingsManagerAvatar.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}

	private void CreateSpawnRoleInfo(int spawnRoleIndex, MVAvatarSpawnRoleCreator spawnRole, GamePassTier tier)
	{
		GamePassesSpawnRoleRewardInfo gamePassesSpawnRoleRewardInfo = UnityEngine.Object.Instantiate(spawnRoleRewardInfoPrefab);
		gamePassesSpawnRoleRewardInfo.transform.SetParent(tierListContainer.transform, worldPositionStays: false);
		gamePassesSpawnRoleRewardInfo.Initialize(spawnRoleIndex, spawnRole.GetSpawnRolePreviewObject(), spawnRole, tier);
		contentCuller.AddContentElement(gamePassesSpawnRoleRewardInfo);
	}

	private void ShowTierUnlockedPopup(bool wasPurchased, bool wasTempUnlocked)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(gamePassTierDisplayed, wasPurchased, wasTempUnlocked);
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
		ShowTierUnlockedPopup(wasPurchased: true, wasTempUnlocked: false);
		purchaseButtonObject.gameObject.SetActive(value: false);
		freeTryUI.SetActive(value: false);
		progressBar.Progress = 1f;
		disabledProgressBar.Progress = 1f;
		ActivateBar();
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(0, gamePassTier);
		int gamePointRequirementBase = tierPricingState[gamePassTierDisplayed].gamePointRequirementBase;
		string text = gamePointRequirementBase.ToString() + " / " + gamePointRequirementBase;
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
		return (int)TierUnlockedPopupController.HighestTierRewardShown < (int)GamePassesManager.PlayerPlanetData.gamePassTier && (int)TierUnlockedPopupController.HighestTierRewardShown < (int)tierToShow && (int)tierToShow <= (int)GamePassesManager.PlayerPlanetData.gamePassTier;
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

	private void ShowAd()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			if (GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable)
			{
				PreviewTier();
				GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable = false;
			}
			else
			{
				MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.PreviewTier);
			}
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
		}
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			PreviewTier();
			break;
		case RewardedAdResult.RewardNotUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The video was canceled. Your Free Try has not been activated."), TM._("Video canceled"));
			});
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	private void PreviewTier()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			MVGameControllerBase.OperationRequests.TogglePreviewTier();
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			isWaitingForFreeTryTier = true;
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (isWaitingForFreeTryTier)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			isWaitingForFreeTryTier = false;
		}
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)previewGamePassTier >= (int)gamePassTierDisplayed && !haveShownFreeTryUnlock)
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ShowTierUnlockedPopup(wasPurchased: false, wasTempUnlocked: true);
			haveShownFreeTryUnlock = true;
		}
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

	public void OnFreeTryPressed()
	{
		ShowAd();
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
