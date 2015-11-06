using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIPlayersWindow : MonoBehaviour
{
	public UXView ViewParent;

	public MVGUIPlayerLine playerLinePrefab;

	public MVGUIPlayerLine smallPlayerLinePrefab;

	public MVGUITeamList FullTeamList;

	public MVGUITeamList HalfTeamList;

	public MVGUITeamList QuarterTeamList;

	private Dictionary<MVTeam, MVGUITeamList> teamToPlayerlist = new Dictionary<MVTeam, MVGUITeamList>();

	private Dictionary<MVPlayer, MVGUIPlayerLine> playerLines = new Dictionary<MVPlayer, MVGUIPlayerLine>();

	private int NoOfTeams = 1;

	private bool updateTeamLists;

	public void InitializeListeners()
	{
		UXView viewParent = ViewParent;
		viewParent.OnShow = (UXView.OnShowDelegate)Delegate.Combine(viewParent.OnShow, new UXView.OnShowDelegate(OnShow));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, (MVNetworkGame.OnPlayerListChangedDelegate)(() =>
		{
			if (ViewParent.isVisible)
			{
				UpdatePlayerList();
			}
		}));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Combine(friends.OnFriendListUpdated, (FriendList.OnFriendListUpdatedDelegate)(() =>
		{
			if (ViewParent.isVisible)
			{
				UpdatePlayerList();
			}
		}));
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, (MVTeamManager.OnTeamsUpdatedDelegate)(() =>
		{
			if (ViewParent.isVisible)
			{
				BuildTeamLists();
			}
			else
			{
				UpdateTeamLists();
			}
		}));
		BuildTeamLists();
	}

	private void OnShow()
	{
		if (updateTeamLists)
		{
			BuildTeamLists();
		}
		else
		{
			UpdatePlayerList();
		}
	}

	private void BuildTeamLists()
	{
		RemoveAllTeamLists();
		List<MVTeam> teamList = MVGameControllerBase.Game.TeamManager.GetTeamList();
		NoOfTeams = teamList.Count;
		if (NoOfTeams > 1 && ShowTeams())
		{
			foreach (MVTeam item in teamList)
			{
				AddTeamList(item);
			}
		}
		else
		{
			NoOfTeams = 1;
			AddTeamList(teamList[0]);
		}
		UpdatePlayerList();
		updateTeamLists = false;
	}

	public void UpdateTeamLists()
	{
		updateTeamLists = true;
	}

	private void RemoveAllTeamLists()
	{
		MVGUITeamList[] componentsInChildren = transform.GetComponentsInChildren<MVGUITeamList>();
		foreach (MVGUITeamList mVGUITeamList in componentsInChildren)
		{
			mVGUITeamList.RemoveAllLines();
			UnityEngine.Object.Destroy(mVGUITeamList.gameObject);
		}
		playerLines.Clear();
		teamToPlayerlist.Clear();
	}

	private bool ShowTeams()
	{
		return MVGameControllerBase.GameMode == MVGameMode.Play || (MVGameControllerLegacyUI.EditorController != null && MVGameControllerLegacyUI.EditorController.PlayInEditor);
	}

	private void AddTeamList(MVTeam team)
	{
		MVGUITeamList original;
		if (NoOfTeams == 1)
		{
			original = FullTeamList;
		}
		else
		{
			original = ((NoOfTeams != 2) ? QuarterTeamList : HalfTeamList);
		}
		MVGUITeamList mVGUITeamList = UnityEngine.Object.Instantiate(original);
		mVGUITeamList.transform.parent = transform;
		mVGUITeamList.transform.localScale = Vector3.one;
		mVGUITeamList.transform.localPosition = GetTeamListPosition(mVGUITeamList);
		mVGUITeamList.InitializeTeamList(team);
		teamToPlayerlist.Add(team, mVGUITeamList);
	}

	private Vector3 GetTeamListPosition(MVGUITeamList playerList)
	{
		Vector2 fullSize = playerList.GetFullSize();
		if (NoOfTeams == 1)
		{
			return new Vector3((0f - fullSize.x) / 2f, fullSize.y / 2f, 0f);
		}
		if (NoOfTeams == 2)
		{
			float x = ((teamToPlayerlist.Count != 0) ? 0f : (0f - fullSize.x));
			return new Vector3(x, fullSize.y / 2f, 0f);
		}
		float x2 = ((teamToPlayerlist.Count % 2 != 0) ? 0f : (0f - fullSize.x));
		float y = ((teamToPlayerlist.Count >= 2) ? 0f : fullSize.y);
		return new Vector3(x2, y, 0f);
	}

	private void UpdatePlayerList()
	{
		ResetTeams();
		Dictionary<MVTeam, List<PlayerData>> dictionary = BuildPlayerList();
		foreach (KeyValuePair<MVTeam, List<PlayerData>> item in dictionary)
		{
			MVTeam key = item.Key;
			List<PlayerData> value = item.Value;
			value.Sort(new ScoreSorter());
			AddToPlayerList(key, value);
		}
	}

	private void AddToPlayerList(MVTeam team, List<PlayerData> players)
	{
		foreach (PlayerData player in players)
		{
			MVGUIPlayerLine mVGUIPlayerLine = playerLines[player.player];
			mVGUIPlayerLine.UpdateLine(player);
			teamToPlayerlist[team].AddLine(mVGUIPlayerLine);
		}
	}

	private void ResetTeams()
	{
		foreach (MVGUITeamList value in teamToPlayerlist.Values)
		{
			ClearLines(value);
		}
	}

	private void ClearLines(MVGUITeamList playersList)
	{
		List<MVGUIPlayerLine> list = new List<MVGUIPlayerLine>();
		List<MVPlayer> list2 = MVGameControllerBase.Game.Players.Values.ToList();
		foreach (MVGUIPlayerLine line in playersList.GetLines())
		{
			if (!list2.Contains(line.GetPlayer()))
			{
				list.Add(line);
			}
		}
		foreach (MVGUIPlayerLine item in list)
		{
			playersList.RemoveLine(item);
		}
		playersList.RemoveAllLines(destroy: false);
	}

	private Dictionary<MVTeam, List<PlayerData>> BuildPlayerList()
	{
		Dictionary<MVTeam, List<PlayerData>> dictionary = new Dictionary<MVTeam, List<PlayerData>>();
		MVGUIPlayerLine original = ((NoOfTeams != 1) ? smallPlayerLinePrefab : playerLinePrefab);
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			PlayerData playerData = new PlayerData(value, MVGameControllerBase.Game.Friends.GetFriendByProfileID(value.ProfileID));
			MVTeam team = value.Team;
			if (teamToPlayerlist.ContainsKey(team))
			{
				if (!dictionary.ContainsKey(team))
				{
					dictionary.Add(team, new List<PlayerData>());
				}
				dictionary[team].Add(playerData);
				if (!playerLines.ContainsKey(value))
				{
					MVGUIPlayerLine mVGUIPlayerLine = UnityEngine.Object.Instantiate(original);
					mVGUIPlayerLine.InitLine(playerData);
					playerLines.Add(value, mVGUIPlayerLine);
				}
			}
		}
		return dictionary;
	}
}
