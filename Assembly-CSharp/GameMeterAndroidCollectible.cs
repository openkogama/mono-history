using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidCollectible : GameMeterAndroidBase
{
	[SerializeField]
	private Image collectibleBar;

	[SerializeField]
	private Text collectibleText;

	public override GameMeterType GameMeterType => GameMeterType.Collectibles;

	private void Start()
	{
		UpdateShowGameMeter();
	}

	public override void UpdateShowGameMeter()
	{
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (singletonWinnerConditionByType != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
			string text = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) + "/" + singletonWinnerConditionByType.Limit;
			collectibleText.text = text;
		}
		else
		{
			Hide();
			collectibleText.text = string.Empty;
		}
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
		collectibleBar.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
		collectibleText.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
		collectibleBar.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
		collectibleText.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
	}

	public override void SetShowGameMeter(bool show)
	{
		collectibleBar.enabled = show;
		collectibleText.enabled = show;
	}
}
