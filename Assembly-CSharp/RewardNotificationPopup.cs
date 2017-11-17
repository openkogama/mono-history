using System.Collections.Generic;

public class RewardNotificationPopup : Notification
{
	private NotificationLifetime lifeTime;

	protected override NotificationLifetime Lifetime => lifeTime;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		lifeTime = (NotificationLifetime)data[(byte)2];
	}

	public void OnRewardClicked()
	{
		if (RewardGenerator.OnRewardPressCalled != null)
		{
			RewardGenerator.OnRewardPressCalled();
		}
	}
}
