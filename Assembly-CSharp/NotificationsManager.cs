using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
	public enum eNotificationPanel
	{
		primary,
		secondary,
		tertiary
	}

	[SerializeField]
	private NotificationArea notificationAreaPrimary;

	[SerializeField]
	private NotificationArea notificationAreaSecondary;

	[SerializeField]
	private NotificationArea notificationAreaTertiary;

	private NotificationArea[] notificationAreas;

	private void Awake()
	{
		notificationAreas = new NotificationArea[3] { notificationAreaPrimary, notificationAreaSecondary, notificationAreaTertiary };
	}

	private void OnEnable()
	{
		NotificationController.Register(this);
	}

	public Notification InstantiateNotification(NotificationType notificationType, eNotificationPanel panel, Dictionary<object, object> data)
	{
		return notificationAreas[(int)panel].InstantiateNotification(notificationType, data);
	}
}
