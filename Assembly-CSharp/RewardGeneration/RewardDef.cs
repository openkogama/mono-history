using System;
using MV.Common;

namespace RewardGeneration;

[Serializable]
public struct RewardDef
{
	public RewardType rewardType;

	public string rewardText;

	public RewardObject rewardPrefabType;
}
