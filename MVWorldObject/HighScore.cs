using System.Collections.Generic;
using System.Linq;

public class HighScore
{
	public List<ScoreActorEntry> highScoreEntries = new List<ScoreActorEntry>();

	public readonly int teamScore;

	public HighScore(int teamScore, Dictionary<int, ActorCounter> actorCounters)
	{
		this.teamScore = teamScore;
		foreach (KeyValuePair<int, ActorCounter> actorCounter in actorCounters)
		{
			highScoreEntries.Add(new ScoreActorEntry(actorCounter.Key, actorCounter.Value.Count));
		}
		SortDescending();
	}

	public void Sort()
	{
		highScoreEntries = highScoreEntries.OrderBy((ScoreActorEntry x) => x.counter).ToList();
	}

	public void SortDescending()
	{
		highScoreEntries = highScoreEntries.OrderByDescending((ScoreActorEntry x) => x.counter).ToList();
	}

	public override string ToString()
	{
		string text = $"TeamScore: {teamScore}.\n";
		foreach (ScoreActorEntry highScoreEntry in highScoreEntries)
		{
			text += $"{highScoreEntry}\n";
		}
		return text;
	}
}
