using System.Collections.Generic;

public class GoldRewardNotification : Notification
{
	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		Lifetime = (NotificationLifetime)(int)data[(byte)2];
	}

	public void RewardClicked()
	{
		if (!TimedPlayReward.IsCollected && TimedPlayReward.CollectedChanged != null)
		{
			TimedPlayReward.CollectedChanged();
		}
	}
}
