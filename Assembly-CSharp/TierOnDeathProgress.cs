using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.UI;

public class TierOnDeathProgress : MonoBehaviour
{
	[SerializeField]
	private Text crystalsGainedSinceDeath;

	[SerializeField]
	private Text nextTierText;

	[SerializeField]
	private ProgressBar tierProgressBar;

	[SerializeField]
	private Text progressText;

	[SerializeField]
	private GameObject progressBarDivider;

	[SerializeField]
	private float progressLerpDuration;

	[SerializeField]
	private float lockFadeLerpDuration;

	[SerializeField]
	private AnimationCurve lockShakeCurve;

	[SerializeField]
	private RectTransform lockImage;

	[SerializeField]
	private CanvasGroup unlockImage;

	private GamePassTier currentTier;

	private int crystalValue;

	private float currentCrystalValue;

	private float timer = -0.5f;

	private float crystalTimer = -0.5f;

	private float timeBeforeStartLerp = -0.5f;

	private float fromProgress;

	private float toProgress;

	private int currentGamePoints;

	private int gamePointsRequired;

	private Vector3 lockStartRot;

	private float intensity;

	private float tierToInterpolateFrom;

	private float tierToInterpolateTo;

	private bool unlockingTier;

	public bool IsShowingTierProgress { get; private set; }

	public void Initialize()
	{
		IsShowingTierProgress = true;
		tierToInterpolateFrom = Mathf.FloorToInt(CalculateTotalProgressValue(GamePointGainEffectManager.GamePointAmountShown));
		tierToInterpolateTo = Mathf.FloorToInt(CalculateTotalProgressValue(GamePassesManager.PlayerPlanetData.progressionGamePoints));
		nextTierText.text = tierToInterpolateFrom.ToString();
		Debug.Log("from-to: " + tierToInterpolateFrom + " - " + tierToInterpolateTo);
		timer = timeBeforeStartLerp;
		crystalTimer = timeBeforeStartLerp;
		currentTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		int gamePointAmountShown = GamePointGainEffectManager.GamePointAmountShown;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		crystalValue = progressionGamePoints - gamePointAmountShown;
		currentCrystalValue = crystalValue;
		crystalsGainedSinceDeath.text = crystalValue.ToString();
		int progressionGamePoints2 = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints2, currentTier);
		currentGamePoints = crystalValue;
		gamePointsRequired = tierPricingState[currentTier].gamePointRequirementBase;
		currentGamePoints = Mathf.Clamp(currentGamePoints, 0, crystalValue);
		lockImage.gameObject.SetActive(value: true);
		unlockImage.gameObject.SetActive(value: false);
		intensity = 0f;
		if (crystalValue != 0)
		{
			intensity = 1f;
		}
		lockStartRot = lockImage.localRotation.eulerAngles;
	}

	private void OnEnable()
	{
		StartCoroutine(DoTierProgress());
	}

	private void OnDisable()
	{
		IsShowingTierProgress = false;
	}

	private IEnumerator DoTierProgress()
	{
		Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus = GamePassesManager.playerTierStateCalculator.GetTierPricingState(GamePassesManager.PlayerPlanetData.progressionGamePoints, (GamePassTier)(int)tierToInterpolateFrom);
		int previousShownGamePointAmount = GamePointGainEffectManager.GamePointAmountShown;
		int newGamePointAmountToShow = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		int from = Mathf.FloorToInt(tierToInterpolateFrom);
		int to = Mathf.FloorToInt(tierToInterpolateTo);
		int current = from;
		bool stopped = false;
		fromProgress = previousShownGamePointAmount;
		toProgress = newGamePointAmountToShow;
		GamePointGainEffectManager.HaveShownTierProgressBarGamePointGainEffect(newGamePointAmountToShow);
		GamePointGainEffectManager.HaveShownGamePointGainEffect(newGamePointAmountToShow);
		Debug.Log("From/To " + fromProgress + "/" + toProgress);
		while (current <= to && !stopped)
		{
			if (current == 3)
			{
				nextTierText.text = current.ToString();
				tierProgressBar.Progress = 1f;
				break;
			}
			nextTierText.text = (current + 1).ToString();
			gamePointsRequired = gameTierShopStatus[(GamePassTier)((byte)current + 1)].gamePointRequirementBase;
			float totalProgress = Mathf.Clamp01(timer / progressLerpDuration);
			float crystalProgress = Mathf.Clamp01(crystalTimer / progressLerpDuration);
			float tierProgress = ReduceGamePointsWithPreviousTierRequirements(gamePoints: (int)Mathf.Floor(Mathf.Lerp(fromProgress, toProgress, totalProgress)), gamePassTierToDisplay: (GamePassTier)((byte)current + 1), gameTierShopStatus: gameTierShopStatus);
			tierProgressBar.Progress = Mathf.Clamp01(tierProgress / (float)gamePointsRequired);
			currentCrystalValue = Mathf.Ceil(Mathf.Lerp(crystalValue, 0f, crystalProgress));
			crystalsGainedSinceDeath.text = currentCrystalValue.ToString();
			if (lockImage.gameObject.activeInHierarchy)
			{
				float num = lockShakeCurve.Evaluate(totalProgress) * intensity;
				Vector3 euler = lockStartRot;
				euler.z = num * 180f;
				lockImage.localRotation = Quaternion.Euler(euler);
			}
			if (progressBarDivider.activeSelf && tierProgressBar.Progress <= 0f)
			{
				progressBarDivider.SetActive(value: false);
			}
			unlockingTier = tierProgress >= (float)gamePointsRequired;
			int displayedProgress = (int)Mathf.Min(tierProgress, gamePointsRequired);
			progressText.text = displayedProgress.ToString() + " / " + gamePointsRequired;
			if (unlockingTier)
			{
				fromProgress = tierProgress;
				lockImage.gameObject.SetActive(value: false);
				unlockImage.gameObject.SetActive(value: true);
				float lockLerpTimer = 0f;
				while (unlockImage.gameObject.activeInHierarchy)
				{
					unlockImage.alpha = Mathf.Lerp(1f, 0f, lockLerpTimer / lockFadeLerpDuration);
					lockLerpTimer += Time.deltaTime;
					if (lockLerpTimer >= lockFadeLerpDuration)
					{
						break;
					}
					yield return null;
				}
				current++;
				if (current == to)
				{
					stopped = true;
				}
				else if (current != 3)
				{
					tierProgressBar.Progress = 0f;
					timer = 0f;
					lockImage.gameObject.SetActive(value: true);
					unlockImage.gameObject.SetActive(value: false);
				}
			}
			if (totalProgress >= 1f)
			{
				break;
			}
			yield return null;
		}
		IsShowingTierProgress = false;
		yield return null;
	}

	private void Update()
	{
		if (!unlockingTier)
		{
			timer += Time.deltaTime;
			crystalTimer += Time.deltaTime;
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

	private int ReduceGamePointsWithPreviousTierRequirements(GamePassTier gamePassTierToDisplay, int gamePoints, Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus)
	{
		for (int num = (int)(gamePassTierToDisplay - 1); num > 0; num--)
		{
			GamePassTier key = (GamePassTier)num;
			gamePoints -= gameTierShopStatus[key].gamePointRequirementBase;
			gamePoints = Mathf.Max(gamePoints, 0);
		}
		return gamePoints;
	}
}
