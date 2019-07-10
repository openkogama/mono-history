using UnityEngine;

public class GameMeterFlag : GameMeterBase
{
	[SerializeField]
	private GameObject flagBar;

	private FlagReachedClient flagClient;

	public override GameMeterType GameMeterType => GameMeterType.Flag;

	public override void Initialize()
	{
	}

	public override void SetGameMeterVisibility()
	{
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition != WinningConditionType.Flag)
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

	public override void SetShowGameMeter(bool show)
	{
		flagBar.SetActive(show);
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
	}
}
