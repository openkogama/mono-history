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
		MVNetworkGame game = MVGameController.Instance.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, (MVNetworkGame.OnPlayerListChangedDelegate)(() =>
		{
			if (ViewParent.isVisible)
			{
				UpdatePlayerList();
			}
		}));
		FriendList friends = MVGameController.Instance.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Combine(friends.OnFriendListUpdated, (FriendList.OnFriendListUpdatedDelegate)(() =>
		{
			if (ViewParent.isVisible)
			{
				UpdatePlayerList();
			}
		}));
		MVTeamManager teamManager = MVGameController.Instance.Game.TeamManager;
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
		List<MVTeam> teamList = MVGameController.Instance.Game.TeamManager.GetTeamList();
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
			AddTeamList(MVTeam.None);
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
		MVGUITeamList[] componentsInChildren = ((Component)((Component)this).transform).GetComponentsInChildren<MVGUITeamList>();
		foreach (MVGUITeamList mVGUITeamList in componentsInChildren)
		{
			mVGUITeamList.RemoveAllLines();
			Object.Destroy((Object)(object)((Component)mVGUITeamList).gameObject);
		}
		playerLines.Clear();
		teamToPlayerlist.Clear();
	}

	private bool ShowTeams()
	{
		return MVGameController.Instance.Game.GameMode == MVGameMode.Play || (MVGameController.Instance.EditorController != null && MVGameController.Instance.EditorController.PlayInEditor);
	}

	private void AddTeamList(MVTeam team)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		MVGUITeamList mVGUITeamList;
		if (NoOfTeams == 1)
		{
			mVGUITeamList = FullTeamList;
		}
		else
		{
			mVGUITeamList = ((NoOfTeams != 2) ? QuarterTeamList : HalfTeamList);
		}
		MVGUITeamList mVGUITeamList2 = Object.Instantiate((Object)(object)mVGUITeamList) as MVGUITeamList;
		((Component)mVGUITeamList2).transform.parent = ((Component)this).transform;
		((Component)mVGUITeamList2).transform.localScale = Vector3.one;
		((Component)mVGUITeamList2).transform.localPosition = GetTeamListPosition(mVGUITeamList2);
		mVGUITeamList2.InitializeTeamList(team);
		teamToPlayerlist.Add(team, mVGUITeamList2);
	}

	private Vector3 GetTeamListPosition(MVGUITeamList playerList)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 fullSize = playerList.GetFullSize();
		if (NoOfTeams == 1)
		{
			return new Vector3((0f - fullSize.x) / 2f, fullSize.y / 2f, 0f);
		}
		if (NoOfTeams == 2)
		{
			float num = ((teamToPlayerlist.Count != 0) ? 0f : (0f - fullSize.x));
			return new Vector3(num, fullSize.y / 2f, 0f);
		}
		float num2 = ((teamToPlayerlist.Count % 2 != 0) ? 0f : (0f - fullSize.x));
		float num3 = ((teamToPlayerlist.Count >= 2) ? 0f : fullSize.y);
		return new Vector3(num2, num3, 0f);
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
		List<MVPlayer> list2 = MVGameController.Instance.Game.Players.Values.ToList();
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
		MVGUIPlayerLine mVGUIPlayerLine = ((NoOfTeams != 1) ? smallPlayerLinePrefab : playerLinePrefab);
		foreach (MVPlayer value in MVGameController.Instance.Game.Players.Values)
		{
			PlayerData playerData = new PlayerData(value, MVGameController.Instance.Game.Friends.GetFriendByProfileID(value.ProfileID));
			MVTeam key = value.Team;
			if (!ShowTeams())
			{
				key = MVTeam.None;
			}
			if (teamToPlayerlist.ContainsKey(key))
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, new List<PlayerData>());
				}
				dictionary[key].Add(playerData);
				if (!playerLines.ContainsKey(value))
				{
					MVGUIPlayerLine mVGUIPlayerLine2 = Object.Instantiate((Object)(object)mVGUIPlayerLine) as MVGUIPlayerLine;
					mVGUIPlayerLine2.InitLine(playerData);
					playerLines.Add(value, mVGUIPlayerLine2);
				}
			}
		}
		return dictionary;
	}
}
