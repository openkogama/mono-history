using System;
using MV.Common;
using UnityEngine;

public class GameTierProgressBarRoundEndController : MonoBehaviour
{
	[SerializeField]
	private GameTierProgressBar tierProgressBar;

	[SerializeField]
	private GameTierProgressBarGainEffectController gainEffectController;

	[SerializeField]
	private GameObject inGameUIContent;

	private bool hasRoundEnded;

	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		tierProgressBar.Initialize();
		gainEffectController.Initialize();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		}
	}

	private void Update()
	{
		if (!hasRoundEnded)
		{
			return;
		}
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
		{
			hasRoundEnded = false;
			if (tierProgressBar.gameObject.activeSelf)
			{
				tierProgressBar.gameObject.SetActive(value: false);
			}
			if (!inGameUIContent.activeSelf)
			{
				inGameUIContent.SetActive(value: true);
			}
		}
		else if (MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS <= 3000)
		{
			if (!tierProgressBar.gameObject.activeSelf)
			{
				tierProgressBar.gameObject.SetActive(value: true);
			}
			if (inGameUIContent.activeSelf)
			{
				inGameUIContent.SetActive(value: false);
			}
		}
	}

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		if (GamePassesManager.GamePassesActive)
		{
			hasRoundEnded = true;
		}
	}
}
