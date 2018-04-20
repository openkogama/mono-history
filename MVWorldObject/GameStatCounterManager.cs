using System;
using System.Collections.Generic;
using MV.WorldObject;

public class GameStatCounterManager : IGameStatCounterQuery
{
	private HashSet<MVTeam> activeTeams = new HashSet<MVTeam>();

	protected Dictionary<GameStatCounterType, TeamsCounter> statTypeCounters = new Dictionary<GameStatCounterType, TeamsCounter>();

	public List<GameStatCounterType> sessionPersistentStats;

	public HashSet<MVTeam> ActiveTeams => new HashSet<MVTeam>(activeTeams);

	public event EventHandler<OnCounterTypeChangedArgs> OnCounterTypeChanged;

	public void OnTeamAdded(object sender, TeamEventArgs e)
	{
		if (activeTeams.Contains(e.team))
		{
			throw new Exception("Team already added");
		}
		activeTeams.Add(e.team);
	}

	public void OnTeamRemoved(object sender, TeamEventArgs e)
	{
		if (!activeTeams.Contains(e.team))
		{
			throw new Exception("Team does not exists");
		}
		activeTeams.Remove(e.team);
	}

	public GameStatCounterManager()
	{
		sessionPersistentStats = new List<GameStatCounterType> { GameStatCounterType.Flag };
	}

	public GameStatCounterManager(byte[] data)
	{
		SetStats(data);
	}

	public bool ContainsStatTypeForActor(GameStatCounterType statType, MVTeam team, int actorNr)
	{
		if (!statTypeCounters.ContainsKey(statType))
		{
			return false;
		}
		return statTypeCounters[statType].ContainsTeamWithActor(team, actorNr);
	}

	public int GetTeamCount(GameStatCounterType statType, MVTeam team)
	{
		if (!statTypeCounters.ContainsKey(statType))
		{
			return 0;
		}
		return statTypeCounters[statType].GetTeamCount(team);
	}

	public HighScores GetHighScores(GameStatCounterType statType, bool presentAsTeamScore, WinningConditionPresentStyle winningConditionPresentStyle, bool byAscending)
	{
		if (!statTypeCounters.ContainsKey(statType))
		{
			return new HighScores(statType, null, presentAsTeamScore, winningConditionPresentStyle, byAscending);
		}
		return statTypeCounters[statType].GetHighScores(statType, presentAsTeamScore, winningConditionPresentStyle, byAscending);
	}

	public int GetActorCount(GameStatCounterType counterType, MVTeam team, int actorNumber)
	{
		if (!statTypeCounters.ContainsKey(counterType))
		{
			return 0;
		}
		return statTypeCounters[counterType].GetActorCount(team, actorNumber);
	}

	public void Increment(GameStatCounterType counterType, MVTeam team, int actorNumber, int value, int otherID, bool includeTeamScore)
	{
		Validate(actorNumber, team);
		AddIfNotPresent(counterType);
		int count = statTypeCounters[counterType].Increment(team, actorNumber, value, includeTeamScore);
		SendChangeEvent(count, counterType, actorNumber, team, otherID);
	}

	public void Update(GameStatCounterType counterType, int actorNumber, MVTeam team, int value, int otherID, bool includeTeamScore)
	{
		Validate(actorNumber, team);
		AddIfNotPresent(counterType);
		int count = statTypeCounters[counterType].Update(team, actorNumber, value, includeTeamScore);
		SendChangeEvent(count, counterType, actorNumber, team, otherID);
	}

	public void RemoveStatsFromActor(int actorNumber)
	{
		foreach (KeyValuePair<GameStatCounterType, TeamsCounter> statTypeCounter in statTypeCounters)
		{
			statTypeCounter.Value.RemoveStatsFromActor(actorNumber);
		}
	}

	public void Clear()
	{
		Dictionary<GameStatCounterType, TeamsCounter> dictionary = new Dictionary<GameStatCounterType, TeamsCounter>();
		for (int i = 0; i < sessionPersistentStats.Count; i++)
		{
			if (statTypeCounters.ContainsKey(sessionPersistentStats[i]))
			{
				dictionary.Add(sessionPersistentStats[i], statTypeCounters[sessionPersistentStats[i]]);
			}
		}
		statTypeCounters.Clear();
		statTypeCounters = dictionary;
	}

	private void SendChangeEvent(int count, GameStatCounterType counterType, int actorNumber, MVTeam team, int otherID)
	{
		if (OnCounterTypeChanged != null)
		{
			OnCounterTypeChanged(this, new OnCounterTypeChangedArgs(count, counterType, actorNumber, team, otherID));
		}
	}

	private void Validate(int actorNumber, MVTeam team)
	{
		if (team == MVTeam.Server && actorNumber != 0)
		{
			throw new Exception("Team.Server is reserved by server. Actor number must be 0.");
		}
	}

	private void AddIfNotPresent(GameStatCounterType statType)
	{
		if (!statTypeCounters.ContainsKey(statType))
		{
			statTypeCounters.Add(statType, new TeamsCounter());
		}
	}

	public override string ToString()
	{
		string text = "Active teams: \n";
		foreach (MVTeam activeTeam in activeTeams)
		{
			text += $"{activeTeam}\n";
		}
		foreach (KeyValuePair<GameStatCounterType, TeamsCounter> statTypeCounter in statTypeCounters)
		{
			string text2 = $"CounterType: {statTypeCounter.Key}. Stats: {statTypeCounter.Value}";
			text += text2;
		}
		return text;
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(statTypeCounters.Count);
		foreach (KeyValuePair<GameStatCounterType, TeamsCounter> statTypeCounter in statTypeCounters)
		{
			ToByteArray(statTypeCounter.Key, statTypeCounter.Value, bytePacker);
		}
		return bytePacker.ToArray();
	}

	public byte[] ToByteArrayStatType(GameStatCounterType statCounterType)
	{
		if (!statTypeCounters.ContainsKey(statCounterType))
		{
			return null;
		}
		BytePacker bytePacker = new BytePacker();
		ToByteArray(statCounterType, statTypeCounters[statCounterType], bytePacker);
		return bytePacker.ToArray();
	}

	private void ToByteArray(GameStatCounterType statTypeCounter, TeamsCounter teamsCounter, BytePacker bytePacker)
	{
		bytePacker.Write((byte)statTypeCounter);
		bytePacker.Write(teamsCounter.ToByteArray());
	}

	public void SetStats(byte[] data)
	{
		BytePacker bytePacker = new BytePacker(data);
		int num = bytePacker.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			SetStat(bytePacker);
		}
	}

	public void SetStat(byte[] data)
	{
		SetStat(new BytePacker(data));
	}

	private void SetStat(BytePacker bp)
	{
		GameStatCounterType key = (GameStatCounterType)bp.ReadByte();
		TeamsCounter value = new TeamsCounter(bp);
		if (statTypeCounters.ContainsKey(key))
		{
			statTypeCounters.Remove(key);
		}
		statTypeCounters.Add(key, value);
	}
}
