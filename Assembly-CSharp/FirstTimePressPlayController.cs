using System.Collections.Generic;
using MV.Common;

public static class FirstTimePressPlayController
{
	private static bool haveBeenPressed;

	public static bool HaveBeenPressed => haveBeenPressed;

	public static void OnFirstTimePlayIsPressed()
	{
		haveBeenPressed = true;
		FpsCounter.StartedPlaying = true;
		Dictionary<object, object> data = new Dictionary<object, object>();
		NotificationController.PushNotification(NotificationType.XPBoost, data);
	}
}
