using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierTempUnlockedInformationPopupContent : TierUnlockedPopupContentBase
{
	[SerializeField]
	private Text tempUnlockInformationText;

	private string tempUnlockInformation = "Game Tier {0} is now available until the next time you die. Enjoy!";

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		string format = TM._(tempUnlockInformation);
		tempUnlockInformationText.text = string.Format(format, (int)unlockedGamePassTier);
	}
}
