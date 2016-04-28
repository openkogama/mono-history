using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
	private static NotificationsManager CurrentManager;

	private static List<FriendRequestNotification> FriendRequestQueue = new List<FriendRequestNotification>();

	private static bool hasSubscribed = false;

	public static void Register(NotificationsManager manager)
	{
		CurrentManager = manager;
		if (!hasSubscribed)
		{
			hasSubscribed = true;
			MVGameControllerBase.OnReceivedNotification = (MVGameControllerBase.OnReceivedNotificationEventDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedNotification, new MVGameControllerBase.OnReceivedNotificationEventDelegate(OnNotificationReceived));
		}
	}

	private void Update()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.NotificationAcceptFriendshipRequest) && FriendRequestQueue.Count > 0)
		{
			FriendRequestQueue[FriendRequestQueue.Count - 1].AcceptFriendship();
		}
	}

	public static void OnNotificationReceived(NotificationType type, Dictionary<object, object> data)
	{
		switch (type)
		{
		case NotificationType.FriendRequest:
			ReceivedFriendsRequest(data);
			break;
		case NotificationType.FriendRequestAccepted:
			FriendRequestAccepted(data);
			break;
		default:
			CurrentManager.InstantiateNotification(type, data);
			break;
		}
	}

	private static void FriendRequestAccepted(Dictionary<object, object> data)
	{
		Friend friend = (Friend)data[(byte)10];
		MVPlayer mVPlayer = null;
		foreach (MVPlayer value in MVGameControllerBase.Game.Players.Values)
		{
			if (friend.profileID == value.ProfileID)
			{
				mVPlayer = value;
				break;
			}
		}
		PushNotification(mVPlayer.Username + TM._(" accepted your friend request!"));
	}

	private static void ReceivedFriendsRequest(Dictionary<object, object> data)
	{
		List<Friend> list = new List<Friend>(MVGameControllerBase.Game.Friends.Pending.Values);
		Friend friend = list[list.Count - 1];
		if (friend.status == FriendStatus.Pending && MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			FriendRequestNotification friendRequestNotification = (FriendRequestNotification)CurrentManager.InstantiateNotification(NotificationType.FriendRequest, data);
			friendRequestNotification.RegisterFriendshipRequest(friend);
			friendRequestNotification.OnNotificationClosedEnd += UnregisterFriendRequestNotification;
			FriendRequestQueue.Add(friendRequestNotification);
		}
	}

	public static void PushNotification(string text, Sprite sprite = null, int lifeTime = 5)
	{
		PushNotification(text, -1f, sprite, lifeTime);
	}

	public static void PushNotification(string text, float xScale, Sprite sprite = null, int lifeTime = 5)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, text);
		dictionary.Add((byte)2, lifeTime);
		dictionary.Add((byte)4, xScale);
		if (sprite != null)
		{
			dictionary.Add((byte)3, sprite);
		}
		CurrentManager.InstantiateNotification(NotificationType.ModalNotification, dictionary);
	}

	private static void UnregisterFriendRequestNotification(Notification notification)
	{
		FriendRequestNotification item = (FriendRequestNotification)notification;
		FriendRequestQueue.Remove(item);
	}
}
