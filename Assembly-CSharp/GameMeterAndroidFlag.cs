using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidFlag : GameMeterAndroidBase
{
	[SerializeField]
	private Image flagBar;

	public override GameMeterType GameMeterType => GameMeterType.Flag;

	private void Start()
	{
		SetGameMeterVisibility();
	}

	public override void SetGameMeterVisibility()
	{
		FlagReachedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>();
		if (singletonWinnerConditionByType != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
		}
		else
		{
			Hide();
		}
	}

	public override void UpdateValue()
	{
	}

	public override void SetShowGameMeter(bool show)
	{
		flagBar.enabled = show;
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
