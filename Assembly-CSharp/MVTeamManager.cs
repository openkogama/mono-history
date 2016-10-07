using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVTeamManager
{
	public delegate void OnTeamsUpdatedDelegate();

	private Dictionary<MVTeam, bool> teamActiveBools = new Dictionary<MVTeam, bool>
	{
		{
			MVTeam.Blue,
			false
		},
		{
			MVTeam.Red,
			false
		},
		{
			MVTeam.Green,
			false
		},
		{
			MVTeam.Yellow,
			false
		},
		{
			MVTeam.None,
			true
		}
	};

	public OnTeamsUpdatedDelegate OnTeamsUpdated;

	public event EventHandler<TeamEventArgs> OnTeamAdded;

	public event EventHandler<TeamEventArgs> OnTeamRemoved;

	public List<TeamData> GetTeamDatas(GameStatCounterType gameStatCounterType)
	{
		List<TeamData> list = new List<TeamData>();
		foreach (KeyValuePair<MVTeam, bool> teamActiveBool in teamActiveBools)
		{
			if (teamActiveBool.Value && teamActiveBool.Key != MVTeam.None)
			{
				int score = GetScore(teamActiveBool.Key, gameStatCounterType);
				int noOfPlayersInTeam = GetNoOfPlayersInTeam(teamActiveBool.Key);
				list.Add(new TeamData(teamActiveBool.Key, noOfPlayersInTeam, score));
			}
		}
		return list;
	}

	public void AddTeam(MVTeam team)
	{
		if (team == MVTeam.Server)
		{
			Debug.LogError("Attempt to AddTeam of type Server");
			return;
		}
		teamActiveBools[team] = true;
		if (TeamCount() > 2)
		{
			teamActiveBools[MVTeam.None] = false;
		}
		if (OnTeamsUpdated != null)
		{
			OnTeamsUpdated();
		}
		if (OnTeamAdded != null && OnTeamAdded != null)
		{
			OnTeamAdded(this, new TeamEventArgs(team));
		}
	}

	public void RemoveTeam(MVTeam team)
	{
		if (team == MVTeam.Server)
		{
			Debug.LogError("Attempt to RemoveTeam of type Server");
			return;
		}
		teamActiveBools[team] = false;
		if (TeamCount() == 1)
		{
			teamActiveBools[MVTeam.None] = true;
		}
		if (OnTeamsUpdated != null)
		{
			OnTeamsUpdated();
		}
		if (OnTeamRemoved != null && OnTeamRemoved != null)
		{
			OnTeamRemoved(this, new TeamEventArgs(team));
		}
	}

	public bool IsTeamActive(MVTeam team)
	{
		return teamActiveBools[team];
	}

	public int GetScore(MVTeam team, GameStatCounterType gameStatCounterType)
	{
		if (!teamActiveBools[team])
		{
			return 0;
		}
		return MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, team);
	}

	public int TeamCount()
	{
		int num = 0;
		foreach (KeyValuePair<MVTeam, bool> teamActiveBool in teamActiveBools)
		{
			if (teamActiveBool.Value && teamActiveBool.Key != MVTeam.None)
			{
				num++;
			}
		}
		return num;
	}

	public List<MVTeam> GetTeamList()
	{
		List<MVTeam> list = new List<MVTeam>();
		foreach (KeyValuePair<MVTeam, bool> teamActiveBool in teamActiveBools)
		{
			if (teamActiveBool.Value)
			{
				list.Add(teamActiveBool.Key);
			}
		}
		return list;
	}

	public List<MVPlayer> GetPlayersInTeam(MVTeam team)
	{
		return MVGameControllerBase.Game.Players.Values.Where((MVPlayer player) => player.Team == team).ToList();
	}

	public MVTeam GetTeamFromActorNr(int actorNumber)
	{
		return MVGameControllerBase.Game.Players[actorNumber].Team;
	}

	public int GetNoOfPlayersInTeam(MVTeam team)
	{
		return GetPlayersInTeam(team).Count;
	}
}
