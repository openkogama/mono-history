using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class FriendList
{
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
		if (profileID == MVGameController.Instance.WOCM.LocalPlayer.ProfileID)
		{
			if (!friends.ContainsKey(friendID))
			{
				friends.Add(friendID, new Friend(friendProfileID, status));
			}
		}
		else if (friendProfileID == MVGameController.Instance.WOCM.LocalPlayer.ProfileID && !pending.ContainsKey(friendID))
		{
			pending.Add(friendID, new Friend(profileID, status));
		}
	}

	public void UpdateFriend(int friendID, int profileID, FriendStatus status)
	{
		if (profileID == MVGameController.Instance.WOCM.LocalPlayer.ProfileID)
		{
			if (friends.ContainsKey(friendID))
			{
				friends[friendID].status = status;
			}
			else
			{
				Debug.LogError((object)"Tried updating friend that doesnt exist");
			}
		}
		else
		{
			AddFriend(friendID, MVGameController.Instance.WOCM.LocalPlayer.ProfileID, profileID, status);
			RemoveFromPendingByProfileID(profileID);
		}
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
	}
}
