using System.Collections.Generic;

public class ScoreSorter : IComparer<PlayerData>
{
	public int Compare(PlayerData data1, PlayerData data2)
	{
		return PlayerValue(data2) - PlayerValue(data1);
	}

	private int PlayerValue(PlayerData data)
	{
		if (data.player == null)
		{
			return 0;
		}
		return data.Score;
	}
}
