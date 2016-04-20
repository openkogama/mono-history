using System.Collections.Generic;
using RewardGeneration;
using UnityEngine;

public class RewardAllPrizes : MonoBehaviour
{
	[SerializeField]
	private RectTransform content;

	public void Initialize(List<RewardRarityGroupDef> sortedRarityGroupList)
	{
		for (int num = sortedRarityGroupList.Count - 1; num >= 0; num--)
		{
			RewardRarityGroupDef rewardRarity = sortedRarityGroupList[num];
			RewardObject rewardObject = Object.Instantiate(rewardRarity.rewardDef.rewardPrefabType);
			rewardObject.transform.SetParent(content.transform, worldPositionStays: false);
			rewardObject.Initialize(rewardRarity);
		}
	}
}
