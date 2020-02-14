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

	private GamePassTier nextTier;

	private int crystalValue;

	private float currentCrystalValue;

	private float timer = -0.5f;

	private float timeBeforeStartLerp = -0.5f;

	private float fromProgress;

	private float toProgress;

	private int currentGamePoints;

	private int gamePointsRequired;

	private Vector3 lockStartRot;

	private float intensity;

	public void Initialize(GamePassTier currentTier)
	{
		nextTier = currentTier + 1;
		timer = timeBeforeStartLerp;
		int gamePointAmountShown = GamePointGainEffectManager.GamePointAmountShown;
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		crystalValue = progressionGamePoints - gamePointAmountShown;
		currentCrystalValue = crystalValue;
		crystalsGainedSinceDeath.text = crystalValue.ToString();
		Debug.Log("gamepoints: " + gamePointAmountShown + " / " + progressionGamePoints);
		nextTierText.text = ((int)(currentTier + 1)/*cast due to constrained. prefix*/).ToString();
		int progressionGamePoints2 = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints2, nextTier);
		currentGamePoints = ReduceGamePointsWithPreviousTierRequirements(nextTier, progressionGamePoints2, tierPricingState);
		gamePointsRequired = tierPricingState[nextTier].gamePointRequirementBase;
		currentGamePoints = Mathf.Clamp(currentGamePoints, 0, gamePointsRequired);
		fromProgress = Mathf.Clamp(currentGamePoints - crystalValue, 0, gamePointsRequired);
		toProgress = currentGamePoints;
		intensity = 0f;
		if (crystalValue != 0)
		{
			intensity = 1f + (float)crystalValue / (float)gamePointsRequired;
		}
		Debug.Log("crystals: " + crystalValue + " gamePointsRequired: " + gamePointsRequired);
		Debug.Log("intensity: " + intensity.ToString("N3"));
		lockStartRot = lockImage.localRotation.eulerAngles;
		GamePointGainEffectManager.HaveShownTierProgressBarGamePointGainEffect(progressionGamePoints2);
		GamePointGainEffectManager.HaveShownGamePointGainEffect(progressionGamePoints2);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		UpdateTierProgressBar();
	}

	private void UpdateTierProgressBar()
	{
		float num = timer / progressLerpDuration;
		float num2 = Mathf.Floor(Mathf.Lerp(fromProgress, toProgress, num));
		tierProgressBar.Progress = num2 / (float)gamePointsRequired;
		currentCrystalValue = Mathf.Ceil(Mathf.Lerp(crystalValue, 0f, num));
		crystalsGainedSinceDeath.text = currentCrystalValue.ToString();
		progressText.text = num2.ToString() + " / " + gamePointsRequired;
		if (lockImage.gameObject.activeInHierarchy)
		{
			float num3 = lockShakeCurve.Evaluate(num) * intensity;
			Vector3 euler = lockStartRot;
			euler.z = num3 * 180f;
			lockImage.localRotation = Quaternion.Euler(euler);
		}
		if (progressBarDivider.activeSelf && tierProgressBar.Progress <= 0f)
		{
			progressBarDivider.SetActive(value: false);
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
		}
		return gamePoints;
	}
}
