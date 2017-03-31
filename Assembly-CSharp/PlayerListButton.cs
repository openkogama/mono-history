using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerListButton : MonoBehaviour
{
	[SerializeField]
	private PlayerLists playerListsPrefab;

	[SerializeField]
	private Button button;

	[SerializeField]
	private Text playersCount;

	[SerializeField]
	private Image notification;

	private GameObject currPlayerLists;

	private Dictionary<int, MVPlayer> prevPlayerListState = new Dictionary<int, MVPlayer>();

	private void Awake()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdateButton));
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(UpdateButton));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendRequestReceived = (UnityAction)Delegate.Combine(friends.OnFriendRequestReceived, new UnityAction(ViewNotification));
		FriendList friends2 = MVGameControllerBase.Game.Friends;
		friends2.OnPendingCountChanged = (UnityAction<int>)Delegate.Combine(friends2.OnPendingCountChanged, new UnityAction<int>(PendingCountChanged));
		MVNetworkGame game2 = MVGameControllerBase.Game;
		game2.OnFinishedLoadingPlayers = (UnityAction)Delegate.Combine(game2.OnFinishedLoadingPlayers, new UnityAction(OnPlayerListReady));
		UpdateButton();
	}

	private void OnPlayerListReady()
	{
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			if (MVGameControllerBase.Game.Friends.Pending.ContainsKey(value.ProfileID))
			{
				notification.gameObject.SetActive(value: true);
				break;
			}
		}
	}

	private void PendingCountChanged(int pending)
	{
		notification.gameObject.SetActive(pending > 0);
	}

	private void ViewNotification()
	{
		notification.gameObject.SetActive(value: true);
	}

	public void CreatePlayerList()
	{
		prevPlayerListState = new Dictionary<int, MVPlayer>(MVGameControllerBase.Game.Players);
		notification.gameObject.SetActive(value: false);
		if (currPlayerLists != null)
		{
			currPlayerLists = null;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
			});
			return;
		}
		PlayerLists playerLists = UnityEngine.Object.Instantiate(playerListsPrefab);
		currPlayerLists = playerLists.gameObject;
		playerLists.Initialize(playerListsPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(currPlayerLists.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	private void UpdateButton()
	{
		UpdatePlayersCount();
		UpdateTeamColor();
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			if (MVGameControllerBase.Game.Friends.PendingProfileIds.Contains(value.ProfileID) && !prevPlayerListState.ContainsKey(value.ProfileID))
			{
				notification.gameObject.SetActive(value: true);
				break;
			}
		}
	}

	private void UpdatePlayersCount()
	{
		playersCount.text = MVGameControllerBase.Game.Players.Count.ToString();
	}

	private void UpdateTeamColor()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() <= 1)
		{
			Styles.SetStyle(button, ButtonStyle.RegularButton, MVTeam.None);
			return;
		}
		MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
		Styles.SetStyle(button, ButtonStyle.RegularButton, team);
	}
}
