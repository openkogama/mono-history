using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
	[SerializeField]
	private NotificationArea[] notificationAreas;

	public static NotificationsManager ActiveInstance { get; private set; }

	protected void OnEnable()
	{
		ActiveInstance = this;
	}

	protected void OnDestroy()
	{
		ActiveInstance = null;
	}

	public void InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		for (int i = 0; i < notificationAreas.Length; i++)
		{
			if (notificationAreas[i].CanInstantiateNotificationType(notificationType))
			{
				notificationAreas[i].InstantiateNotification(notificationType, data);
				return;
			}
		}
		Debug.LogError("Could not find a notification area who could instantiate notification of type " + notificationType);
	}
}
