using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class FriendList
{
	public delegate void OnFriendListUpdatedDelegate();

	public delegate void OnFriendRequestUpdated(Friend friend);

	public OnFriendListUpdatedDelegate OnFriendListUpdated;

	public OnFriendRequestUpdated OnFriendRequestAccepted;

	public UnityAction OnFriendRequestReceived;

	public UnityAction<int> OnPendingCountChanged;

	private Dictionary<int, Friend> friends = new Dictionary<int, Friend>();

	private Dictionary<int, Friend> pending = new Dictionary<int, Friend>();

	private List<int> pendingNotifications = new List<int>();

	public Dictionary<int, Friend> Friends => friends;

	public Dictionary<int, Friend> Pending => pending;

	public Friend this[int friendID]
	{
		get
		{
			if (!friends.ContainsKey(friendID))
			{
				if (!pending.ContainsKey(friendID))
				{
					return null;
				}
				return pending[friendID];
			}
			return friends[friendID];
		}
	}

	public FriendList()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnFinishedLoadingPlayers = (UnityAction)Delegate.Combine(game.OnFinishedLoadingPlayers, new UnityAction(OnPlayersLoaded));
	}

	private void OnPlayersLoaded()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnFinishedLoadingPlayers = (UnityAction)Delegate.Remove(game.OnFinishedLoadingPlayers, new UnityAction(OnPlayersLoaded));
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			if (pendingNotifications.Contains(value.ProfileID) && OnFriendRequestReceived != null)
			{
				OnFriendRequestReceived();
			}
		}
	}

	public void AddFriend(int friendID, int profileID, int friendProfileID, FriendStatus status)
	{
		if (profileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			if (!friends.ContainsKey(friendID))
			{
				Friend value = new Friend(friendID, friendProfileID, status);
				friends.Add(friendID, value);
			}
		}
		else if (friendProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID && !pending.ContainsKey(friendID))
		{
			Friend value2 = new Friend(friendID, profileID, status);
			pending.Add(friendID, value2);
			pendingNotifications.Add(profileID);
			foreach (MVPlayer value3 in MVGameControllerBase.Game.Players.Values)
			{
				if (value3.ProfileID == profileID && OnFriendRequestReceived != null)
				{
					OnFriendRequestReceived();
				}
			}
		}
		if (OnFriendListUpdated != null)
		{
			OnFriendListUpdated();
		}
	}

	public void UpdateFriend(int friendID, int profileID, FriendStatus status)
	{
		if (profileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			if (friends.ContainsKey(friendID))
			{
				if (status == FriendStatus.Deleted)
				{
					friends.Remove(friendID);
				}
				else
				{
					if (OnFriendRequestAccepted != null)
					{
						OnFriendRequestAccepted(friends[friendID]);
					}
					friends[friendID].status = status;
					Dictionary<object, object> dictionary = new Dictionary<object, object>();
					dictionary.Add((byte)10, friends[friendID]);
					Dictionary<object, object> data = dictionary;
					NotificationController.OnNotificationReceived(NotificationType.FriendRequestAccepted, data);
				}
			}
			else
			{
				Debug.LogError("Tried updating friend that doesnt exist");
			}
		}
		else
		{
			RemoveFromPendingByProfileID(profileID);
			if (status != FriendStatus.Deleted)
			{
				if (OnPendingCountChanged != null)
				{
					OnPendingCountChanged(GetOnlineFriends().Count);
				}
				NotificationController.PushNotification(string.Format(TM._("Accepted friend request from {0}."), MVGameControllerBase.Game.LocalPlayer.Username), null, 3);
				AddFriend(friendID, MVGameControllerBase.Game.LocalPlayer.ProfileID, profileID, status);
			}
		}
		if (OnFriendListUpdated != null)
		{
			OnFriendListUpdated();
		}
	}

	public bool IsFriend(int profileID)
	{
		Friend friendByProfileID = GetFriendByProfileID(profileID);
		return friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted;
	}

	public Friend GetFriendByProfileID(int profileID)
	{
		if (friends.Any((KeyValuePair<int, Friend> f) => f.Value.profileID == profileID))
		{
			return friends.First((KeyValuePair<int, Friend> f) => f.Value.profileID == profileID).Value;
		}
		if (pending.Any((KeyValuePair<int, Friend> f) => f.Value.profileID == profileID))
		{
			return pending.First((KeyValuePair<int, Friend> f) => f.Value.profileID == profileID).Value;
		}
		return null;
	}

	public Dictionary<int, MVPlayer> GetOnlineFriends()
	{
		Dictionary<int, MVPlayer> dictionary = new Dictionary<int, MVPlayer>();
		foreach (KeyValuePair<int, MVPlayer> player in MVGameControllerBase.Game.Players)
		{
			if (IsFriend(player.Value.ProfileID))
			{
				dictionary.Add(player.Key, player.Value);
			}
		}
		return dictionary;
	}

	private void RemoveFromPendingByProfileID(int profileID)
	{
		foreach (int key in pending.Keys)
		{
			Friend friend = pending[key];
			if (friend.profileID == profileID)
			{
				pending.Remove(key);
				break;
			}
		}
		if (OnFriendListUpdated != null)
		{
			OnFriendListUpdated();
		}
	}
}
