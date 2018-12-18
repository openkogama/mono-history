using System.Collections.Generic;

public class GoldRewardNotification : Notification
{
	private NotificationLifetime lifeTime;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		lifeTime = (NotificationLifetime)data[(byte)2];
	}

	public void RewardClicked()
	{
		if (!TimedPlayReward.RewardTracker.IsCollected)
		{
			if (TimedPlayReward.RewardTracker.CollectedChanged != null)
			{
				TimedPlayReward.RewardTracker.CollectedChanged();
			}
			Close();
		}
	}
}
