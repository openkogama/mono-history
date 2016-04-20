using RewardGeneration;

public class RewardObjectGold : RewardObject
{
	public override void Initialize(RewardRarityGroupDef rewardRarity)
	{
		base.Initialize(rewardRarity);
	}

	protected override string FormatRewardText(RewardRarityGroupDef rewardRarity)
	{
		ActorGoldRewardClient actorGoldRewardClient = rewardRarity.actorReward as ActorGoldRewardClient;
		return string.Format(amountText.text, actorGoldRewardClient.goldAmount.ToString("n0"));
	}
}
