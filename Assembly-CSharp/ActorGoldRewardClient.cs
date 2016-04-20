using MV.Common;

public class ActorGoldRewardClient : IActorRewardClient
{
	public int goldAmount;

	public RewardType RewardType { get; set; }

	public RewardRarity RewardRarity { get; set; }

	public override string ToString()
	{
		return $"RewardType {RewardType}. RewardRarity {RewardRarity}. goldAmount {goldAmount}.";
	}
}
