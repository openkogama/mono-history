using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerLists : MonoBehaviour
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

	private PlayerLists playerListsPrefab;

	[SerializeField]
	private PlayerList playerListPrefab;

	[SerializeField]
	private GridLayoutGroup gridGroup;

	[SerializeField]
	private Vector2 cellSize1Team;

	[SerializeField]
	private Vector2 cellSize2Teams;

	[SerializeField]
	private Vector2 cellSize4Teams;

	private MVTeam defaultTeam = MVTeam.None;

	public void Initialize(PlayerLists playerListsPrefab)
	{
		this.playerListsPrefab = playerListsPrefab;
	}

	private void Start()
	{
		IEnumerable<MVPlayer> values = MVGameControllerBase.Game.Players.Values;
		List<MVTeam> teamList = MVGameControllerBase.Game.TeamManager.GetTeamList();
		CreatePlayerLists(values, teamList);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(ReCreate));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Combine(friends.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(ReCreate));
	}

	private void OnDestroy()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Remove(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(ReCreate));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Remove(friends.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(ReCreate));
	}

	private void ReCreate()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		PlayerLists newPlayerLists = UnityEngine.Object.Instantiate(playerListsPrefab);
		newPlayerLists.Initialize(playerListsPrefab);
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
		bool flag = teams.Contains(MVTeam.None);
		int num = teams.Count;
		if (flag && num > 1)
		{
			teams.Remove(MVTeam.None);
			flag = false;
			num--;
		}
		defaultTeam = teams[0];
		if (num > 2)
		{
			gridGroup.cellSize = cellSize4Teams;
		}
		else if (num > 1)
		{
			gridGroup.cellSize = cellSize2Teams;
		}
		else
		{
			gridGroup.cellSize = cellSize1Team;
		}
		Dictionary<MVTeam, PlayerList> dictionary = new Dictionary<MVTeam, PlayerList>();
		if (flag)
		{
			dictionary.Add(MVTeam.None, CreatePlayerList(MVTeam.None, 0));
		}
		else
		{
			foreach (MVTeam team2 in teams)
			{
				dictionary.Add(team2, CreatePlayerList(team2, MVGameControllerBase.Game.TeamManager.GetScore(team2, GameStatCounterType.Kill)));
			}
		}
		if (num <= 0)
		{
			return;
		}
		Dictionary<MVTeam, List<MVPlayer>> sortedTeamLists = GetSortedTeamLists(players, teams);
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in sortedTeamLists)
		{
			foreach (MVPlayer item2 in item.Value)
			{
				MVTeam team = item2.Team;
				if (team == MVTeam.None)
				{
					team = defaultTeam;
				}
				dictionary[team].Add(item2);
			}
		}
	}

	private PlayerList CreatePlayerList(MVTeam team, int score)
	{
		PlayerList playerList = UnityEngine.Object.Instantiate(playerListPrefab);
		playerList.transform.SetParent(gridGroup.transform, worldPositionStays: false);
		playerList.gameObject.SetActive(value: true);
		playerList.Initialize(team, score);
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
			if (team == MVTeam.None)
			{
				team = defaultTeam;
			}
			dictionary[team].Add(player);
		}
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in dictionary)
		{
			item.Value.Sort(new ScoreSorter());
		}
		return dictionary;
	}
}
