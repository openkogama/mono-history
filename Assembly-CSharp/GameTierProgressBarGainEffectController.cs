using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTierProgressBarGainEffectController : MonoBehaviour
{
	[Serializable]
	private class TierTargetData
	{
		public Transform gamePointEffectTargetTransform;

		public float tierScaleEffectStartTime;
	}

	[SerializeField]
	private Transform gamePointEffectContainer;

	[SerializeField]
	private GamePointGainEffect gamePointGainEffectPrefab;

	[SerializeField]
	private GameTierProgressBar tierProgressBar;

	[SerializeField]
	private float offsetDirectionXMin;

	[SerializeField]
	private float offsetDirectionXMax;

	[SerializeField]
	private float offsetDirectionYMin;

	[SerializeField]
	private float offsetDirectionYMax;

	[SerializeField]
	private List<TierTargetData> targetDataList = new List<TierTargetData>();

	[SerializeField]
	private AnimationCurve onHitScaleEffect;

	private bool isInitialized;

	private int currentGamePoints;

	private int gamePointsToInstantiate;

	private float createGamePointTime;

	private const float createGamePointDelayMax = 0.2f;

	private const float createGamePointDelayMin = 0.1f;

	private const int maxAmountOfQueuedGainEffects = 10;

	private List<GamePointGainEffect> gamePointGainEffectPool = new List<GamePointGainEffect>();

	private List<GamePointGainEffect> gamePointGainEffectCurrentlyUsed = new List<GamePointGainEffect>();

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			if (GamePassesManager.GamePassesActive)
			{
				currentGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
			}
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			GamePointGainEffectManager.OnGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnGamePointGainEffectShown, new Action<int>(OnHaveShownGainEffect));
			GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown, new Action<int>(OnHaveShownTierProgressBarGainEffect));
			for (int i = 0; i < 5; i++)
			{
				CreateGamePointGainEffect();
			}
		}
	}

	private void Start()
	{
		Initialize();
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		GamePointGainEffectManager.OnGamePointGainEffectShown = (Action<int>)Delegate.Remove(GamePointGainEffectManager.OnGamePointGainEffectShown, new Action<int>(OnHaveShownGainEffect));
		GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown = (Action<int>)Delegate.Remove(GamePointGainEffectManager.OnTierProgressBarGamePointGainEffectShown, new Action<int>(OnHaveShownTierProgressBarGainEffect));
	}

	private void Update()
	{
		if (gamePointsToInstantiate > 0 && createGamePointTime < Time.time)
		{
			StartGamePointGainEffect();
		}
		HandleScaleEffect();
	}

	private void HandleScaleEffect()
	{
		for (int i = 0; i < targetDataList.Count; i++)
		{
			float time = Time.time - targetDataList[i].tierScaleEffectStartTime;
			float num = onHitScaleEffect.Evaluate(time);
			targetDataList[i].gamePointEffectTargetTransform.localScale = new Vector3(num, num, num);
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (GamePassesManager.GamePassesActive && GamePassesManager.playerTierStateCalculator != null && GamePassesManager.playerTierStateCalculator.gamePassRewardsActivated)
		{
			int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
			if (progressionGamePoints > currentGamePoints)
			{
				HandleNewGamePointAmount(progressionGamePoints);
			}
		}
	}

	private void OnHaveShownGainEffect(int gamePointAmountShown)
	{
		currentGamePoints = gamePointAmountShown;
		StopAllGainEffects();
	}

	private void OnHaveShownTierProgressBarGainEffect(int gamePointAmountShown)
	{
		if (!gameObject.activeInHierarchy)
		{
			OnHaveShownGainEffect(gamePointAmountShown);
		}
	}

	private void HandleNewGamePointAmount(int newGamePointsAmount)
	{
		gamePointsToInstantiate += newGamePointsAmount - currentGamePoints;
		if (gamePointsToInstantiate > 10)
		{
			gamePointsToInstantiate = 10;
		}
		currentGamePoints = newGamePointsAmount;
		if (Time.time > createGamePointTime)
		{
			StartGamePointGainEffect();
		}
	}

	private void StartGamePointGainEffect()
	{
		if (gamePointGainEffectPool.Count == 0)
		{
			CreateGamePointGainEffect();
		}
		GamePointGainEffect gamePointGainEffect = gamePointGainEffectPool[gamePointGainEffectPool.Count - 1];
		gamePointGainEffectPool.RemoveAt(gamePointGainEffectPool.Count - 1);
		gamePointGainEffect.transform.localPosition = Vector3.zero;
		gamePointGainEffect.StartEffect(GetTargetTransform(), offsetDirectionXMin, offsetDirectionXMax, offsetDirectionYMin, offsetDirectionYMax);
		gamePointGainEffect.gameObject.SetActive(value: true);
		gamePointGainEffectCurrentlyUsed.Add(gamePointGainEffect);
		gamePointsToInstantiate--;
		createGamePointTime = Time.time + UnityEngine.Random.Range(0.1f, 0.2f);
	}

	private void CreateGamePointGainEffect()
	{
		GamePointGainEffect gamePointGainEffect = UnityEngine.Object.Instantiate(gamePointGainEffectPrefab);
		gamePointGainEffect.gameObject.SetActive(value: false);
		gamePointGainEffect.transform.SetParent(gamePointEffectContainer, worldPositionStays: false);
		gamePointGainEffect.Initialize(OnGamePointReached, gamePointGainEffectPool.Count + gamePointGainEffectCurrentlyUsed.Count);
		gamePointGainEffectPool.Add(gamePointGainEffect);
	}

	private void OnGamePointReached(int id)
	{
		for (int i = 0; i < gamePointGainEffectCurrentlyUsed.Count; i++)
		{
			if (gamePointGainEffectCurrentlyUsed[i].ID == id)
			{
				GamePointGainEffect gamePointGainEffect = gamePointGainEffectCurrentlyUsed[i];
				gamePointGainEffect.gameObject.SetActive(value: false);
				gamePointGainEffectPool.Add(gamePointGainEffect);
				gamePointGainEffectCurrentlyUsed.RemoveAt(i);
				int value = (int)(tierProgressBar.GetCurrentTier() - 1);
				value = Mathf.Clamp(value, 0, targetDataList.Count - 1);
				targetDataList[value].tierScaleEffectStartTime = Time.time;
			}
		}
	}

	private Transform GetTargetTransform()
	{
		int value = (int)(tierProgressBar.GetCurrentTier() - 1);
		value = Mathf.Clamp(value, 0, targetDataList.Count - 1);
		return targetDataList[value].gamePointEffectTargetTransform;
	}

	private void StopAllGainEffects()
	{
		gamePointsToInstantiate = 0;
		for (int num = gamePointGainEffectCurrentlyUsed.Count - 1; num >= 0; num--)
		{
			GamePointGainEffect gamePointGainEffect = gamePointGainEffectCurrentlyUsed[num];
			gamePointGainEffect.gameObject.SetActive(value: false);
			gamePointGainEffectPool.Add(gamePointGainEffect);
			gamePointGainEffectCurrentlyUsed.RemoveAt(num);
		}
	}
}
