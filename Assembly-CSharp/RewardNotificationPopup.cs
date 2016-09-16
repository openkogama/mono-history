using System.Collections.Generic;

public class RewardNotificationPopup : Notification
{
	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		Lifetime = (NotificationLifetime)(int)data[(byte)2];
	}

	public void OnRewardClicked()
	{
		if (RewardGenerator.OnRewardPressCalled != null)
		{
			RewardGenerator.OnRewardPressCalled();
		}
	}
}
