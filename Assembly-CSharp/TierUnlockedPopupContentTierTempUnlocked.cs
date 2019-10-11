using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierUnlockedPopupContentTierTempUnlocked : TierUnlockedPopupContentBase
{
	[SerializeField]
	private Text tierRankText;

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		int num = (int)unlockedGamePassTier;
		tierRankText.text = num.ToString();
		titleText.text = "FREE TRY!";
	}
}
