using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class NotificationAreaSingleInstance : NotificationArea
{
	public override void InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		objectPool.ReturnAllExistingNotifications();
		Notification panel = objectPool.GetPanel(notificationType);
		if (panel == null)
		{
			Debug.LogWarning("Notification is null");
			return;
		}
		panel.transform.SetParent(contentHolderTransform, worldPositionStays: false);
		panel.Initialize(data);
	}
}
