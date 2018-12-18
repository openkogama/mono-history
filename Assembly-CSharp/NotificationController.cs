using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
	private static HashSet<int> incomingPlayerFriendRequests = new HashSet<int>();

	private static NotificationsManager NotificationsManager => NotificationsManager.ActiveInstance;

	protected void Awake()
	{
		MVGameControllerBase.OnReceivedNotification = (MVGameControllerBase.OnReceivedNotificationEventDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedNotification, new MVGameControllerBase.OnReceivedNotificationEventDelegate(OnNotificationReceived));
	}

	protected void OnDestroy()
	{
		MVGameControllerBase.OnReceivedNotification = (MVGameControllerBase.OnReceivedNotificationEventDelegate)Delegate.Remove(MVGameControllerBase.OnReceivedNotification, new MVGameControllerBase.OnReceivedNotificationEventDelegate(OnNotificationReceived));
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
			NotificationsManager.InstantiateNotification(type, data);
			break;
		}
	}

	private static void FriendRequestAccepted(Dictionary<object, object> data)
	{
		Friend friend = (Friend)data[(byte)10];
		MVPlayer player = null;
		MVGameControllerBase.Game.MVPlayerContainer.TryGetPlayerByProfileId(friend.profileID, out player);
		PushNotification(player.Username + TM._(" accepted your friend request!"));
	}

	private static void ReceivedFriendsRequest(Dictionary<object, object> data)
	{
		List<Friend> list = new List<Friend>(MVGameControllerBase.Game.Friends.Pending.Values);
		Friend friend = list[list.Count - 1];
		if (friend.status == FriendStatus.Pending && MVGameControllerBase.JoinState == MVJoinState.Playing && !incomingPlayerFriendRequests.Contains(friend.profileID))
		{
			incomingPlayerFriendRequests.Add(friend.profileID);
			data.Add((byte)15, friend.friendID);
			NotificationsManager.InstantiateNotification(NotificationType.FriendRequest, data);
		}
	}

	public static void PushNotification(string text, Sprite sprite = null, int lifeTime = 5)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, text);
		dictionary.Add((byte)2, lifeTime);
		if (sprite != null)
		{
			dictionary.Add((byte)3, sprite);
		}
		NotificationsManager.InstantiateNotification(NotificationType.ModalNotification, dictionary);
	}

	public static void PushNotification(NotificationType notificationType, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		Dictionary<object, object> data = new Dictionary<object, object>();
		PushNotification(notificationType, data, lifeTime);
	}

	public static void PushNotification(NotificationType notificationType, Dictionary<object, object> data, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		data.Add((byte)2, lifeTime);
		NotificationsManager.InstantiateNotification(notificationType, data);
	}

	public static void PushNoticationInstruction(string instruction, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, instruction);
		PushNotification(NotificationType.FirstTimeXPRewarded, dictionary, lifeTime);
	}
}
