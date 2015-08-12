using System.Collections.Generic;
using MV.WorldObject;

public class TeamsCounter
{
	protected Dictionary<MVTeam, TeamCounter> teamCounters = new Dictionary<MVTeam, TeamCounter>();

	public TeamsCounter()
	{
	}

	public TeamsCounter(BytePacker bp)
	{
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			MVTeam key = (MVTeam)bp.ReadByte();
			TeamCounter value = new TeamCounter(bp);
			teamCounters.Add(key, value);
		}
	}

	public bool ContainsTeamWithActor(MVTeam team, int actorNr)
	{
		if (!teamCounters.ContainsKey(team))
		{
			return false;
		}
		return teamCounters[team].ContainsActor(actorNr);
	}

	public HighScores GetHighScores(GameStatCounterType gameStatCounterType, bool presentAsTeamScore, WinningConditionPresentStyle winningConditionPresentStyle, bool byAscending)
	{
		return new HighScores(gameStatCounterType, teamCounters, presentAsTeamScore, winningConditionPresentStyle, byAscending);
	}

	public int GetTeamCount(MVTeam team)
	{
		if (!teamCounters.ContainsKey(team))
		{
			return 0;
		}
		return teamCounters[team].TeamCount;
	}

	public int GetActorCount(MVTeam team, int actorNumber)
	{
		if (!teamCounters.ContainsKey(team))
		{
			return 0;
		}
		return teamCounters[team].GetActorCount(actorNumber);
	}

	public int Increment(MVTeam team, int actorNumber, int value, bool includeTeamScore)
	{
		AddIfNotPresent(team);
		return teamCounters[team].Increment(actorNumber, value, includeTeamScore);
	}

	public int Update(MVTeam team, int actorNumber, int value, bool includeTeamScore)
	{
		AddIfNotPresent(team);
		return teamCounters[team].Update(actorNumber, value, includeTeamScore);
	}

	public int UpdateTeam(MVTeam team, int value)
	{
		AddIfNotPresent(team);
		return teamCounters[team].UpdateTeam(value);
	}

	public void RemoveStatsFromActor(int actorNumber)
	{
		foreach (KeyValuePair<MVTeam, TeamCounter> teamCounter in teamCounters)
		{
			teamCounter.Value.RemoveStatsFromActor(actorNumber);
		}
	}

	private void AddIfNotPresent(MVTeam team)
	{
		if (!teamCounters.ContainsKey(team))
		{
			teamCounters.Add(team, new TeamCounter());
		}
	}

	public override string ToString()
	{
		string text = "";
		foreach (KeyValuePair<MVTeam, TeamCounter> teamCounter in teamCounters)
		{
			string text2 = $"\nTeam: {teamCounter.Key}. TeamCounter: {teamCounter.Value}\n";
			text += text2;
		}
		return text;
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(teamCounters.Count);
		foreach (KeyValuePair<MVTeam, TeamCounter> teamCounter in teamCounters)
		{
			bytePacker.Write((byte)teamCounter.Key);
			bytePacker.Write(teamCounter.Value.ToByteArray());
		}
		return bytePacker.ToArray();
	}
}
