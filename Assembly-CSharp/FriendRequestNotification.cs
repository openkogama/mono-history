using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestNotification : Notification
{
	[SerializeField]
	private Text label;

	[SerializeField]
	private Button acceptFriendship;

	private Friend requester;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	private void Awake()
	{
		acceptFriendship.onClick.AddListener(AcceptFriendship);
	}

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		string text = (string)data[(byte)1];
		label.text = text + " sent you a friend request!";
	}

	public void RegisterFriendshipRequest(Friend friend)
	{
		requester = friend;
	}

	public void AcceptFriendship()
	{
		ValidateFriendRequest();
		MVGameControllerBase.OperationRequests.RequestAcceptFriendShip(requester.friendID);
		Close();
	}

	private void ValidateFriendRequest()
	{
		int level = MVGameControllerBase.Game.LocalPlayer.Level;
		int friendsLimit = BadgeManager.GetFriendsLimit(level);
		int count = MVGameControllerBase.Game.Friends.Friends.Count;
		if (count < friendsLimit)
		{
			return;
		}
		int num = level + 1;
		int friendsLimit2 = BadgeManager.GetFriendsLimit(num);
		string format = TM._("You can only have {0} friends at level {1}. Get to level {2} and you can have {3} friends.");
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Your friendlist is full"));
		throw new Exception(string.Format(format, friendsLimit, level, num, friendsLimit2));
	}
}
