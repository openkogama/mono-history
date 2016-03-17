using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidGameCoin : GameMeterAndroidBase
{
	[SerializeField]
	private Image gameCoinBar;

	[SerializeField]
	private RollingNumberCounterAndroid counter;

	public override GameMeterType GameMeterType => GameMeterType.GameCoins;

	private void Start()
	{
		MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager.OnActivationChange = (MVGameCoinManager.OnActivationChangeDelegate)Delegate.Combine(gameCoinManager.OnActivationChange, new MVGameCoinManager.OnActivationChangeDelegate(OnActivationChange));
		MVGameCoinManager gameCoinManager2 = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager2.OnGameCoinAmountChange = (MVGameCoinManager.OnGameCoinAmountChangeDelegate)Delegate.Combine(gameCoinManager2.OnGameCoinAmountChange, new MVGameCoinManager.OnGameCoinAmountChangeDelegate(OnGameCoinAmountChange));
		OnActivationChange(MVGameControllerBase.Game.GameCoinManager.Active);
	}

	public override void UpdateShowGameMeter()
	{
	}

	public void OnActivationChange(bool wantToShow)
	{
		MeterActive = wantToShow;
		if (MeterActive)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void OnGameCoinAmountChange(int amount)
	{
		counter.SetCounter(amount);
	}

	public override void SetShowGameMeter(bool show)
	{
		gameCoinBar.enabled = show;
		counter.enabled = show;
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
		gameCoinBar.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
		gameCoinBar.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
	}
}
