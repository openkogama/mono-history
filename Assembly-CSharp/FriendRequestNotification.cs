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
		label.text = text + TM._(" sent you a friend request!");
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
		int level2 = level + 1;
		int friendsLimit2 = BadgeManager.GetFriendsLimit(level2);
		string text = TM._("You can only have {0} friends at level {1}. Get to level {2} and you can have {3} friends.");
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Your friendlist is full"));
		return false;
	}
}
