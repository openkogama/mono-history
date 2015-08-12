public class BadgeUrlData
{
	private int level = -1;

	private string url = string.Empty;

	private int friendsLimit = 200;

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public string URL
	{
		get
		{
			return url;
		}
		set
		{
			url = value;
		}
	}

	public int FriendsLimit
	{
		get
		{
			return friendsLimit;
		}
		set
		{
			friendsLimit = value;
		}
	}

	public BadgeUrlData()
	{
	}

	public BadgeUrlData(int level, string url)
	{
		this.level = level;
		this.url = url;
	}

	public override string ToString()
	{
		return $"Level {Level}. Url {URL}. FriendsLimit {FriendsLimit}.";
	}
}
