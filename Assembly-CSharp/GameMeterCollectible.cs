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
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
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
		int num = ((MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.Collectible, MVGameControllerBase.Game.LocalPlayer.Team));
		if (prevValue != num && num != 0)
		{
			for (int i = 0; i < gameMeterVisualEffects.Count; i++)
			{
				gameMeterVisualEffects[i].ExecuteEffect();
			}
			prevValue = num;
		}
		string text = num + "/" + collectedClient.Limit;
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
