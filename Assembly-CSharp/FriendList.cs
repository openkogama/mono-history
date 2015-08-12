using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class FriendList
{
	public delegate void OnFriendListUpdatedDelegate();

	public OnFriendListUpdatedDelegate OnFriendListUpdated;

	private Dictionary<int, Friend> friends = new Dictionary<int, Friend>();

	private Dictionary<int, Friend> pending = new Dictionary<int, Friend>();

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

	public void AddFriend(int friendID, int profileID, int friendProfileID, FriendStatus status)
	{
		if (profileID == MVGameController.Game.LocalPlayer.ProfileID)
		{
			if (!friends.ContainsKey(friendID))
			{
				Friend value = new Friend(friendID, friendProfileID, status);
				friends.Add(friendID, value);
			}
		}
		else if (friendProfileID == MVGameController.Game.LocalPlayer.ProfileID && !pending.ContainsKey(friendID))
		{
			Friend value2 = new Friend(friendID, profileID, status);
			pending.Add(friendID, value2);
		}
		if (OnFriendListUpdated != null)
		{
			OnFriendListUpdated();
		}
	}

	public void UpdateFriend(int friendID, int profileID, FriendStatus status)
	{
		if (profileID == MVGameController.Game.LocalPlayer.ProfileID)
		{
			if (friends.ContainsKey(friendID))
			{
				if (status == FriendStatus.Deleted)
				{
					friends.Remove(friendID);
				}
				else
				{
					friends[friendID].status = status;
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
				AddFriend(friendID, MVGameController.Game.LocalPlayer.ProfileID, profileID, status);
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
		foreach (KeyValuePair<int, MVPlayer> player in MVGameController.Game.Players)
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
