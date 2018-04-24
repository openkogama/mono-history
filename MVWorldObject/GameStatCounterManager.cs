using System;
using System.Collections.Generic;
using MV.WorldObject;

public class GameStatCounterManager : IGameStatCounterQuery
{
	private List<GameStatCounterType> sessionPersistentStats = new List<GameStatCounterType> { GameStatCounterType.Flag };

	private Dictionary<GameStatCounterType, TeamsCounter> persistentStats;

	private HashSet<MVTeam> activeTeams = new HashSet<MVTeam>();

	protected Dictionary<GameStatCounterType, TeamsCounter> statTypeCounters = new Dictionary<GameStatCounterType, TeamsCounter>();

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
		if (persistentStats == null)
		{
			persistentStats = new Dictionary<GameStatCounterType, TeamsCounter>();
		}
	}

	public GameStatCounterManager(byte[] data)
	{
		if (persistentStats == null)
		{
			persistentStats = new Dictionary<GameStatCounterType, TeamsCounter>();
		}
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
		int num = statTypeCounters[counterType].Increment(team, actorNumber, value, includeTeamScore);
		SendChangeEvent(num, counterType, actorNumber, team, otherID);
		if (sessionPersistentStats.Contains(counterType) && IsNewScoreBetter(num, persistentStats[counterType].GetActorCount(team, actorNumber), counterType))
		{
			persistentStats[counterType].Update(team, actorNumber, num, includeTeamScore);
		}
	}

	public void Update(GameStatCounterType counterType, int actorNumber, MVTeam team, int value, int otherID, bool includeTeamScore)
	{
		Validate(actorNumber, team);
		AddIfNotPresent(counterType);
		if (sessionPersistentStats.Contains(counterType))
		{
			ClearStats();
			AddIfNotPresent(counterType);
			if (IsNewScoreBetter(value, persistentStats[counterType].GetActorCount(team, actorNumber), counterType))
			{
				persistentStats[counterType].Update(team, actorNumber, value, includeTeamScore);
			}
		}
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
		statTypeCounters.Clear();
		AddPersistentStats();
	}

	public void ClearStats()
	{
		statTypeCounters.Clear();
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
		if (sessionPersistentStats.Contains(statType) && !persistentStats.ContainsKey(statType))
		{
			persistentStats.Add(statType, new TeamsCounter());
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
		GameStatCounterType gameStatCounterType = (GameStatCounterType)bp.ReadByte();
		TeamsCounter value = new TeamsCounter(bp);
		if (statTypeCounters.ContainsKey(gameStatCounterType))
		{
			statTypeCounters.Remove(gameStatCounterType);
		}
		statTypeCounters.Add(gameStatCounterType, value);
		if (sessionPersistentStats.Contains(gameStatCounterType))
		{
			if (persistentStats.ContainsKey(gameStatCounterType))
			{
				persistentStats.Remove(gameStatCounterType);
			}
			persistentStats.Add(gameStatCounterType, value);
		}
	}

	private bool IsNewScoreBetter(int newScore, int oldScore, GameStatCounterType statType)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			if (newScore > oldScore)
			{
				return true;
			}
			break;
		case GameStatCounterType.Flag:
			if (oldScore < 0)
			{
				return true;
			}
			if (newScore <= 0)
			{
				return false;
			}
			if (newScore < oldScore || oldScore == 0)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private void AddPersistentStats()
	{
		foreach (KeyValuePair<GameStatCounterType, TeamsCounter> persistentStat in persistentStats)
		{
			statTypeCounters.Add(persistentStat.Key, new TeamsCounter(persistentStat.Value));
		}
	}
}
