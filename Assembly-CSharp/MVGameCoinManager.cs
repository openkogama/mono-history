using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGameCoinManager
{
	public delegate void OnActivationChangeDelegate(bool active);

	public delegate void OnGameCoinAmountChangeDelegate(int amount);

	private ObscuredFloat startTime = 0f;

	private ObscuredFloat interval = 2f;

	private ObscuredInt intervalAmount = 1;

	private ObscuredInt boostLeft;

	private ObscuredBool boostEnabled;

	private ObscuredInt prevBoostTime;

	private ObscuredFloat boostedInterval = 2f;

	private ObscuredInt gameCoinPickupValue = 25;

	private ObscuredInt gameCoins = 0;

	private ObscuredBool isActive = false;

	private ObscuredInt totalPurchaseAmount = 0;

	public OnActivationChangeDelegate OnActivationChange;

	public OnGameCoinAmountChangeDelegate OnGameCoinAmountChange;

	public Action<int, bool> BoostStateChanged;

	public int GameCoinAmount => gameCoins;

	public int TotalPurchaseAmount => totalPurchaseAmount;

	public bool Active => isActive;

	public int BoostLeft => boostLeft;

	public bool BoostEnabled => boostEnabled;

	public MVGameCoinManager(int boostLeft)
	{
		this.boostLeft = boostLeft;
	}

	public void OnGameBoostChanged(int boostLeft, bool boostEnabled)
	{
		this.boostLeft = boostLeft;
		this.boostEnabled = boostEnabled;
		if (boostEnabled)
		{
			prevBoostTime = WaitForTicks.GetEnvironmentTick(0);
			boostedInterval = 0.25f;
		}
		else
		{
			boostedInterval = interval;
		}
		if (BoostStateChanged != null)
		{
			BoostStateChanged(boostLeft, boostEnabled);
		}
	}

	public void Update(MVNetworkGame game)
	{
		if ((bool)boostEnabled)
		{
			int num = WaitForTicks.Diff(prevBoostTime);
			prevBoostTime = WaitForTicks.GetEnvironmentTick(0);
			boostLeft = (int)boostLeft - num;
			boostLeft = Math.Max(0, boostLeft);
		}
		if (!isActive)
		{
			return;
		}
		if (!game.IsPlaying || game.NetworkGameStateListener.CurrentGameState != MVGameStateType.Round)
		{
			startTime = Time.time;
		}
		else if (Time.time - (float)startTime > (float)boostedInterval)
		{
			gameCoins = (int)gameCoins + (int)intervalAmount;
			if (OnGameCoinAmountChange != null)
			{
				OnGameCoinAmountChange(gameCoins);
			}
			startTime = Time.time;
		}
	}

	public void Reset(MVNetworkGame game)
	{
		startTime = Time.time;
		gameCoins = 0;
		if (OnGameCoinAmountChange != null)
		{
			OnGameCoinAmountChange(gameCoins);
		}
		if (OnActivationChange != null)
		{
			OnActivationChange(isActive);
		}
	}

	public void GameCoinCollect()
	{
		gameCoins = (int)gameCoins + (int)gameCoinPickupValue;
		if (OnGameCoinAmountChange != null)
		{
			OnGameCoinAmountChange(gameCoins);
		}
	}

	public void GameCoinChestCollect(int amount)
	{
		gameCoins = (int)gameCoins + amount;
		if (OnGameCoinAmountChange != null)
		{
			OnGameCoinAmountChange(gameCoins);
		}
	}

	public bool Consume(GameCoinLogic gameCoinLogic)
	{
		if ((int)gameCoins < gameCoinLogic.PurchaseAmount)
		{
			return false;
		}
		gameCoins = (int)gameCoins - gameCoinLogic.PurchaseAmount;
		if (OnGameCoinAmountChange != null)
		{
			OnGameCoinAmountChange(gameCoins);
		}
		return true;
	}

	public void ReportPurchaseAmountInEditor(int amount)
	{
		totalPurchaseAmount = (int)totalPurchaseAmount + amount;
		Evaluate();
	}

	public void ReportPickupChangeInEditor()
	{
		Evaluate();
	}

	private void Evaluate()
	{
		int count = MVGameController.WOCM.GetWorldObjectsByType(WorldObjectType.GameCoin).Count;
		int count2 = MVGameController.WOCM.GetWorldObjectsByType(WorldObjectType.GameCoinChest).Count;
		if ((int)totalPurchaseAmount <= 0 && count <= 0 && count2 <= 0)
		{
			HandleActivationChange(active: false);
		}
		else
		{
			HandleActivationChange(active: true);
		}
	}

	private void HandleActivationChange(bool active)
	{
		if (active != (bool)isActive)
		{
			isActive = active;
			if (OnActivationChange != null)
			{
				OnActivationChange(isActive);
			}
		}
	}
}
