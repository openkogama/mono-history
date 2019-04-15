namespace MV.WorldObject.GamePassSystem;

public class HighScoreEntry
{
	public int profileID;

	public string username = "";

	public int gamePoints;

	public bool isSubscriber;

	public HighScoreEntry()
	{
	}

	public HighScoreEntry(int profileID, string username, int gamePoints, bool isSubscriber)
	{
		this.profileID = profileID;
		this.username = username;
		this.gamePoints = gamePoints;
		this.isSubscriber = isSubscriber;
	}

	public override string ToString()
	{
		return $"{profileID}. {username}. {gamePoints}. {isSubscriber}.";
	}
}
