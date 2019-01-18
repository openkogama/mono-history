using System;
using MV.WorldObject.Subscription.SubscriptionTypes;
using MV.WorldObject.Subscription.SubscriptionTypes.BuiltInTypes;

namespace MV.WorldObject.Subscription;

public class SubscriptionRulesWrapper
{
	private readonly SubscriptionBase subscriptionBase;

	private readonly SubscriptionDefault subscriptionDefault;

	public readonly SubscriptionType SubscriptionType;

	public SubscriptionRulesWrapper(SubscriptionType subscriptionType)
	{
		SubscriptionType = subscriptionType;
		subscriptionDefault = new SubscriptionDefault();
		subscriptionBase = GetSubscription(subscriptionType);
	}

	public bool HasBenefit(SubscriptionBenefit subscriptionBenefit)
	{
		return subscriptionBase.HasBenefit(subscriptionBenefit);
	}

	public T GetRule<T>(SubscriptionBenefit subscriptionBenefit) where T : SubscriptionRule
	{
		if (subscriptionBase.TryGetRule<T>(subscriptionBenefit, out var rule))
		{
			return rule;
		}
		if (subscriptionDefault.TryGetRule<T>(subscriptionBenefit, out rule))
		{
			return rule;
		}
		throw new Exception("SubscriptionDefault must implement all rules. Does not implement: " + subscriptionBenefit);
	}

	private static SubscriptionBase GetSubscription(SubscriptionType subscriptionType)
	{
		return subscriptionType switch
		{
			SubscriptionType.None => (SubscriptionBase)new SubscriptionNone(), 
			SubscriptionType.Membership1 => new SubscriptionMember1(), 
			SubscriptionType.Membership2 => new SubscriptionMember2(), 
			_ => throw new Exception("Unknown subscription type: " + subscriptionType), 
		};
	}

	public override string ToString()
	{
		return "SubscriptionType: " + SubscriptionType;
	}
}
