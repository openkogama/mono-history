using System;
using MV.WorldObject;

public interface IGameStatCounterQuery
{
	event EventHandler<OnCounterTypeChangedArgs> OnCounterTypeChanged;

	int GetTeamCount(GameStatCounterType statType, MVTeam team);

	int GetActorCount(GameStatCounterType statType, MVTeam team, int actorNumber);

	HighScores GetHighScores(GameStatCounterType statType, bool presentAsTeamScore, WinningConditionPresentStyle winningConditionPresentStyle, bool byAscending);
}
