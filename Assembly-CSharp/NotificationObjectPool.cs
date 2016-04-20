using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationObjectPool : MonoBehaviour
{
	[SerializeField]
	private LevelUpNotification LevelUpNotification;

	[SerializeField]
	private KillNotification KillNotification;

	[SerializeField]
	private XPNotification XPNotification;

	[SerializeField]
	private PlayerJoinedNotification PlayerJoinedNotification;

	[SerializeField]
	private PlayerLeftNotification PlayerLeftNotification;

	[SerializeField]
	private FriendRequestNotification FriendRequestNotification;

	[SerializeField]
	private ModalNotification ModalNotification;

	[SerializeField]
	private List<Notification> elements = new List<Notification>();

	public Notification GetPanel(NotificationType type)
	{
		if (!elements.Find((Notification x) => (byte)x.Type == (byte)type))
		{
			return CreateTempPanel(type);
		}
		int index = elements.FindIndex((Notification x) => (byte)x.Type == (byte)type);
		Notification notification = elements[index];
		notification.SelfDestroy = false;
		notification.pool = this;
		notification.gameObject.SetActive(value: true);
		elements.Remove(elements[index]);
		return notification;
	}

	public void Return(Notification notification)
	{
		notification.gameObject.SetActive(value: false);
		notification.transform.SetParent(transform);
		elements.Add(notification);
	}

	private Notification CreateTempPanel(NotificationType type)
	{
		Notification notification = null;
		switch (type)
		{
		case NotificationType.Kill:
			notification = Object.Instantiate(KillNotification);
			break;
		case NotificationType.FriendRequest:
			notification = Object.Instantiate(FriendRequestNotification);
			break;
		case NotificationType.PlayerJoined:
			notification = Object.Instantiate(PlayerJoinedNotification);
			break;
		case NotificationType.PlayerLeft:
			notification = Object.Instantiate(PlayerLeftNotification);
			break;
		case NotificationType.XP:
			notification = Object.Instantiate(XPNotification);
			break;
		case NotificationType.ModalNotification:
			notification = Object.Instantiate(ModalNotification);
			break;
		case NotificationType.LevelUp:
			notification = Object.Instantiate(LevelUpNotification);
			break;
		}
		if (notification == null)
		{
			Debug.LogError("Couldn't find notification type " + type);
		}
		return notification;
	}
}
