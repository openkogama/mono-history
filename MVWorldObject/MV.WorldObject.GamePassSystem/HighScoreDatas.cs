using System.Collections.Generic;

namespace MV.WorldObject.GamePassSystem;

public class HighScoreDatas
{
	public List<HighScoreEntry> highScores = new List<HighScoreEntry>();

	public int topRank;

	public HighScoreDatas()
	{
	}

	public HighScoreDatas(int topRank, List<HighScoreEntry> highScores)
	{
		this.topRank = topRank;
		this.highScores = highScores;
	}

	public override string ToString()
	{
		string text = "";
		for (int i = 0; i < highScores.Count; i++)
		{
			text += $"{i + topRank}. {highScores[i]}.\n";
		}
		return text;
	}
}
