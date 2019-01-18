using MV.WorldObject.Subscription.SubscriptionRules;

namespace MV.WorldObject.Subscription.SubscriptionTypes;

public class SubscriptionMember1 : SubscriptionBase
{
	public SubscriptionMember1()
	{
		benefits.Add(SubscriptionBenefit.XPBoost, new XpBooster(50, 10, 10, 400));
		benefits.Add(SubscriptionBenefit.FreeBuildingGameObjects, new SubscriptionRuleBoolean(enabled: true));
		benefits.Add(SubscriptionBenefit.FreeBuildingMaterials, new SubscriptionRuleBoolean(enabled: true));
		benefits.Add(SubscriptionBenefit.FreeBuildingThemes, new SubscriptionRuleBoolean(enabled: true));
	}
}
