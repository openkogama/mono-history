using MV.WorldObject;

public class TeamData
{
	public readonly MVTeam team;

	public readonly int playersCount;

	public readonly int score;

	public TeamData(MVTeam team, int playersCount, int score)
	{
		this.team = team;
		this.playersCount = playersCount;
		this.score = score;
	}
}
