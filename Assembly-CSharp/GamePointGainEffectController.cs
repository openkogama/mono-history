using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class GamePointGainEffectController : MonoBehaviour
{
	[SerializeField]
	private Transform gamePointEffectContainer;

	[SerializeField]
	private Transform gamePointEffectTargetTransform;

	[SerializeField]
	private GamePointGainEffectCountController countController;

	[SerializeField]
	private GamePointGainEffect gamePointGainEffectPrefab;

	private int currentGamePoints;

	private int gamePointsToInstantiate;

	private float createGamePointTime;

	private const float createGamePointDelayMax = 0.2f;

	private const float createGamePointDelayMin = 0.1f;

	private const int maxAmountOfQueuedGainEffects = 10;

	private Vector3 gainEffectSpawnOffset = new Vector3(0f, -250f, 1f);

	private List<GamePointGainEffect> gamePointGainEffectPool = new List<GamePointGainEffect>();

	private List<GamePointGainEffect> gamePointGainEffectCurrentlyUsed = new List<GamePointGainEffect>();

	private void Start()
	{
		if (MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled))
		{
			gameObject.SetActive(value: false);
			gamePointEffectContainer.gameObject.SetActive(value: false);
		}
		if (GamePassesManager.GamePassesActive)
		{
			currentGamePoints = GamePassesManager.PlayerPlanetData.highScoreGamePoints;
		}
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		FakeGamePointGainEffectManager.OnFakeGamePointGainEffect = (Action<int>)Delegate.Combine(FakeGamePointGainEffectManager.OnFakeGamePointGainEffect, new Action<int>(OnFakeGainEffect));
		GamePointGainEffectManager.OnGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnGamePointGainEffectShown, new Action<int>(OnHaveShownGainEffect));
		GamePointGainEffectManager.OnInGamePointGainEffectShown = (Action<int>)Delegate.Combine(GamePointGainEffectManager.OnInGamePointGainEffectShown, new Action<int>(OnIngameGainEffectShown));
		for (int i = 0; i < 5; i++)
		{
			CreateGamePointGainEffect();
		}
	}

	private void OnDestroy()
	{
		FakeGamePointGainEffectManager.OnFakeGamePointGainEffect = (Action<int>)Delegate.Remove(FakeGamePointGainEffectManager.OnFakeGamePointGainEffect, new Action<int>(OnFakeGainEffect));
	}

	private void OnDisable()
	{
		StopAllGainEffects();
	}

	private void Update()
	{
		if (gamePointsToInstantiate > 0 && createGamePointTime < Time.time)
		{
			StartGamePointGainEffect();
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (GamePassesManager.GamePassesActive)
		{
			int highScoreGamePoints = GamePassesManager.PlayerPlanetData.highScoreGamePoints;
			if (highScoreGamePoints > currentGamePoints)
			{
				HandleNewGamePointAmount(highScoreGamePoints);
			}
			GamePointGainEffectManager.OnInGamePointGainEffectShown(highScoreGamePoints);
		}
	}

	private void OnFakeGainEffect(int newGamePoints)
	{
		HandleAddedGamePoints(newGamePoints);
	}

	private void OnHaveShownGainEffect(int gamePointAmountShown)
	{
		currentGamePoints = gamePointAmountShown;
		countController.UpdateGamePointAmountTextToValue(currentGamePoints);
		StopAllGainEffects();
		countController.HideCount();
	}

	private void OnIngameGainEffectShown(int gamePointAmountShown)
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
		countController.SetupAmountCatchingUp(newGamePointsAmount, gamePointsToInstantiate + gamePointGainEffectCurrentlyUsed.Count);
		if (Time.time > createGamePointTime)
		{
			StartGamePointGainEffect();
		}
	}

	private void HandleAddedGamePoints(int newAddedGamePoints)
	{
		gamePointsToInstantiate += newAddedGamePoints;
		if (gamePointsToInstantiate > 10)
		{
			gamePointsToInstantiate = 10;
		}
		currentGamePoints += newAddedGamePoints;
		countController.SetupAmountCatchingUp(currentGamePoints, gamePointsToInstantiate + gamePointGainEffectCurrentlyUsed.Count);
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
		gamePointGainEffect.transform.localPosition = gainEffectSpawnOffset;
		gamePointGainEffect.StartEffect(gamePointEffectTargetTransform);
		gamePointGainEffect.gameObject.SetActive(value: true);
		gamePointGainEffectCurrentlyUsed.Add(gamePointGainEffect);
		gamePointsToInstantiate--;
		createGamePointTime = Time.time + UnityEngine.Random.Range(0.1f, 0.2f);
		countController.OnGamePointGainEffectStarted();
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
				countController.OnGamePointGainEffectReached();
			}
		}
		if (gamePointGainEffectCurrentlyUsed.Count <= 0 && gamePointsToInstantiate <= 0)
		{
			OnGainEffectsDone();
		}
	}

	private void OnGainEffectsDone()
	{
		int highScoreGamePoints = GamePassesManager.PlayerPlanetData.highScoreGamePoints;
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit || MVGameControllerBase.IsTouristSession)
		{
			highScoreGamePoints = currentGamePoints;
		}
		countController.UpdateGamePointAmountTextToValue(highScoreGamePoints);
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
