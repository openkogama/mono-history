public class TimeLimitClient : TimeLimit, IWinningConditionBriefing
{
	public override bool IsBriefingNode => CounterType != GameStatCounterType.None;

	public TimeLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		GameStatCounterType statType = GameStatCounterType.None;
		WinningConditionControl.TryGetPrioritizedStat(out statType);
		CounterType = statType;
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		winningConditionDebriefingView.SetupDebriefing(condition, HighScores, IsTeamMode);
	}
}
