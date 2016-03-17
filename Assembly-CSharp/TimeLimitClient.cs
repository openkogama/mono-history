public class TimeLimitClient : TimeLimit, IWinningConditionBriefing
{
	public override bool IsBriefingNode => CounterType != GameStatCounterType.None;

	public TimeLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, GameStatCounterType counterType)
		: base(parent, id, gameCounterManager, counterType)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		switch (CounterType)
		{
		case GameStatCounterType.YUp:
			winningConditionBriefingView.AddBriefing(WinningConditionType.Highest);
			break;
		case GameStatCounterType.YDown:
			winningConditionBriefingView.AddBriefing(WinningConditionType.Lowest);
			break;
		}
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		switch (CounterType)
		{
		case GameStatCounterType.YUp:
			winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Highest, HighScores, IsTeamMode);
			break;
		case GameStatCounterType.YDown:
			winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Lowest, HighScores, IsTeamMode);
			break;
		default:
			winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Time, HighScores, IsTeamMode);
			break;
		}
	}
}
