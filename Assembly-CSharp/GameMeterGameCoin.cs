using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterGameCoin : GameMeterBase
{
	[SerializeField]
	private Image gameCoinBar;

	[SerializeField]
	private GameObject coinAmount;

	[SerializeField]
	private RollingNumberCounterAndroid counter;

	public override GameMeterType GameMeterType => GameMeterType.GameCoins;

	private void Start()
	{
		MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager.OnActivationChange = (MVGameCoinManager.OnActivationChangeDelegate)Delegate.Combine(gameCoinManager.OnActivationChange, new MVGameCoinManager.OnActivationChangeDelegate(OnActivationChange));
		MVGameCoinManager gameCoinManager2 = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager2.OnGameCoinAmountChange = (MVGameCoinManager.OnGameCoinAmountChangeDelegate)Delegate.Combine(gameCoinManager2.OnGameCoinAmountChange, new MVGameCoinManager.OnGameCoinAmountChangeDelegate(OnGameCoinAmountChanged));
	}

	public override void SetGameMeterVisibility()
	{
		OnActivationChange(MVGameControllerBase.Game.GameCoinManager.Active);
	}

	private void OnGameCoinAmountChanged(int amount)
	{
		counter.SetCounter(amount);
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
			gameCoinManager.OnActivationChange = (MVGameCoinManager.OnActivationChangeDelegate)Delegate.Remove(gameCoinManager.OnActivationChange, new MVGameCoinManager.OnActivationChangeDelegate(OnActivationChange));
		}
	}

	public void OnActivationChange(bool wantToShow)
	{
		MeterActive = wantToShow;
		if (wantToShow)
		{
			Show();
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
		gameCoinBar.enabled = show;
		coinAmount.SetActive(show);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
	}
}
