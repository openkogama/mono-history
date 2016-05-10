using RewardGeneration;

public class RewardObjectXP : RewardObject
{
	public override void Initialize(RewardRarityGroupDef rewardRarity)
	{
		amountText.text = textFormat;
		base.Initialize(rewardRarity);
	}

	protected override string FormatRewardText(RewardRarityGroupDef rewardRarity)
	{
		ActorXPRewardClient actorXPRewardClient = rewardRarity.actorReward as ActorXPRewardClient;
		return string.Format(amountText.text, actorXPRewardClient.xpAmount.ToString("n0"));
	}
}
