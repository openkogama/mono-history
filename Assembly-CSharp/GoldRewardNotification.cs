using System.Collections.Generic;

public class GoldRewardNotification : Notification
{
	private NotificationLifetime lifeTime;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		lifeTime = (NotificationLifetime)(int)data[(byte)2];
	}

	public void RewardClicked()
	{
		if (!TimedPlayReward.IsCollected)
		{
			if (TimedPlayReward.CollectedChanged != null)
			{
				TimedPlayReward.CollectedChanged();
			}
			Close();
		}
	}
}
