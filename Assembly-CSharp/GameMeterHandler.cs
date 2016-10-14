using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMeterHandler : MonoBehaviour
{
	[SerializeField]
	private List<GameMeterBase> gameMeters;

	private void OnEnable()
	{
		UpdateValue();
	}

	private void Awake()
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].SetGameMeterVisibility();
		}
		MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged += CounterChanged;
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionCountChanged += ConditionCountChanged;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdateValue));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged -= CounterChanged;
			MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionCountChanged -= ConditionCountChanged;
			MVNetworkGame game = MVGameControllerBase.Game;
			game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Remove(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdateValue));
		}
	}

	private void UpdateValue()
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].UpdateValue();
		}
	}

	private void ConditionCountChanged(object sender, EventArgs args)
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].SetGameMeterVisibility();
		}
	}

	private void CounterChanged(object sender, OnCounterTypeChangedArgs args)
	{
		if (MVGameControllerBase.Game.GameStatCounterManager.ActiveTeams.Count > 1 || args.actorNumber == MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr)
		{
			UpdateValue();
		}
	}
}
