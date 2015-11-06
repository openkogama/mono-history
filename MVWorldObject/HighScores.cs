using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;

public class HighScores
{
	private bool byAscending;

	public readonly Dictionary<MVTeam, HighScore> highScores = new Dictionary<MVTeam, HighScore>();

	public readonly GameStatCounterType gameStatCounterType = GameStatCounterType.None;

	public readonly bool presentAsTeamScore = false;

	public readonly WinningConditionPresentStyle winningConditionPresentStyle = WinningConditionPresentStyle.NoWinner;

	public HighScores(GameStatCounterType gameStatCounterType, Dictionary<MVTeam, TeamCounter> teamCounters, bool presentAsTeamScore, WinningConditionPresentStyle winningConditionPresentStyle, bool byAscending)
	{
		this.gameStatCounterType = gameStatCounterType;
		if (teamCounters != null)
		{
			foreach (KeyValuePair<MVTeam, TeamCounter> teamCounter in teamCounters)
			{
				if (teamCounter.Key != MVTeam.Server)
				{
					highScores.Add(teamCounter.Key, teamCounter.Value.HighScore);
				}
			}
		}
		this.presentAsTeamScore = presentAsTeamScore;
		this.winningConditionPresentStyle = winningConditionPresentStyle;
		this.byAscending = byAscending;
	}

	public List<ScoreTeamEntry> GenerateTeamScores()
	{
		List<ScoreTeamEntry> list = new List<ScoreTeamEntry>();
		foreach (KeyValuePair<MVTeam, HighScore> highScore in highScores)
		{
			list.Add(new ScoreTeamEntry(highScore.Key, highScore.Value.teamScore));
		}
		if (byAscending)
		{
			return list.OrderBy((ScoreTeamEntry x) => x.counter).ToList();
		}
		return list.OrderByDescending((ScoreTeamEntry x) => x.counter).ToList();
	}

	public List<ScoreActorEntry> GenerateActorScores()
	{
		Dictionary<int, ScoreActorEntry> dictionary = new Dictionary<int, ScoreActorEntry>();
		foreach (KeyValuePair<MVTeam, HighScore> highScore in highScores)
		{
			foreach (ScoreActorEntry highScoreEntry in highScore.Value.highScoreEntries)
			{
				ScoreActorEntry scoreActorEntry = new ScoreActorEntry(highScoreEntry.actorNumber, highScoreEntry.counter);
				if (dictionary.ContainsKey(highScoreEntry.actorNumber))
				{
					scoreActorEntry = new ScoreActorEntry(scoreActorEntry.actorNumber, dictionary[highScoreEntry.actorNumber].counter + scoreActorEntry.counter);
				}
				dictionary[scoreActorEntry.actorNumber] = scoreActorEntry;
			}
		}
		List<ScoreActorEntry> source = new List<ScoreActorEntry>(dictionary.Values);
		if (byAscending)
		{
			return source.OrderBy((ScoreActorEntry x) => x.counter).ToList();
		}
		return source.OrderByDescending((ScoreActorEntry x) => x.counter).ToList();
	}

	public override string ToString()
	{
		string text = "HighScore pr team:\n";
		foreach (KeyValuePair<MVTeam, HighScore> highScore in highScores)
		{
			text += $"Team: {highScore.Key}\n";
			text += highScore.Value;
		}
		return text;
	}
}
