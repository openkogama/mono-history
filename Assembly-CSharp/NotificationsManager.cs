using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
	public enum eNotificationPanel
	{
		primary,
		secondary,
		tertiary,
		custom
	}

	[SerializeField]
	private NotificationArea notificationAreaPrimary;

	[SerializeField]
	private NotificationArea notificationAreaSecondary;

	[SerializeField]
	private NotificationArea notificationAreaTertiary;

	[SerializeField]
	private NotificationArea notificationAreaCustom;

	private NotificationArea[] notificationAreas;

	private void Awake()
	{
		notificationAreas = new NotificationArea[4] { notificationAreaPrimary, notificationAreaSecondary, notificationAreaTertiary, notificationAreaCustom };
	}

	private void OnEnable()
	{
		NotificationController.Register(this);
	}

	public void InstantiateNotification(NotificationType notificationType, eNotificationPanel panel, Dictionary<object, object> data)
	{
		notificationAreas[(int)panel].InstantiateNotification(notificationType, data);
	}
}
