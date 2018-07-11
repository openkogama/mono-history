using System;
using MV.Common;
using UnityEngine;

public class MVNetworkGameStateListener
{
	private int startTime;

	private int duration;

	private MVGameStateType currentGameState;

	private int timeLeft;

	public int TimeLeftMS => timeLeft;

	public int CountdownInSeconds => (int)Mathf.Ceil(MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS / 1000);

	public float CountdownInPercentage => (float)timeLeft / (float)duration;

	public MVGameStateType CurrentGameState => currentGameState;

	public int StartTime => startTime;

	public event EventHandler<GameStateChangeEventArgs> OnGameStateChanged;

	public void ChangeState(MVGameStateType gameStateType, int startTime, int duration, bool fromGameSnapshot)
	{
		currentGameState = gameStateType;
		this.startTime = startTime;
		this.duration = duration;
		timeLeft = duration - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - startTime);
		if (!fromGameSnapshot && OnGameStateChanged != null)
		{
			OnGameStateChanged(this, new GameStateChangeEventArgs());
		}
	}

	public void Update(MVNetworkGame game)
	{
		if (currentGameState == MVGameStateType.None)
		{
			return;
		}
		if (timeLeft > 0)
		{
			timeLeft = duration - (game.ServerTimeInMilliSeconds - startTime);
			if (timeLeft < 0)
			{
				timeLeft = 0;
			}
		}
		else
		{
			timeLeft = 0;
		}
	}
}
