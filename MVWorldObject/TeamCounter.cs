using System.Collections.Generic;
using MV.WorldObject;

public class TeamCounter
{
	private Dictionary<int, ActorCounter> actorCounters = new Dictionary<int, ActorCounter>();

	private int teamCount = 0;

	public int TeamCount => teamCount;

	public HighScore HighScore => new HighScore(teamCount, actorCounters);

	public TeamCounter()
	{
	}

	public TeamCounter(BytePacker bp)
	{
		teamCount = bp.ReadInt32();
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int key = bp.ReadInt32();
			ActorCounter value = new ActorCounter(bp);
			actorCounters.Add(key, value);
		}
	}

	public int GetActorCount(int actorNumber)
	{
		if (!actorCounters.ContainsKey(actorNumber))
		{
			return 0;
		}
		return actorCounters[actorNumber].Count;
	}

	public bool ContainsActor(int actorNumber)
	{
		return actorCounters.ContainsKey(actorNumber);
	}

	public int Increment(int actorNumber, int value, bool includeTeamScore)
	{
		if (!actorCounters.ContainsKey(actorNumber))
		{
			actorCounters.Add(actorNumber, new ActorCounter(0));
		}
		if (includeTeamScore)
		{
			teamCount += value;
		}
		return actorCounters[actorNumber].Increment(value);
	}

	public int Update(int actorNumber, int value, bool includeTeamScore)
	{
		if (includeTeamScore)
		{
			teamCount = value;
		}
		actorCounters[actorNumber] = new ActorCounter(value);
		return actorCounters[actorNumber].Count;
	}

	public int UpdateTeam(int value)
	{
		teamCount = value;
		return teamCount;
	}

	public void RemoveStatsFromActor(int actorNumber)
	{
		actorCounters.Remove(actorNumber);
	}

	public override string ToString()
	{
		string text = $"Team count: {teamCount}.\n";
		foreach (KeyValuePair<int, ActorCounter> actorCounter in actorCounters)
		{
			string arg = $"Actor number: {actorCounter.Key}. Count: {actorCounter.Value}";
			text += $"{arg}\n";
		}
		return text;
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(teamCount);
		bytePacker.Write(actorCounters.Count);
		foreach (KeyValuePair<int, ActorCounter> actorCounter in actorCounters)
		{
			bytePacker.Write(actorCounter.Key);
			bytePacker.Write(actorCounter.Value.ToByteArray());
		}
		return bytePacker.ToArray();
	}
}
