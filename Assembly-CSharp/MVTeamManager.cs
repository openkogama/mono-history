using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVTeamManager
{
	public delegate void OnTeamsUpdatedDelegate();

	private readonly Dictionary<MVTeam, string> teamNamesDefault = new Dictionary<MVTeam, string>
	{
		{
			MVTeam.Blue,
			TM._("Blue Team")
		},
		{
			MVTeam.Red,
			TM._("Red Team")
		},
		{
			MVTeam.Green,
			TM._("Green Team")
		},
		{
			MVTeam.Yellow,
			TM._("Yellow Team")
		}
	};

	private Dictionary<MVTeam, string> teamNames = new Dictionary<MVTeam, string>
	{
		{
			MVTeam.Blue,
			TM._("Blue Team")
		},
		{
			MVTeam.Red,
			TM._("Red Team")
		},
		{
			MVTeam.Green,
			TM._("Green Team")
		},
		{
			MVTeam.Yellow,
			TM._("Yellow Team")
		}
	};

	public OnTeamsUpdatedDelegate OnTeamsUpdated;

	private Dictionary<MVTeam, HashSet<int>> teams = new Dictionary<MVTeam, HashSet<int>>();

	public int NumSpawnPoint
	{
		get
		{
			int num = 0;
			foreach (HashSet<int> value in teams.Values)
			{
				num += value.Count;
			}
			return num;
		}
	}

	public event EventHandler<TeamEventArgs> OnTeamAdded;

	public event EventHandler<TeamEventArgs> OnTeamRemoved;

	public List<TeamData> GetTeamDatas(GameStatCounterType gameStatCounterType)
	{
		List<TeamData> list = new List<TeamData>();
		foreach (MVTeam key in teams.Keys)
		{
			int score = GetScore(key, gameStatCounterType);
			int noOfPlayersInTeam = GetNoOfPlayersInTeam(key);
			list.Add(new TeamData(key, noOfPlayersInTeam, score, teamNames[key]));
		}
		return list;
	}

	public Dictionary<MVTeam, string> GetTeamNames()
	{
		return teamNames;
	}

	public void UpdateTeamName(MVTeam team, string name)
	{
		teamNames[team] = name;
	}

	public void SetTeamNameToDefault(MVTeam team)
	{
		teamNames[team] = teamNamesDefault[team];
	}

	public void OnAddSpawnPoint(int woId, MVTeam team)
	{
		if (team == MVTeam.Server)
		{
			Debug.LogError("Attempt to AddTeam of type Server");
			return;
		}
		bool flag = false;
		if (!teams.ContainsKey(team))
		{
			teams.Add(team, new HashSet<int>());
			flag = true;
		}
		teams[team].Add(woId);
		if (flag)
		{
			if (OnTeamsUpdated != null)
			{
				OnTeamsUpdated();
			}
			if (OnTeamAdded != null && OnTeamAdded != null)
			{
				OnTeamAdded(this, new TeamEventArgs(team));
			}
		}
	}

	public void OnRemoveSpawnPoint(int id, MVTeam team)
	{
		if (!teams[team].Remove(id))
		{
			throw new Exception("Team wo id already removed");
		}
		if (teams[team].Count == 0)
		{
			teams.Remove(team);
			if (OnTeamsUpdated != null)
			{
				OnTeamsUpdated();
			}
			if (OnTeamRemoved != null && OnTeamRemoved != null)
			{
				OnTeamRemoved(this, new TeamEventArgs(team));
			}
			MVTeam defaultTeam = GetDefaultTeam();
			MVGameControllerBase.Game.MVPlayerContainer.UpdateTeamForPlayersOnRemovedTeam(team, defaultTeam);
		}
		Debug.Log("OnRemoveSpawnPoint");
		Debug.Log(ToString());
	}

	public List<MVWorldObjectClient> GetSpawnPointsForTeam(MVTeam team)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		foreach (int item in teams[team])
		{
			list.Add(MVGameControllerBase.WOCM.GetWorldObjectClient(item));
		}
		return list;
	}

	public List<MVWorldObjectClient> GetOnlySpawnPointsForTeam(MVTeam team)
	{
		if (TeamHasSpawnPoints(team))
		{
			List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
			{
				foreach (int item in teams[team])
				{
					MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item);
					if (worldObjectClient is MVSpawnPoint)
					{
						list.Add(worldObjectClient);
					}
				}
				return list;
			}
		}
		return GetSpawnPointsForTeam(team);
	}

	public MVTeam GetDefaultTeam()
	{
		for (int i = 0; i < 4; i++)
		{
			MVTeam mVTeam = (MVTeam)i;
			if (teams.ContainsKey(mVTeam))
			{
				return mVTeam;
			}
		}
		throw new Exception("Could not find default team");
	}

	public bool IsTeamActive(MVTeam team)
	{
		return teams.ContainsKey(team);
	}

	public int GetScore(MVTeam team, GameStatCounterType gameStatCounterType)
	{
		if (!teams.ContainsKey(team))
		{
			return 0;
		}
		return MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, team);
	}

	public int TeamCount()
	{
		return teams.Count;
	}

	public List<MVTeam> GetTeamList()
	{
		return teams.Keys.ToList();
	}

	public bool HasTeam(MVTeam team)
	{
		return teams.ContainsKey(team);
	}

	public bool TeamHasSpawnPoints(MVTeam team)
	{
		List<MVWorldObjectClient> spawnPointsForTeam = MVGameControllerBase.Game.TeamManager.GetSpawnPointsForTeam(team);
		bool result = false;
		for (int i = 0; i < spawnPointsForTeam.Count; i++)
		{
			if (spawnPointsForTeam[i] is MVSpawnPoint)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool TeamHasSpawnRoles(MVTeam team)
	{
		List<MVWorldObjectClient> spawnPointsForTeam = MVGameControllerBase.Game.TeamManager.GetSpawnPointsForTeam(team);
		bool result = false;
		for (int i = 0; i < spawnPointsForTeam.Count; i++)
		{
			if (spawnPointsForTeam[i].WorldObjectType == WorldObjectType.AvatarSpawnRoleCreator)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public List<MVPlayer> GetPlayersInTeam(MVTeam team)
	{
		return MVGameControllerBase.Game.MVPlayerContainer.Values.Where((MVPlayer player) => player.Team == team).ToList();
	}

	public MVTeam GetTeamFromActorNr(int actorNumber)
	{
		return (actorNumber != 0) ? MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNumber).Team : MVTeam.Server;
	}

	public int GetNoOfPlayersInTeam(MVTeam team)
	{
		return GetPlayersInTeam(team).Count;
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (MVTeam team in GetTeamList())
		{
			text = text + "\n" + team;
		}
		return text;
	}
}
