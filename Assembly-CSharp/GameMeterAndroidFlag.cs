using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidFlag : GameMeterAndroidBase
{
	[SerializeField]
	private Image flagBar;

	public override GameMeterType GameMeterType => GameMeterType.Flag;

	private void Start()
	{
		UpdateShowGameMeter();
	}

	public override void UpdateShowGameMeter()
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

	public override void SetShowGameMeter(bool show)
	{
		flagBar.enabled = show;
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
		flagBar.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
		flagBar.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
	}
}
