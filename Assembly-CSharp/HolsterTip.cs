using System.Collections.Generic;
using MV.Common;

public class HolsterTip
{
	private enum HolsterState
	{
		NotSet,
		Holstered,
		Unholstered
	}

	private HolsterState holsterState;

	public void SetHolsterState(bool itemIsHolstered)
	{
		HolsterState holsterState = (itemIsHolstered ? HolsterState.Holstered : HolsterState.Unholstered);
		if (this.holsterState != holsterState)
		{
			this.holsterState = holsterState;
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)1, TM._("Press V or use scrollwheel to toggle Holster."));
			Dictionary<object, object> data = dictionary;
			NotificationController.PushNotification(NotificationType.PlayerTip, NotificationsManager.eNotificationPanel.secondary, data);
		}
	}
}
