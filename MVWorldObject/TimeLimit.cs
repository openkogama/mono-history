using System;

public class TimeLimit : WinningCondition
{
	private GameStatCounterType counterType;

	public GameStatCounterType CounterType
	{
		get
		{
			return counterType;
		}
		set
		{
			counterType = value;
		}
	}

	public override HighScores HighScores => GetHighScores();

	public override bool IsSingleton => true;

	private HighScores GetHighScores()
	{
		return CounterType switch
		{
			GameStatCounterType.Collectible => gameCounterManager.GetHighScores(GameStatCounterType.Collectible, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.OneWinner, byAscending: false), 
			GameStatCounterType.YDown => gameCounterManager.GetHighScores(GameStatCounterType.YDown, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.OneWinner, byAscending: true), 
			GameStatCounterType.YUp => gameCounterManager.GetHighScores(GameStatCounterType.YUp, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.OneWinner, byAscending: false), 
			GameStatCounterType.Kill => gameCounterManager.GetHighScores(GameStatCounterType.Kill, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.OneWinner, byAscending: false), 
			GameStatCounterType.OculusKill => gameCounterManager.GetHighScores(GameStatCounterType.OculusKill, gameCounterManager.ActiveTeams.Count > 1, WinningConditionPresentStyle.OneWinner, byAscending: false), 
			_ => base.HighScores, 
		};
	}

	public TimeLimit(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, GameStatCounterType counterType)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: true, GameStatCounterType.Time, WinningConditionPresentStyle.MultipleWinners)
	{
		this.counterType = counterType;
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
		if (e.count <= Limit)
		{
			SendWinningConditionChangedEvent(new EventArgs(), e);
		}
	}
}
