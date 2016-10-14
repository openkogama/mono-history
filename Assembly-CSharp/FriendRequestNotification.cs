using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestNotification : Notification
{
	[SerializeField]
	private Text label;

	private Friend requester;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		string arg = (string)data[(byte)1];
		label.text = string.Format(TM._(" Accept {0} as friend!"), arg);
		label.text += " <R>";
	}

	public void RegisterFriendshipRequest(Friend friend)
	{
		requester = friend;
	}

	public void AcceptFriendship()
	{
		if (ValidateFriendRequest())
		{
			MVGameControllerBase.OperationRequests.RequestAcceptFriendShip(requester.friendID);
		}
		Close();
	}

	private bool ValidateFriendRequest()
	{
		int level = MVGameControllerBase.Game.LocalPlayer.Level;
		int friendsLimit = BadgeManager.GetFriendsLimit(level);
		int count = MVGameControllerBase.Game.Friends.Friends.Count;
		if (count < friendsLimit)
		{
			return true;
		}
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Your friendlist is full"));
		return false;
	}
}
