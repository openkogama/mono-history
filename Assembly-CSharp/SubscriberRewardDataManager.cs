using MV.WorldObject.GamePassSystem.GamePassEarnings;

public static class SubscriberRewardDataManager
{
	private struct SubscriberRewardData
	{
		public int baseXPBonus;
	}

	private static SubscriberRewardData subscriberRewardData = default;

	public static KogamaVatValues VatValues { get; set; }

	public static void SetBaseXPBonus(int baseXpBonus)
	{
		subscriberRewardData.baseXPBonus = baseXpBonus;
	}

	public static int GetBaseXpAmount()
	{
		return subscriberRewardData.baseXPBonus;
	}
}
