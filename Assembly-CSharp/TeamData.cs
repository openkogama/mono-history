using MV.WorldObject;

public class TeamData
{
	public readonly MVTeam team;

	public readonly int playersCount;

	public readonly int score;

	public readonly string representedName;

	public TeamData(MVTeam team, int playersCount, int score, string representedName)
	{
		this.team = team;
		this.playersCount = playersCount;
		this.score = score;
		this.representedName = representedName;
	}
}
