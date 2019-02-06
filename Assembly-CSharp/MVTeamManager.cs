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
		}
	};

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

	public event EventHandler<TeamEventArgs> OnTeamAdded;

	public event EventHandler<TeamEventArgs> OnTeamRemoved;

	public List<TeamData> GetTeamDatas(GameStatCounterType gameStatCounterType)
	{
		List<TeamData> list = new List<TeamData>();
		foreach (KeyValuePair<MVTeam, bool> teamActiveBool in teamActiveBools)
		{
			if (teamActiveBool.Value)
			{
				int score = GetScore(teamActiveBool.Key, gameStatCounterType);
				int noOfPlayersInTeam = GetNoOfPlayersInTeam(teamActiveBool.Key);
				list.Add(new TeamData(teamActiveBool.Key, noOfPlayersInTeam, score, teamNames[teamActiveBool.Key]));
			}
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

	public void AddTeam(MVTeam team)
	{
		if (team == MVTeam.Server)
		{
			Debug.LogError("Attempt to AddTeam of type Server");
			return;
		}
		teamActiveBools[team] = true;
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
			if (teamActiveBool.Value)
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
				MVTeam key = teamActiveBool.Key;
				list.Add(key);
			}
		}
		return list;
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

	public bool IsOnSameTeam(MVWorldObjectClient a, MVWorldObjectClient b)
	{
		if (TeamCount() <= 1)
		{
			return false;
		}
		if (a is ITeamInteractorNPC && b is ITeamInteractorNPC)
		{
			return a.OwnerActorNr == b.OwnerActorNr;
		}
		MVPlayer player = null;
		MVPlayer player2 = null;
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(a.OwnerActorNr, out player))
		{
			ITeamInteractorNPC teamInteractorNPC = b as ITeamInteractorNPC;
			if (b.OwnerActorNr == 0 && teamInteractorNPC != null)
			{
				return teamInteractorNPC.IsOnSameTeam(player.Team);
			}
		}
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(b.OwnerActorNr, out player2))
		{
			ITeamInteractorNPC teamInteractorNPC2 = a as ITeamInteractorNPC;
			if (a.OwnerActorNr == 0 && teamInteractorNPC2 != null)
			{
				return teamInteractorNPC2.IsOnSameTeam(player2.Team);
			}
		}
		if (player != null && player2 != null && player.Team == player2.Team)
		{
			return true;
		}
		return a.OwnerActorNr == b.OwnerActorNr;
	}
}
