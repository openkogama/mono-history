using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterCollectible : GameMeterBase
{
	[SerializeField]
	private Image collectibleBar;

	[SerializeField]
	private Text collectibleText;

	private int prevValue;

	private AllCollectiblesCollectedClient collectedClient;

	public override GameMeterType GameMeterType => GameMeterType.Collectibles;

	private void Start()
	{
		SetGameMeterVisibility();
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset += OnVictoryConditionMet;
	}

	private void OnVictoryConditionMet(object sender, EventArgs args)
	{
		UpdateValue();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset -= OnVictoryConditionMet;
		}
	}

	public override void SetGameMeterVisibility()
	{
		collectedClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition != WinningConditionType.Collectible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public override void UpdateValue()
	{
		if (collectedClient == null)
		{
			return;
		}
		int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible);
		progress.Progress = gameStat / collectedClient.Limit;
		if (prevValue != gameStat && gameStat != 0)
		{
			for (int i = 0; i < gameMeterVisualEffects.Count; i++)
			{
				gameMeterVisualEffects[i].ExecuteEffect();
			}
			prevValue = gameStat;
		}
		string text = gameStat + "/" + collectedClient.Limit;
		collectibleText.text = text;
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
	}

	public override void SetShowGameMeter(bool show)
	{
		collectibleBar.enabled = show;
		collectibleText.enabled = show;
	}
}
