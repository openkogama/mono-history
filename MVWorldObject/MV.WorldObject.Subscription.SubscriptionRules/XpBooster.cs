namespace MV.WorldObject.Subscription.SubscriptionRules;

public class XpBooster : SubscriptionRule
{
	public readonly int baseBoost;

	public readonly int firstOtherMemberBoost;

	public readonly int otherMembersBoost;

	public readonly int maxMemberBoost;

	public XpBooster(int baseBoost, int firstOtherMemberBoost, int otherMembersBoost, int maxMemberBoost)
	{
		this.baseBoost = baseBoost;
		this.firstOtherMemberBoost = firstOtherMemberBoost;
		this.otherMembersBoost = otherMembersBoost;
		this.maxMemberBoost = maxMemberBoost;
	}

	public int GetBaseBoostedXP(int xp)
	{
		float num = (float)xp * ((float)baseBoost / 100f);
		return (int)((float)xp + num);
	}

	public int GetBoostedXp(int xp, int membersCount)
	{
		float num = (float)xp * ((float)baseBoost / 100f);
		float num2 = 0f;
		if (membersCount > 0)
		{
			num2 += (float)xp * ((float)firstOtherMemberBoost / 100f);
		}
		if (membersCount > 1)
		{
			num2 += (float)xp * ((float)otherMembersBoost / 100f) * (float)(membersCount - 1);
		}
		if (num2 > (float)xp * ((float)maxMemberBoost / 100f))
		{
			num2 = (float)xp * ((float)maxMemberBoost / 100f);
		}
		return (int)((float)xp + num + num2);
	}

	public int GetTotalXPBoost(int membersCount)
	{
		float num = (float)baseBoost / 100f;
		float num2 = 0f;
		if (membersCount > 0)
		{
			num2 += (float)firstOtherMemberBoost / 100f;
		}
		if (membersCount > 1)
		{
			num2 += (float)otherMembersBoost / 100f * (float)(membersCount - 1);
		}
		if (num2 > (float)maxMemberBoost / 100f)
		{
			num2 = (float)maxMemberBoost / 100f;
		}
		num += num2;
		num *= 100f;
		return (int)num;
	}
}
