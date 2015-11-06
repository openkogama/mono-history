using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVTeamManager
{
	public delegate void OnTeamsUpdatedDelegate();

	private MVTeamData[] teamData = new MVTeamData[4];

	public OnTeamsUpdatedDelegate OnTeamsUpdated;

	public event EventHandler<TeamEventArgs> OnTeamAdded;

	public event EventHandler<TeamEventArgs> OnTeamRemoved;

	public MVTeamManager()
	{
		for (int i = 0; i < 4; i++)
		{
			teamData[i] = new MVTeamData();
		}
	}

	public void AddTeam(MVTeam team)
	{
		if (team == MVTeam.Server)
		{
			Debug.LogError("Attempt to AddTeam of type Server");
			return;
		}
		teamData[(int)team].active = true;
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
		teamData[(int)team].active = false;
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
		return teamData[(int)team].active;
	}

	public int GetScore(MVTeam team, GameStatCounterType gameStatCounterType)
	{
		if (!teamData[(int)team].active)
		{
			return 0;
		}
		return MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, team);
	}

	public int TeamCount()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (teamData[i].active)
			{
				num++;
			}
		}
		return num;
	}

	public List<MVTeam> GetTeamList()
	{
		List<MVTeam> list = new List<MVTeam>();
		for (int i = 0; i < 4; i++)
		{
			if (teamData[i].active)
			{
				list.Add((MVTeam)(int)Enum.ToObject(typeof(MVTeam), i));
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
