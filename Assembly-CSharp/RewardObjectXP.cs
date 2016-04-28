using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using RewardGeneration;
using UnityEngine;
using UnityEngine.UI;

public class RewardObjectXP : RewardObject
{
	[Serializable]
	private class XPRewardStyleDef
	{
		[SerializeField]
		public RewardRarity Rarity;

		[SerializeField]
		public Color BackgroundColor;

		[SerializeField]
		public Color TextColor;
	}

	[SerializeField]
	private Image XPBackground;

	[SerializeField]
	private Image XPText;

	[SerializeField]
	private RectTransform xpReward;

	[SerializeField]
	private List<XPRewardStyleDef> XPStyles = new List<XPRewardStyleDef>();

	public override void Initialize(RewardRarityGroupDef rewardRarity)
	{
		base.Initialize(rewardRarity);
		amountText.text = FormatRewardText(rewardRarity);
		PerformXPAction(rewardRarity);
	}

	private void PerformXPAction(RewardRarityGroupDef rewardRarity)
	{
		XPRewardStyleDef xPRewardStyleDef = XPStyles.FirstOrDefault((XPRewardStyleDef x) => x.Rarity == rewardRarity.rarity);
		XPText.color = xPRewardStyleDef.TextColor;
		XPBackground.color = xPRewardStyleDef.BackgroundColor;
	}

	protected override string FormatRewardText(RewardRarityGroupDef rewardRarity)
	{
		ActorXPRewardClient actorXPRewardClient = rewardRarity.actorReward as ActorXPRewardClient;
		return string.Format(amountText.text, actorXPRewardClient.xpAmount.ToString("n0"));
	}
}
