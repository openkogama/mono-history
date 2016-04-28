using MV.Common;

public interface IActorRewardClient
{
	RewardType RewardType { get; }

	RewardRarity RewardRarity { get; }
}
