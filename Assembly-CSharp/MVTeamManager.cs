using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class MVTeamManager
{
	public delegate void OnTeamScoreUpdateDelegate();

	public delegate void OnTeamsUpdatedDelegate();

	private MVTeamData[] teamData = new MVTeamData[4];

	public OnTeamScoreUpdateDelegate OnTeamScoreUpdate;

	public OnTeamsUpdatedDelegate OnTeamsUpdated;

	public MVTeamManager()
	{
		for (int i = 0; i < 4; i++)
		{
			teamData[i] = new MVTeamData();
		}
	}

	public void AddTeam(MVTeam team)
	{
		if (team == MVTeam.None)
		{
			Debug.LogError((object)"Attempt to AddTeam of type None");
			return;
		}
		teamData[(int)team].active = true;
		teamData[(int)team].score = 0;
		if (OnTeamsUpdated != null)
		{
			OnTeamsUpdated();
		}
	}

	public void RemoveTeam(MVTeam team)
	{
		if (team == MVTeam.None)
		{
			Debug.LogError((object)"Attempt to RemoveTeam of type None");
			return;
		}
		teamData[(int)team].active = false;
		if (OnTeamsUpdated != null)
		{
			OnTeamsUpdated();
		}
	}

	public bool IsTeamActive(MVTeam team)
	{
		if (team == MVTeam.None)
		{
			return false;
		}
		return teamData[(int)team].active;
	}

	public void AddScore(MVTeam team, int score)
	{
		switch (team)
		{
		case MVTeam.None:
			Debug.LogError((object)"Attempt to AddScore to team of type None");
			return;
		case MVTeam.All:
			foreach (MVTeam team2 in GetTeamList())
			{
				teamData[(int)team2].score += score;
			}
			break;
		default:
			if (teamData[(int)team].active)
			{
				teamData[(int)team].score += score;
			}
			break;
		}
		if (OnTeamScoreUpdate != null)
		{
			OnTeamScoreUpdate();
		}
	}

	public void SetScore(MVTeam team, int score)
	{
		switch (team)
		{
		case MVTeam.None:
			return;
		case MVTeam.All:
			foreach (MVTeam team2 in GetTeamList())
			{
				teamData[(int)team2].score = score;
			}
			break;
		default:
			if (teamData[(int)team].active)
			{
				teamData[(int)team].score = score;
			}
			break;
		}
		if (OnTeamScoreUpdate != null)
		{
			OnTeamScoreUpdate();
		}
	}

	public int GetScore(MVTeam team)
	{
		if (team == MVTeam.None)
		{
			return 0;
		}
		if (!teamData[(int)team].active)
		{
			return 0;
		}
		return teamData[(int)team].score;
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
		return MVGameController.Instance.Game.Players.Values.Where((MVPlayer player) => player.Team == team).ToList();
	}

	public MVTeam GetTeamFromActorNr(int actorNumber)
	{
		return MVGameController.Instance.Game.Players[actorNumber].Team;
	}

	public int GetNoOfPlayersInTeam(MVTeam team)
	{
		return GetPlayersInTeam(team).Count;
	}
}
