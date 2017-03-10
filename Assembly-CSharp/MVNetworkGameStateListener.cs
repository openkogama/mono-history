using System;
using MV.Common;

public class MVNetworkGameStateListener
{
	private int startTime;

	private int duration;

	private MVGameStateType currentGameState;

	private int timeLeft;

	private MVGameStateReason lastReason;

	private int lastInstigatorActorNr;

	public int TimeLeftMS => timeLeft;

	public MVGameStateType CurrentGameState => currentGameState;

	public MVGameStateReason LastReason => lastReason;

	public int LastInstigatorActorNr => lastInstigatorActorNr;

	public int StartTime => startTime;

	public event EventHandler<GameStateChangeEventArgs> OnGameStateChanged;

	public void ChangeState(MVNetworkGame game, MVGameStateType gameStateType, int startTime, int duration, MVGameStateReason reason, int actorNr)
	{
		currentGameState = gameStateType;
		this.startTime = startTime;
		this.duration = duration;
		timeLeft = duration - (game.ServerTimeInMilliSeconds - startTime);
		lastReason = reason;
		lastInstigatorActorNr = actorNr;
		if (OnGameStateChanged != null)
		{
			OnGameStateChanged(this, new GameStateChangeEventArgs(actorNr, reason));
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
