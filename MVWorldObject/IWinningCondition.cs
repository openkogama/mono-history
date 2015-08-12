using System;

public interface IWinningCondition
{
	int ID { get; }

	IWinningCondition Parent { get; }

	int Limit { get; }

	bool Forfilled { get; }

	OnCounterTypeChangedArgs InstigatorCounterTypeChangedEvent { get; }

	bool IsBriefingNode { get; }

	HighScores HighScores { get; }

	bool IsSingleton { get; }

	bool IsTeamMode { get; }

	event EventHandler<EventArgs> OnWinningConditionChanged;

	bool Traverse(Func<IWinningCondition, bool> callBack);

	void Reset();
}
