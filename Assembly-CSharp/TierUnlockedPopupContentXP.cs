using MV.Common;
using UnityEngine.Events;

public class TierUnlockedPopupContentXP : TierUnlockedPopupContentBase
{
	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		int xPReward = GamePassProgressionController.GetXPReward(unlockedGamePassTier);
		titleText.text = xPReward + " XP!";
	}
}
