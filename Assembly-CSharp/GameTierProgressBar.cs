using System;
using System.Collections.Generic;
using GameMeterVisuals;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameTierProgressBar : MonoBehaviour
{
	[Serializable]
	private struct TierProgressData
	{
		public ProgressBar progressBar;

		public Text progressText;

		public GameObject progressDivider;

		public GamePassesTextBubble progressBarTextBubble;

		public Text hoverProgressText;

		public GamePassesTextBubble avatarHead;

		public RawImage avatarHeadImage;

		public GameObject avatarHeadUI;

		public ProgressBar disabledProgressBar;

		public Text disabledProgressText;

		public NotificationFade notificationFade;

		public NotificationFade disabledNotificationFade;

		public GameObject disabledProgressDivider;

		public GamePassesTextBubble disabledBarTextBubble;

		public GameObject tierIconCheckmark;

		public GameObject tierIconNumber;

		public ProgressBar endResultProgressBar;
	}

	[SerializeField]
	private AvatarPreviewer previewer;

	[SerializeField]
	private GamePassesTextBubble highScoreTipTextBubble;

	[SerializeField]
	private GameObject betaModeInformationPopup;

	[SerializeField]
	private GameObject buildModeInformationPopup;

	[SerializeField]
	private GameObject touristInformationPopup;

	[SerializeField]
	protected List<GameMeterVisualEffect> gameMeterVisualEffects = new List<GameMeterVisualEffect>();

	[SerializeField]
	private List<TierProgressData> tierProgressDataList;

	private bool isInitialized;

	private float interpolateTowardsProgressValue;

	private float previousProgressValue;

	private float interpolationStartTime;

	private bool shouldInterpolate;

	private const float interpolationDuration = 2f;

	private AvatarPreviewer headPreviewer;

	private const int avatarHeadImageWidth = 128;

	private const int avatarHeadImageHeight = 128;

	private static bool haveShownTips;

	public void OnHeadClick()
	{
		int num = Mathf.FloorToInt(previousProgressValue);
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		string textBubbleText = tierPricingState[(GamePassTier)((byte)num + 1)].gamePointRequirementBase - ReduceGamePointsWithPreviousTierRequirements((GamePassTier)((byte)num + 1), progressionGamePoints, tierPricingState) + " Crystals to go!";
		tierProgressDataList[num].avatarHead.Activate(textBubbleText);
	}

	public void OnInactiveProgressBarClicked(int progressBarNumberPressed)
	{
		tierProgressDataList[progressBarNumberPressed].disabledBarTextBubble.Activate("Progression is disabled in Stand-Alone to prevent cheating.");
	}

	public void OnActiveProgressBarClicked(int progressBarNumberPressed)
	{
		if (MVGameControllerBase.IsTouristSession)
		{
			GameObject informationPopup = UnityEngine.Object.Instantiate(touristInformationPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(informationPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
		}
	}

	public GamePassTier GetCurrentTier()
	{
		return (GamePassTier)((byte)Mathf.FloorToInt(previousProgressValue) + 1);
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(HandleDisabledProgressBarVisibility));
			GamePointGainEffectManager.OnGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnGamePointGainEffectShown, new Action<int>(OnHaveShownGainEffect));
			GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown, new Action<int>(OnHaveShownTierProgressBarGainEffect));
			if (GamePassesManager.GamePassesActive)
			{
				int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
				UpdateProgressBars(progressionGamePoints);
				UpdateDividerVisibility(progressionGamePoints);
				HandleDisabledProgressBarVisibility();
				previousProgressValue = CalculateTotalProgressValue(progressionGamePoints);
				interpolateTowardsProgressValue = previousProgressValue;
				CreateAvatarHeadImages();
			}
		}
	}

	private void Start()
	{
		if (!haveShownTips)
		{
			HandleShowTips();
		}
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(HandleDisabledProgressBarVisibility));
		GamePointGainEffectManager.OnGamePointGainEffectShown = (Action<int>)Delegate.Remove(GamePointGainEffectManager.OnGamePointGainEffectShown, new Action<int>(OnHaveShownGainEffect));
		GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown = (Action<int>)Delegate.Remove(GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown, new Action<int>(OnHaveShownTierProgressBarGainEffect));
	}

	private void OnEnable()
	{
		interpolationStartTime = Time.time;
		if (GamePassesManager.GamePassesActive)
		{
			int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
			GamePointGainEffectManager.HaveShownTierProgressBarGamePointGainEffect(progressionGamePoints);
		}
	}

	private void OnDisable()
	{
		if (MVGameControllerBase.GameSessionData != null && MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit && IsProgressBarEnabled())
		{
			previousProgressValue = interpolateTowardsProgressValue;
			int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
			UpdateProgressBars(progressionGamePoints);
			UpdateDividerVisibility(progressionGamePoints);
			HideProgressText();
		}
	}

	private void Update()
	{
		if (!shouldInterpolate)
		{
			return;
		}
		int value = Mathf.FloorToInt(previousProgressValue);
		value = Mathf.Clamp(value, 0, tierProgressDataList.Count - 1);
		float num = Mathf.LerpUnclamped(previousProgressValue - (float)value, Mathf.Clamp(interpolateTowardsProgressValue - (float)value, 0f, 1f), (Time.time - interpolationStartTime) * 0.5f);
		if (num > interpolateTowardsProgressValue - (float)value)
		{
			num = interpolateTowardsProgressValue - (float)value;
			shouldInterpolate = false;
			tierProgressDataList[value].notificationFade.Unpause();
			tierProgressDataList[value].disabledNotificationFade.Unpause();
		}
		if (num >= 1f)
		{
			tierProgressDataList[value].avatarHeadUI.gameObject.SetActive(value: false);
			tierProgressDataList[value].progressDivider.SetActive(value: false);
			tierProgressDataList[value].disabledProgressDivider.SetActive(value: false);
			ActivateBar(value);
			tierProgressDataList[value].notificationFade.Unpause();
			tierProgressDataList[value].disabledNotificationFade.Unpause();
			int num2 = value + 1;
			previousProgressValue = num2;
			interpolationStartTime = Time.time;
			if (num2 < tierProgressDataList.Count)
			{
				tierProgressDataList[num2].avatarHeadUI.gameObject.SetActive(value: true);
				tierProgressDataList[num2].progressDivider.SetActive(value: true);
				tierProgressDataList[num2].disabledProgressDivider.SetActive(value: true);
				tierProgressDataList[num2].notificationFade.Activate();
				tierProgressDataList[num2].notificationFade.PauseAt(0.5f);
				tierProgressDataList[num2].disabledNotificationFade.Activate();
				tierProgressDataList[num2].disabledNotificationFade.PauseAt(0.5f);
			}
		}
		tierProgressDataList[value].progressBar.Progress = num;
		tierProgressDataList[value].disabledProgressBar.Progress = num;
		UpdateProgressText(num + (float)value, (GamePassTier)((byte)value + 1));
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			UpdateEditModeDisabledProgressBars();
			return;
		}
		if (!GamePassProgressionController.IsProgressionEnabled || !IsProgressBarEnabled())
		{
			HandleUnlockedTiersProgressBars();
			return;
		}
		interpolateTowardsProgressValue = CalculateTotalProgressValue(GamePassesManager.PlayerPlanetData.progressionGamePoints);
		if (previousProgressValue != interpolateTowardsProgressValue)
		{
			shouldInterpolate = true;
		}
		int num = Mathf.FloorToInt(Mathf.Clamp(previousProgressValue, 0f, tierProgressDataList.Count - 1));
		if (shouldInterpolate)
		{
			tierProgressDataList[num].notificationFade.Activate();
			tierProgressDataList[num].notificationFade.PauseAt(0.5f);
			tierProgressDataList[num].disabledNotificationFade.Activate();
			tierProgressDataList[num].disabledNotificationFade.PauseAt(0.5f);
			int num2 = num;
			for (float num3 = interpolateTowardsProgressValue - (float)num2; num3 > 0f; num3--)
			{
				tierProgressDataList[num2].endResultProgressBar.Progress = Mathf.Clamp(num3, 0f, 1f);
				num2++;
			}
		}
	}

	private void OnHaveShownGainEffect(int newGamePointAmountShown)
	{
		previousProgressValue = CalculateTotalProgressValue(newGamePointAmountShown);
		UpdateProgressBars(newGamePointAmountShown);
		UpdateDividerVisibility(newGamePointAmountShown);
	}

	private void OnHaveShownTierProgressBarGainEffect(int newGamePointAmountShown)
	{
		if (!gameObject.activeInHierarchy)
		{
			OnHaveShownGainEffect(newGamePointAmountShown);
		}
	}

	private void UpdateProgressBars(int playerGamePoints)
	{
		UpdateProgressBar(GamePassTier.Tier1, playerGamePoints);
		UpdateProgressBar(GamePassTier.Tier2, playerGamePoints);
		UpdateProgressBar(GamePassTier.Tier3, playerGamePoints);
		for (int i = 0; i < gameMeterVisualEffects.Count; i++)
		{
			gameMeterVisualEffects[i].ExecuteEffect();
		}
	}

	private void HandleUnlockedTiersProgressBars()
	{
		for (int i = 1; i <= 3; i++)
		{
			if (IsTierUnlocked((GamePassTier)i))
			{
				tierProgressDataList[i - 1].disabledProgressBar.Progress = 1f;
				tierProgressDataList[i - 1].progressBar.Progress = 1f;
				ActivateBar(i - 1);
			}
		}
	}

	private void UpdateEditModeDisabledProgressBars()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		for (int num = 2; num >= 0; num--)
		{
			if ((int)(gamePassTier - 1) >= num)
			{
				ActivateBar(num);
			}
			else
			{
				DeactivateBar(num);
			}
		}
	}

	private void UpdateProgressBar(GamePassTier progressBarToUpdate, int playerGamePoints)
	{
		if (GamePassesManager.GamePassesActive)
		{
			GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
			Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(playerGamePoints, gamePassTier);
			int num = ReduceGamePointsWithPreviousTierRequirements(progressBarToUpdate, playerGamePoints, tierPricingState);
			int gamePointRequirementBase = tierPricingState[progressBarToUpdate].gamePointRequirementBase;
			if (gamePointRequirementBase > 0)
			{
				float num2 = (float)num / (float)gamePointRequirementBase;
				if (num > gamePointRequirementBase)
				{
					num = gamePointRequirementBase;
				}
				if (num < 0)
				{
					num = 0;
				}
				string text = ((float)num / (float)gamePointRequirementBase * 100f).ToString("0.00") + "%";
				tierProgressDataList[(int)(progressBarToUpdate - 1)].progressBar.Progress = num2;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].progressText.text = text;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].hoverProgressText.text = text;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressBar.Progress = num2;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressText.text = text;
				if (num2 >= 1f)
				{
					ActivateBar((int)(progressBarToUpdate - 1));
				}
			}
			else if (IsTierUnlocked(progressBarToUpdate))
			{
				tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressBar.Progress = 1f;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].progressBar.Progress = 1f;
				ActivateBar((int)(progressBarToUpdate - 1));
			}
			else
			{
				tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressBar.Progress = 0f;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].progressBar.Progress = 0f;
			}
		}
		else
		{
			tierProgressDataList[(int)(progressBarToUpdate - 1)].progressBar.Progress = 0f;
			tierProgressDataList[(int)(progressBarToUpdate - 1)].progressText.text = string.Empty;
			tierProgressDataList[(int)(progressBarToUpdate - 1)].hoverProgressText.text = string.Empty;
			tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressBar.Progress = 0f;
			tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressText.text = string.Empty;
			if (IsTierUnlocked(progressBarToUpdate))
			{
				tierProgressDataList[(int)(progressBarToUpdate - 1)].disabledProgressBar.Progress = 1f;
				tierProgressDataList[(int)(progressBarToUpdate - 1)].progressBar.Progress = 1f;
				ActivateBar((int)(progressBarToUpdate - 1));
			}
		}
	}

	private void HideProgressText()
	{
		for (int i = 0; i < tierProgressDataList.Count; i++)
		{
			tierProgressDataList[i].notificationFade.Deactivate();
			tierProgressDataList[i].disabledNotificationFade.Deactivate();
		}
	}

	private void ActivateBar(int barIndex)
	{
		tierProgressDataList[barIndex].progressBar.Progress = 1f;
		tierProgressDataList[barIndex].disabledProgressBar.Progress = 1f;
		if (!tierProgressDataList[barIndex].tierIconCheckmark.activeSelf)
		{
			tierProgressDataList[barIndex].tierIconCheckmark.SetActive(value: true);
		}
		if (tierProgressDataList[barIndex].tierIconNumber.activeSelf)
		{
			tierProgressDataList[barIndex].tierIconNumber.SetActive(value: false);
		}
	}

	private void DeactivateBar(int barIndex)
	{
		tierProgressDataList[barIndex].progressBar.Progress = 0f;
		tierProgressDataList[barIndex].disabledProgressBar.Progress = 0f;
		if (tierProgressDataList[barIndex].tierIconCheckmark.activeSelf)
		{
			tierProgressDataList[barIndex].tierIconCheckmark.SetActive(value: false);
		}
		if (!tierProgressDataList[barIndex].tierIconNumber.activeSelf)
		{
			tierProgressDataList[barIndex].tierIconNumber.SetActive(value: true);
		}
	}

	private float GetProgressBarPercentage(GamePassTier tierToShowProgressFor)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		float num = (float)ReduceGamePointsWithPreviousTierRequirements(tierToShowProgressFor, progressionGamePoints, tierPricingState) / (float)tierPricingState[tierToShowProgressFor].gamePointRequirementBase;
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
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

	private void UpdateDividerVisibility(int playerGamePoints)
	{
		if (!GamePassesManager.GamePassesActive)
		{
			return;
		}
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(playerGamePoints, gamePassTier);
		bool flag = false;
		for (int i = 1; i <= 3; i++)
		{
			GamePassTier gamePassTier2 = (GamePassTier)i;
			float num = (float)ReduceGamePointsWithPreviousTierRequirements(gamePassTier2, playerGamePoints, tierPricingState) / (float)tierPricingState[gamePassTier2].gamePointRequirementBase;
			if (num >= 1f)
			{
				tierProgressDataList[i - 1].progressDivider.gameObject.SetActive(value: false);
				tierProgressDataList[i - 1].avatarHeadUI.gameObject.SetActive(value: false);
				tierProgressDataList[i - 1].disabledProgressDivider.gameObject.SetActive(value: false);
			}
			else if (num < 1f && num > 0f && !flag && IsProgressBarEnabled())
			{
				flag = true;
				tierProgressDataList[i - 1].progressDivider.gameObject.SetActive(value: true);
				tierProgressDataList[i - 1].avatarHeadUI.gameObject.SetActive(value: true);
				tierProgressDataList[i - 1].disabledProgressDivider.gameObject.SetActive(value: true);
			}
			else
			{
				tierProgressDataList[i - 1].progressDivider.gameObject.SetActive(value: false);
				tierProgressDataList[i - 1].avatarHeadUI.gameObject.SetActive(value: false);
				tierProgressDataList[i - 1].disabledProgressDivider.gameObject.SetActive(value: false);
			}
		}
	}

	private float CalculateTotalProgressValue(int gamePoints)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		float num = 0f;
		for (int i = 1; i <= 3; i++)
		{
			GamePassTier key = (GamePassTier)i;
			int gamePointRequirementBase = tierPricingState[key].gamePointRequirementBase;
			if (gamePointRequirementBase > 0)
			{
				float num2 = (float)gamePoints / (float)gamePointRequirementBase;
				gamePoints -= gamePointRequirementBase;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				num += num2;
			}
			else
			{
				num = 0f;
			}
		}
		return num;
	}

	private int CalculateGamePointsFromTotalProgress(float totalProgressValue)
	{
		return CalculateGamePointsFromTierProgress(totalProgressValue, GamePassTier.Tier3);
	}

	private int CalculateGamePointsFromTierProgress(float totalProgressValue, GamePassTier tierToCalculateTo)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		float num = 0f;
		for (int i = 1; i <= (int)tierToCalculateTo; i++)
		{
			GamePassTier key = (GamePassTier)i;
			int gamePointRequirementBase = tierPricingState[key].gamePointRequirementBase;
			float num2 = (float)tierPricingState[key].gamePointRequirementBase * totalProgressValue;
			if (num2 > (float)gamePointRequirementBase)
			{
				num2 = gamePointRequirementBase;
			}
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			totalProgressValue--;
			num += num2;
		}
		return Mathf.FloorToInt(num);
	}

	private void UpdateProgressText(float totalProgress, GamePassTier currentTier)
	{
		int gamePoints = CalculateGamePointsFromTierProgress(totalProgress, currentTier);
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int num = ReduceGamePointsWithPreviousTierRequirements(currentTier, gamePoints, tierPricingState);
		int gamePointRequirementBase = tierPricingState[currentTier].gamePointRequirementBase;
		string text = ((float)num / (float)gamePointRequirementBase * 100f).ToString("0.00") + "%";
		tierProgressDataList[(int)(currentTier - 1)].progressText.text = text;
		tierProgressDataList[(int)(currentTier - 1)].hoverProgressText.text = text;
		tierProgressDataList[(int)(currentTier - 1)].disabledProgressText.text = text;
	}

	private void HandleDisabledProgressBarVisibility()
	{
		bool flag = false;
		flag = true;
		bool flag2 = IsProgressBarEnabled() && !flag;
		for (int i = 1; i <= 3; i++)
		{
			if (IsTierUnlocked((GamePassTier)i))
			{
				ActivateBar(i - 1);
			}
			tierProgressDataList[i - 1].progressBar.gameObject.SetActive(flag2);
			tierProgressDataList[i - 1].disabledProgressBar.gameObject.SetActive(!flag2);
		}
	}

	private void HandleShowTips()
	{
		haveShownTips = true;
		bool flag = MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit;
		bool flag2 = false;
		flag2 = true;
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		if (!flag && !isTouristSession)
		{
			int rank = GamePassesManager.PlayerPlanetData.rank;
			highScoreTipTextBubble.Activate("Rank\n" + rank);
		}
		if (flag)
		{
			GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
			int index = Mathf.Clamp((int)(gamePassTier - 1), 0, tierProgressDataList.Count);
			tierProgressDataList[index].disabledBarTextBubble.Activate("Progression is disabled in build mode");
			tierProgressDataList[index].progressBarTextBubble.Activate("Progression is disabled in build mode");
		}
		else if (isTouristSession)
		{
			tierProgressDataList[0].disabledBarTextBubble.Activate("Sign up to be able to save progression");
			tierProgressDataList[0].progressBarTextBubble.Activate("Sign up to be able to save progression");
		}
		else if (flag2)
		{
			GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
			int index2 = Mathf.Clamp((int)(gamePassTier2 - 1), 0, tierProgressDataList.Count);
			tierProgressDataList[index2].disabledBarTextBubble.Activate("Progression is disabled in standalone to prevent cheating");
			tierProgressDataList[index2].progressBarTextBubble.Activate("Progression is disabled in standalone to prevent cheating");
		}
		else if (!IsProgressBarEnabled())
		{
			GamePassTier gamePassTier3 = GamePassesManager.PlayerPlanetData.gamePassTier;
			int index3 = Mathf.Clamp((int)(gamePassTier3 - 1), 0, tierProgressDataList.Count);
			tierProgressDataList[index3].disabledBarTextBubble.Activate("Progression locked while the game is in Beta-Mode");
			tierProgressDataList[index3].progressBarTextBubble.Activate("Progression locked while the game is in Beta-Mode");
		}
	}

	private bool IsProgressBarEnabled()
	{
		if (GamePassesManager.playerTierStateCalculator != null && GamePassesManager.playerTierStateCalculator.gamePassRewardsActivated)
		{
			return true;
		}
		return false;
	}

	private bool IsTierUnlocked(GamePassTier tierToCheck)
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		TierLockState tierLockState = tierPricingState[tierToCheck].tierLockState;
		return tierLockState == TierLockState.Unlocked;
	}

	private void CreateAvatarHeadImages()
	{
		headPreviewer = UnityEngine.Object.Instantiate(previewer);
		MVCubeModelInstance bodyPart = MVGameControllerBase.LocalPlayer.Body.GetBodyPart("Head");
		GameObject gameObject = UnityEngine.Object.Instantiate(bodyPart.GameObject);
		gameObject.SetLayerRecursively(LayerUtil.GetLayerNumber(LayerFlags.Preview));
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
			{
				if (componentsInChildren[i].materials[j].HasProperty("_Color"))
				{
					Color color = componentsInChildren[i].materials[j].color;
					color.a = 1f;
					componentsInChildren[i].materials[j].color = color;
				}
			}
		}
		Transform previewItemsRoot = new GameObject().transform;
		headPreviewer.Initialize(128, 128, CameraClearFlags.Color, LayerFlags.Preview, new Vector3(0f, -0.5f, -1f), previewItemsRoot, new Vector3(100f, 100f, 100f), "Avatar Head preview", bodyPart, gameObject, new Vector3(15f, 0f, 0f));
		headPreviewer.previewCam.transform.position += new Vector3(0f, 0.72f, 0f);
		headPreviewer.PreviewGameObject.transform.Rotate(new Vector3(0f, 227f, 0f));
		for (int k = 0; k < tierProgressDataList.Count; k++)
		{
			tierProgressDataList[k].avatarHeadImage.texture = headPreviewer.PreviewTexture;
		}
	}
}
