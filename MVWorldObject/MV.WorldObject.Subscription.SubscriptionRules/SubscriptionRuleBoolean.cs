namespace MV.WorldObject.Subscription.SubscriptionRules;

public class SubscriptionRuleBoolean : SubscriptionRule
{
	public bool Enabled { get; private set; }

	public SubscriptionRuleBoolean(bool enabled)
	{
		Enabled = enabled;
	}
}
