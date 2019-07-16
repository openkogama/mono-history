using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.Subscription;
using MV.WorldObject.Subscription.SubscriptionRules;
using UnityEngine;

public class MVGameCoinManager
{
	public delegate void OnActivationChangeDelegate(bool active);

	public delegate void OnGameCoinAmountChangeDelegate(int amount);

	private ObscuredFloat startTime = 0f;

	private ObscuredFloat interval = 2f;

	private ObscuredInt intervalAmount = 1;

	private int currentBoostMultiplier = 1;

	private ObscuredBool boostEnabled;

	private ObscuredFloat boostedInterval = 2f;

	private ObscuredInt gameCoinPickupValue = 25;

	private ObscuredInt gameCoins = 0;

	private ObscuredBool isActive = false;

	private ObscuredInt totalPurchaseAmount = 0;

	public OnActivationChangeDelegate OnActivationChange;

	public OnGameCoinAmountChangeDelegate OnGameCoinAmountChange;

	public Action<bool> BoostStateChanged;

	public int GameCoinAmount => gameCoins;

	public int TotalPurchaseAmount => totalPurchaseAmount;

	public bool Active => isActive;

	public bool BoostEnabled => boostEnabled;

	public MVGameCoinManager()
	{
		if (MVGameControllerBase.Game.MVPlayerContainer.Count == 0 || !MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
		}
		else
		{
			Initialize();
		}
	}

	public void OnGameBoostChanged(bool boostEnabled)
	{
		this.boostEnabled = boostEnabled;
		if (boostEnabled)
		{
			boostedInterval = (float)interval / (float)MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.GetRule<GameCoinBooster>(SubscriptionBenefit.GameCoinBoost).GetBoostedGameCoins(1);
		}
		else
		{
			boostedInterval = interval;
		}
		if (BoostStateChanged != null)
		{
			BoostStateChanged(boostEnabled);
		}
	}

	public void Update(MVNetworkGame game)
	{
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
			gameCoins = (int)gameCoins + GetBoostedGameCoinCount(intervalAmount);
			if (OnGameCoinAmountChange != null)
			{
				OnGameCoinAmountChange(gameCoins);
			}
			startTime = Time.time;
		}
	}

	private int GetBoostedGameCoinCount(int defaultAmount)
	{
		return defaultAmount * currentBoostMultiplier;
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
		ObscuredInt obscuredInt = gameCoinPickupValue;
		gameCoins = (int)gameCoins + GetBoostedGameCoinCount(obscuredInt);
		if (OnGameCoinAmountChange != null)
		{
			OnGameCoinAmountChange(gameCoins);
		}
	}

	public void GameCoinChestCollect(int amount)
	{
		gameCoins = (int)gameCoins + GetBoostedGameCoinCount(amount);
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
		int count = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.GameCoin).Count;
		int count2 = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.GameCoinChest).Count;
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
			MVGameControllerBase.Game.LocalPlayer.BoostController.AllowBoost(BoostType.GameCoinsIntMultiplier, active);
		}
	}

	private void LateInitialize()
	{
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
		Initialize();
	}

	private void Initialize()
	{
		boostEnabled = MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.GameCoinBoost);
		OnGameBoostChanged(boostEnabled);
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.GameCoinsIntMultiplier, OnGameCoinBoostChanged);
		OnGameCoinBoostChanged();
	}

	private void OnGameCoinBoostChanged()
	{
		currentBoostMultiplier = 1;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.GameCoinsIntMultiplier, out var _))
		{
			currentBoostMultiplier = 2;
		}
	}
}
