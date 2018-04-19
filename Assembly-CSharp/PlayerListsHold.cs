using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerListsHold : MonoBehaviour
{
	private class ScoreSorter : IComparer<MVPlayer>
	{
		public int Compare(MVPlayer data1, MVPlayer data2)
		{
			return PlayerValue(data2) - PlayerValue(data1);
		}

		private int PlayerValue(MVPlayer data)
		{
			return data?.GetGameStat(GameStatCounterType.Kill) ?? 0;
		}
	}

	private PlayerListsHold playerListsPrefab;

	[SerializeField]
	private PlayerListHold playerListPrefab;

	[SerializeField]
	private GridLayoutGroup gridGroup;

	[SerializeField]
	private Vector2 cellSize1Team;

	[SerializeField]
	private Vector2 cellSize2Teams;

	[SerializeField]
	private Vector2 cellSize4Teams;

	private GameStatCounterType typeToDisplay;

	public void Initialize(PlayerListsHold playerListsPrefab, GameStatCounterType typeToDisplay)
	{
		this.playerListsPrefab = playerListsPrefab;
		this.typeToDisplay = typeToDisplay;
	}

	private void Start()
	{
		IEnumerable<MVPlayer> values = MVGameControllerBase.Game.MVPlayerContainer.Values;
		List<MVTeam> teamList = MVGameControllerBase.Game.TeamManager.GetTeamList();
		CreatePlayerLists(values, teamList);
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(ReCreate));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Combine(friends.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(ReCreate));
	}

	private void OnDestroy()
	{
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(ReCreate));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Remove(friends.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(ReCreate));
	}

	private void ReCreate()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		PlayerListsHold newPlayerLists = UnityEngine.Object.Instantiate(playerListsPrefab);
		newPlayerLists.Initialize(playerListsPrefab, typeToDisplay);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newPlayerLists.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	private void CreatePlayerLists(IEnumerable<MVPlayer> players, List<MVTeam> teams)
	{
		int count = teams.Count;
		bool flag = false;
		if (count == 1)
		{
			flag = true;
		}
		if (count > 2)
		{
			gridGroup.cellSize = cellSize4Teams;
		}
		else if (count > 1)
		{
			gridGroup.cellSize = cellSize2Teams;
		}
		else
		{
			gridGroup.cellSize = cellSize1Team;
		}
		Dictionary<MVTeam, PlayerListHold> dictionary = new Dictionary<MVTeam, PlayerListHold>();
		if (flag)
		{
			dictionary.Add(MVTeam.None, CreatePlayerList(MVTeam.None, 0));
		}
		else
		{
			foreach (MVTeam team in teams)
			{
				dictionary.Add(team, CreatePlayerList(team, MVGameControllerBase.Game.TeamManager.GetScore(team, typeToDisplay)));
			}
			SortPlayerListsAfterScore(dictionary, teams, typeToDisplay);
		}
		if (count <= 0)
		{
			return;
		}
		Dictionary<MVTeam, List<MVPlayer>> sortedTeamLists = GetSortedTeamLists(players, teams);
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in sortedTeamLists)
		{
			foreach (MVPlayer item2 in item.Value)
			{
				MVTeam mVTeam = item2.Team;
				if (flag)
				{
					mVTeam = MVTeam.None;
				}
				else if (mVTeam == MVTeam.None && !flag)
				{
					continue;
				}
				dictionary[mVTeam].Add(item2);
			}
		}
	}

	private PlayerListHold CreatePlayerList(MVTeam team, int score)
	{
		PlayerListHold playerListHold = UnityEngine.Object.Instantiate(playerListPrefab);
		playerListHold.transform.SetParent(gridGroup.transform, worldPositionStays: false);
		playerListHold.gameObject.SetActive(value: true);
		playerListHold.Initialize(team, score, typeToDisplay);
		return playerListHold;
	}

	private void SortPlayerListsAfterScore(Dictionary<MVTeam, PlayerListHold> playerLists, List<MVTeam> teams, GameStatCounterType typeToDisplay)
	{
		List<PlayerListHold> list = new List<PlayerListHold>();
		foreach (MVTeam team in teams)
		{
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				if (!flag)
				{
					int score = MVGameControllerBase.Game.TeamManager.GetScore(team, typeToDisplay);
					int score2 = MVGameControllerBase.Game.TeamManager.GetScore(list[i].Team, typeToDisplay);
					if (WinningConditionControl.IsNewScoreBetter(score, score2, typeToDisplay))
					{
						list.Insert(i, playerLists[team]);
						flag = true;
					}
				}
				else
				{
					list[i].transform.SetAsLastSibling();
				}
			}
			if (!flag)
			{
				list.Add(playerLists[team]);
				playerLists[team].transform.SetAsLastSibling();
			}
		}
	}

	private Dictionary<MVTeam, List<MVPlayer>> GetSortedTeamLists(IEnumerable<MVPlayer> players, List<MVTeam> teams)
	{
		Dictionary<MVTeam, List<MVPlayer>> dictionary = new Dictionary<MVTeam, List<MVPlayer>>();
		foreach (MVTeam team2 in teams)
		{
			dictionary.Add(team2, new List<MVPlayer>());
		}
		foreach (MVPlayer player in players)
		{
			MVTeam team = player.Team;
			if (team != MVTeam.None)
			{
				dictionary[team].Add(player);
			}
		}
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in dictionary)
		{
			item.Value.Sort(new ScoreSorter());
		}
		return dictionary;
	}
}
