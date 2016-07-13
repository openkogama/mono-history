using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMeterHandler : MonoBehaviour
{
	[SerializeField]
	private List<GameMeterAndroidBase> gameMeters;

	private void Start()
	{
		for (int i = 0; i < gameMeters.Count; i++)
		{
			gameMeters[i].SetGameMeterVisibility();
		}
		MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged += CounterChanged;
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionCountChanged += ConditionCountChanged;
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
		if (args.actorNumber == MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr)
		{
			for (int i = 0; i < gameMeters.Count; i++)
			{
				gameMeters[i].UpdateValue();
			}
		}
	}
}
