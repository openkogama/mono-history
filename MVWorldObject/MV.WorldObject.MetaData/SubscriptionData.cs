using System;
using MV.WorldObject.Subscription;

namespace MV.WorldObject.MetaData;

public class SubscriptionData
{
	public SubscriptionType SubscriptionType;

	public SubscriptionType ExpiredSubscriptionType;

	public DateTime SubscriptionActivateTime;

	public TimeSpan SubscriptionDuration;

	public static SubscriptionData GetTouristProfileData()
	{
		SubscriptionData subscriptionData = new SubscriptionData();
		subscriptionData.SubscriptionType = SubscriptionType.None;
		return subscriptionData;
	}

	private TimeSpan GetTimeLeft()
	{
		TimeSpan timeSpan = DateTime.UtcNow - SubscriptionActivateTime;
		return SubscriptionDuration - timeSpan;
	}

	public bool GetHasTimeLeft()
	{
		return GetTimeLeft().TotalSeconds > 0.0;
	}
}
