using System;
using UnityEngine;

public class GameCoinCounter : GameMeterBase
{
	[SerializeField]
	private RollingNumberCounter rollingNumberCounter;

	[SerializeField]
	private UXGroup rollingNumberGroup;

	[SerializeField]
	private UXPlane bg;

	public override bool MeterActive
	{
		get
		{
			return meterActive;
		}
		set
		{
			rollingNumberGroup.SetVisible(value);
			if (meterActive != value)
			{
				meterActive = value;
				if (meterActive)
				{
					bg.SetAlpha(1f, string.Empty);
				}
				else
				{
					bg.SetAlpha(inActiveAlpha, string.Empty);
				}
			}
		}
	}

	public override GameMeterType GameMeterType => GameMeterType.GameCoins;

	private void Start()
	{
		MVGameCoinManager gameCoinManager = MVGameController.Game.GameCoinManager;
		gameCoinManager.OnActivationChange = (MVGameCoinManager.OnActivationChangeDelegate)Delegate.Combine(gameCoinManager.OnActivationChange, new MVGameCoinManager.OnActivationChangeDelegate(OnActivationChange));
		MVGameCoinManager gameCoinManager2 = MVGameController.Game.GameCoinManager;
		gameCoinManager2.OnGameCoinAmountChange = (MVGameCoinManager.OnGameCoinAmountChangeDelegate)Delegate.Combine(gameCoinManager2.OnGameCoinAmountChange, new MVGameCoinManager.OnGameCoinAmountChangeDelegate(OnGameCoinAmountChange));
		OnActivationChange(MVGameController.Game.GameCoinManager.Active);
		rollingNumberCounter.Initialize();
		rollingNumberCounter.UseOverlay = false;
	}

	private void Update()
	{
		MeterActive = meterActive;
	}

	public void OnActivationChange(bool wantToShow)
	{
		MeterActive = wantToShow;
	}

	public void OnGameCoinAmountChange(int amount)
	{
		rollingNumberCounter.SetCounter(amount);
	}
}
