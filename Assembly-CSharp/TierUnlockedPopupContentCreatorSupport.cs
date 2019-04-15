using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierUnlockedPopupContentCreatorSupport : TierUnlockedPopupContentBase
{
	[SerializeField]
	private Text informationText;

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		informationText.text = (1f - SubscriberRewardDataManager.VatValues.regularUserVat) * 100f + "% of what you spend goes straight to the creator(s) of this game.";
	}
}
