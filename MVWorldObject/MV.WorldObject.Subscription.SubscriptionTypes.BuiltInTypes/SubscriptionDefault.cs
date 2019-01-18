using MV.WorldObject.Subscription.SubscriptionRules;

namespace MV.WorldObject.Subscription.SubscriptionTypes.BuiltInTypes;

public class SubscriptionDefault : SubscriptionBase
{
	public SubscriptionDefault()
	{
		benefits.Add(SubscriptionBenefit.XPBoost, new XpBooster(0, 10, 10, 400));
		benefits.Add(SubscriptionBenefit.GameCoinBoost, new GameCoinBooster(0));
		benefits.Add(SubscriptionBenefit.FreeBuildingGameObjects, new SubscriptionRuleBoolean(enabled: false));
	}
}
