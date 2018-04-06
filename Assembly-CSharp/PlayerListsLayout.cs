using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerListsLayout : MonoBehaviour
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

	private PlayerListsLayout playerListsPrefab;

	[SerializeField]
	private PlayerList playerListPrefab;

	[SerializeField]
	private GameObject topGrid;

	[SerializeField]
	private GameObject bottomGrid;

	private GameStatCounterType typeToDisplay;

	public void Initialize(PlayerListsLayout playerListsPrefab, GameStatCounterType typeToDisplay)
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
		if (MVGameControllerBase.Game != null)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(ReCreate));
			FriendList friends = MVGameControllerBase.Game.Friends;
			friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Remove(friends.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(ReCreate));
		}
	}

	private void ReCreate()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		PlayerListsLayout newPlayerLists = UnityEngine.Object.Instantiate(playerListsPrefab);
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
		bottomGrid.SetActive(value: false);
		Dictionary<MVTeam, PlayerList> dictionary = new Dictionary<MVTeam, PlayerList>();
		if (flag)
		{
			PlayerList playerList = CreatePlayerList(MVTeam.None, 0);
			dictionary.Add(MVTeam.None, playerList);
			playerList.transform.SetParent(topGrid.transform, worldPositionStays: false);
		}
		else
		{
			bottomGrid.SetActive(value: true);
			for (int i = 0; i < teams.Count; i++)
			{
				PlayerList playerList2 = CreatePlayerList(teams[i], MVGameControllerBase.Game.TeamManager.GetScore(teams[i], typeToDisplay));
				if (i > 1)
				{
					playerList2.transform.SetParent(bottomGrid.transform, worldPositionStays: false);
				}
				else
				{
					playerList2.transform.SetParent(topGrid.transform, worldPositionStays: false);
				}
				dictionary.Add(teams[i], playerList2);
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

	private PlayerList CreatePlayerList(MVTeam team, int score)
	{
		PlayerList playerList = UnityEngine.Object.Instantiate(playerListPrefab);
		playerList.gameObject.SetActive(value: true);
		playerList.Initialize(team, score, typeToDisplay);
		return playerList;
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

	private void SortPlayerListsAfterScore(Dictionary<MVTeam, PlayerList> playerLists, List<MVTeam> teams, GameStatCounterType typeToDisplay)
	{
		List<PlayerList> list = new List<PlayerList>();
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
}
