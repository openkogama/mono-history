using MV.WorldObject;

public class ScoreTeamEntry : IScoreEntry
{
	public readonly MVTeam team;

	public readonly int counter;

	public ScoreTeamEntry(MVTeam team, int counter)
	{
		this.team = team;
		this.counter = counter;
	}

	public override string ToString()
	{
		return $"Team: {team}. Counter: {counter}.";
	}
}
