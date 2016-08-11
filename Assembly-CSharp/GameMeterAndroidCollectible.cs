using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidCollectible : GameMeterAndroidBase
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
		if (collectedClient != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
			UpdateValue();
		}
		else
		{
			Hide();
		}
	}

	public override void UpdateValue()
	{
		if (collectedClient != null)
		{
			int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible);
			if (prevValue != gameStat && gameStat != 0)
			{
				prevValue = gameStat;
				NotificationController.PushNotification(string.Format(TM._("Collected star! {0}/{1}"), gameStat, collectedClient.Limit));
			}
			string text = gameStat + "/" + collectedClient.Limit;
			collectibleText.text = text;
		}
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
