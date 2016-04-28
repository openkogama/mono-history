using System;
using MV.Common;
using UnityEngine;

namespace RewardGeneration;

[Serializable]
public struct RewardRarityGroupDef
{
	public Color rarityColor;

	public RewardRarity rarity;

	public float showProbability;

	[NonSerialized]
	public IActorRewardClient actorReward;

	[NonSerialized]
	public float normalizedProbability;

	[NonSerialized]
	public RewardDef rewardDef;
}
