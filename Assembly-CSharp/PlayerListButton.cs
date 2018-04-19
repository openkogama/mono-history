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
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(UpdateButton));
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(UpdateButton));
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendRequestReceived = (UnityAction)Delegate.Combine(friends.OnFriendRequestReceived, new UnityAction(ViewNotification));
		FriendList friends2 = MVGameControllerBase.Game.Friends;
		friends2.OnPendingCountChanged = (UnityAction<int>)Delegate.Combine(friends2.OnPendingCountChanged, new UnityAction<int>(PendingCountChanged));
		MVPlayerContainer mVPlayerContainer2 = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer2.OnPlayerListLoaded = (Action)Delegate.Combine(mVPlayerContainer2.OnPlayerListLoaded, new Action(OnPlayerListReady));
		UpdateButton();
	}

	private void OnPlayerListReady()
	{
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListLoaded = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListLoaded, new Action(OnPlayerListReady));
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
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
		prevPlayerListState = new Dictionary<int, MVPlayer>();
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			prevPlayerListState.Add(value.ActorNr, value);
		}
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
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (MVGameControllerBase.Game.Friends.PendingProfileIds.Contains(value.ProfileID) && !prevPlayerListState.ContainsKey(value.ProfileID))
			{
				notification.gameObject.SetActive(value: true);
				break;
			}
		}
		prevPlayerListState = new Dictionary<int, MVPlayer>();
		foreach (MVPlayer value2 in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			prevPlayerListState.Add(value2.ActorNr, value2);
		}
	}

	private void UpdatePlayersCount()
	{
		playersCount.text = MVGameControllerBase.Game.MVPlayerContainer.Count.ToString();
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
