using System.Collections.Generic;
using MV.Common;

public static class HolsterTip
{
	public static void Show()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)1, TM._("Press <Q> to toggle Holster. Press <V> to drop."));
		Dictionary<object, object> data = dictionary;
		NotificationController.PushNotification(NotificationType.PlayerTip, NotificationsManager.eNotificationPanel.secondary, data);
	}
}
