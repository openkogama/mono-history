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

	public void Initialize(PlayerListsHold playerListsPrefab)
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
		PlayerListsHold newPlayerLists = UnityEngine.Object.Instantiate(playerListsPrefab);
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
		if (teams.Count > 2)
		{
			gridGroup.cellSize = cellSize4Teams;
		}
		else if (teams.Count > 1)
		{
			gridGroup.cellSize = cellSize2Teams;
		}
		else
		{
			gridGroup.cellSize = cellSize1Team;
		}
		Dictionary<MVTeam, PlayerListHold> dictionary = new Dictionary<MVTeam, PlayerListHold>();
		foreach (MVTeam team in teams)
		{
			PlayerListHold playerListHold = UnityEngine.Object.Instantiate(playerListPrefab);
			playerListHold.transform.SetParent(transform, worldPositionStays: false);
			playerListHold.gameObject.SetActive(value: true);
			if (teams.Count == 1)
			{
				playerListHold.Initialize(MVTeam.None, 0);
			}
			else
			{
				playerListHold.Initialize(team, MVGameControllerBase.Game.TeamManager.GetScore(team, GameStatCounterType.Kill));
			}
			dictionary.Add(team, playerListHold);
		}
		Dictionary<MVTeam, List<MVPlayer>> sortedTeamLists = GetSortedTeamLists(players, teams);
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in sortedTeamLists)
		{
			foreach (MVPlayer item2 in item.Value)
			{
				dictionary[item2.Team].Add(item2);
			}
		}
	}

	private Dictionary<MVTeam, List<MVPlayer>> GetSortedTeamLists(IEnumerable<MVPlayer> players, List<MVTeam> teams)
	{
		Dictionary<MVTeam, List<MVPlayer>> dictionary = new Dictionary<MVTeam, List<MVPlayer>>();
		foreach (MVTeam team in teams)
		{
			dictionary.Add(team, new List<MVPlayer>());
		}
		foreach (MVPlayer player in players)
		{
			dictionary[player.Team].Add(player);
		}
		foreach (KeyValuePair<MVTeam, List<MVPlayer>> item in dictionary)
		{
			item.Value.Sort(new ScoreSorter());
		}
		return dictionary;
	}
}
