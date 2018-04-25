using System;
using System.Collections.Generic;
using MV.Common;
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
		MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged += OnGameStatUpdated;
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionAddedOrRemoved += ConditionCountChanged;
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(UpdateValue));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged -= CounterChanged;
			MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged -= OnGameStatUpdated;
			MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionAddedOrRemoved -= ConditionCountChanged;
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(UpdateValue));
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

	private void OnGameStatUpdated(object sender, OnCounterTypeChangedArgs args)
	{
		WinningConditionNotificationManager.UpdateNotification(args.actorNumber, args.counterType, args.count);
		if (args.actorNumber != MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr)
		{
			return;
		}
		GameStatCounterType counterType = args.counterType;
		if (counterType == GameStatCounterType.Kill)
		{
			WinningConditionControl.TryGetPrioritizedStat(out var statType);
			if (statType == GameStatCounterType.Kill)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)1, TM._("Score +1"));
				NotificationController.PushNotification(NotificationType.KillPrimary, dictionary, NotificationLifetime.Low);
			}
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
