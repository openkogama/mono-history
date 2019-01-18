using MV.WorldObject.Subscription.SubscriptionRules;

namespace MV.WorldObject.Subscription.SubscriptionTypes;

public class SubscriptionMember2 : SubscriptionBase
{
	public SubscriptionMember2()
	{
		benefits.Add(SubscriptionBenefit.XPBoost, new XpBooster(50, 10, 2, 400));
		benefits.Add(SubscriptionBenefit.FreeBuildingGameObjects, new SubscriptionRuleBoolean(enabled: true));
	}
}
