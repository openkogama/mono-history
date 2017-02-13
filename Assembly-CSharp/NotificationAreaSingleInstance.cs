using System.Collections.Generic;
using MV.Common;

public class NotificationAreaSingleInstance : NotificationArea
{
	public override Notification InstantiateNotification(NotificationType notificationType, Dictionary<object, object> data)
	{
		objectPool.ReturnAllExistingNotifications();
		Notification panel = objectPool.GetPanel(notificationType);
		panel.transform.SetParent(contentHolderTransform, worldPositionStays: false);
		panel.Initialize(data);
		return panel;
	}
}
