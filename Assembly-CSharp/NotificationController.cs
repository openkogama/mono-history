using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
	private static NotificationsManager CurrentManager;

	private static bool hasSubscribed = false;

	private static HashSet<int> incomingPlayerFriendRequests = new HashSet<int>();

	public static void Register(NotificationsManager manager)
	{
		CurrentManager = manager;
		if (!hasSubscribed)
		{
			hasSubscribed = true;
			MVGameControllerBase.OnReceivedNotification = (MVGameControllerBase.OnReceivedNotificationEventDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedNotification, new MVGameControllerBase.OnReceivedNotificationEventDelegate(OnNotificationReceived));
		}
	}

	public static void OnNotificationReceived(NotificationType type, Dictionary<object, object> data, NotificationsManager.eNotificationPanel panel = NotificationsManager.eNotificationPanel.tertiary)
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
			CurrentManager.InstantiateNotification(type, panel, data);
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
			CurrentManager.InstantiateNotification(NotificationType.FriendRequest, NotificationsManager.eNotificationPanel.tertiary, data);
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
		CurrentManager.InstantiateNotification(NotificationType.ModalNotification, NotificationsManager.eNotificationPanel.tertiary, dictionary);
	}

	public static void PushNotification(NotificationType notificationType, NotificationsManager.eNotificationPanel notificationPriority, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		Dictionary<object, object> data = new Dictionary<object, object>();
		PushNotification(notificationType, notificationPriority, data, lifeTime);
	}

	public static void PushNotification(NotificationType notificationType, NotificationsManager.eNotificationPanel notificationPriority, Dictionary<object, object> data, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		data.Add((byte)2, lifeTime);
		CurrentManager.InstantiateNotification(notificationType, notificationPriority, data);
	}

	public static void PushNoticationInstruction(string instruction, NotificationLifetime lifeTime = NotificationLifetime.High)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, instruction);
		PushNotification(NotificationType.FirstTimeXPRewarded, NotificationsManager.eNotificationPanel.custom, dictionary, lifeTime);
	}
}
