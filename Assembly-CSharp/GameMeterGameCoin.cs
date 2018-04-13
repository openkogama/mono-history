using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterGameCoin : GameMeterBase
{
	[SerializeField]
	private Image gameCoinBar;

	[SerializeField]
	private GameObject coinAmount;

	public override GameMeterType GameMeterType => GameMeterType.GameCoins;

	private void Start()
	{
		MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager.OnActivationChange = (MVGameCoinManager.OnActivationChangeDelegate)Delegate.Combine(gameCoinManager.OnActivationChange, new MVGameCoinManager.OnActivationChangeDelegate(OnActivationChange));
	}

	public override void SetGameMeterVisibility()
	{
		OnActivationChange(MVGameControllerBase.Game.GameCoinManager.Active);
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
