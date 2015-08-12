public class TimeLimitClient : TimeLimit, IWinningConditionBriefing
{
	public override bool IsBriefingNode => CounterType != GameStatCounterType.None;

	public TimeLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, GameStatCounterType counterType)
		: base(parent, id, gameCounterManager, counterType)
	{
	}

	public void GetBriefing(MVGUIWinningConditionBriefingView winningConditionBriefingView)
	{
		switch (CounterType)
		{
		case GameStatCounterType.YUp:
			winningConditionBriefingView.AddBriefing("Highest");
			break;
		case GameStatCounterType.YDown:
			winningConditionBriefingView.AddBriefing("Lowest");
			break;
		}
	}

	public void GetDebriefing(MVGUIWinningConditionDebriefingView winningConditionDebriefingView)
	{
		switch (CounterType)
		{
		case GameStatCounterType.YUp:
			winningConditionDebriefingView.SetupDebriefing("Highest", HighScores, IsTeamMode);
			break;
		case GameStatCounterType.YDown:
			winningConditionDebriefingView.SetupDebriefing("Lowest", HighScores, IsTeamMode);
			break;
		default:
			winningConditionDebriefingView.SetupDebriefing("Time", HighScores, IsTeamMode);
			break;
		}
	}
}
