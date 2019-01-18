using System.Collections.Generic;

namespace MV.WorldObject.Subscription;

public abstract class SubscriptionBase
{
	protected Dictionary<SubscriptionBenefit, SubscriptionRule> benefits = new Dictionary<SubscriptionBenefit, SubscriptionRule>();

	public bool HasBenefit(SubscriptionBenefit subscriptionBenefit)
	{
		return benefits.ContainsKey(subscriptionBenefit);
	}

	public bool TryGetRule<T>(SubscriptionBenefit subscriptionBenefit, out T rule) where T : SubscriptionRule
	{
		if (benefits.TryGetValue(subscriptionBenefit, out var value))
		{
			rule = (T)value;
			return true;
		}
		rule = null;
		return false;
	}
}
