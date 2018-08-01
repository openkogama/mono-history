using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterTimeAttackFlag : GameMeterBase
{
	[SerializeField]
	private Image timeAttackFlagBar;

	[SerializeField]
	private Text timeAttackFlagText;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.TimeAttackFlag;

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
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition != WinningConditionType.TimeAttackFlag)
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
		int num = ((MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.TimeAttackFlag) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.TimeAttackFlag, MVGameControllerBase.Game.LocalPlayer.Team));
		if (prevValue != num && num != 0)
		{
			for (int i = 0; i < gameMeterVisualEffects.Count; i++)
			{
				gameMeterVisualEffects[i].ExecuteEffect();
			}
			prevValue = num;
		}
		string text = WinningConditionControl.MakeIntoScoreText(num, GameStatCounterType.TimeAttackFlag);
		timeAttackFlagText.text = text;
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
		timeAttackFlagBar.enabled = show;
		timeAttackFlagText.enabled = show;
	}
}
