using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterTimeAttackFlag : GameMeterBase
{
	[SerializeField]
	private Image timeAttackFlagBar;

	[SerializeField]
	private Text timeAttackFlagText;

	private bool shouldUpdate;

	public override GameMeterType GameMeterType => GameMeterType.TimeAttackFlag;

	private void Start()
	{
		SetGameMeterVisibility();
		timeAttackFlagText.text = "00:00:00";
		FlagDebriefingControl.OnFlagDebriefing = (Action<int>)Delegate.Combine(FlagDebriefingControl.OnFlagDebriefing, new Action<int>(OnStartFlagCountdown));
		FlagDebriefingControl.OnFlagCountDown = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagCountDown, new Action(OnStartFlagCountdown));
		FlagDebriefingControl.OnFlagCountDownEnd = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagCountDownEnd, new Action(OnEndFlagCountdown));
	}

	private void Update()
	{
		if (shouldUpdate)
		{
			int score = Mathf.FloorToInt((Time.time - FlagDebriefingControl.RunStartTime) * 1000f);
			string text = WinningConditionControl.MakeIntoScoreText(score, GameStatCounterType.TimeAttackFlag);
			timeAttackFlagText.text = text;
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

	private void OnStartFlagCountdown()
	{
		shouldUpdate = false;
		timeAttackFlagText.text = "00:00:00";
	}

	private void OnStartFlagCountdown(int captureTime)
	{
		OnStartFlagCountdown();
	}

	private void OnEndFlagCountdown()
	{
		shouldUpdate = true;
	}
}
