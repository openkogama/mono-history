using System.Collections.Generic;
using MV.Common;
using UnityEngine.Events;

public class TierUnlockedPopupContentAccess : TierUnlockedPopupContentBase
{
	private GamePassTier unlockedGamePassTier;

	public override void Initialize(GamePassTier unlockedGamePassTier, UnityAction onDisplayDoneCallback)
	{
		this.unlockedGamePassTier = unlockedGamePassTier;
		base.Initialize(unlockedGamePassTier, onDisplayDoneCallback);
		UpdateItemAmountText();
	}

	private void UpdateItemAmountText()
	{
		string format = TM._("x{0} IN-GAME ACCESS UNLOCKED!");
		titleText.text = string.Format(format, GetItemAmount());
	}

	private int GetItemAmount()
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierItemData = MVGameControllerBase.Game.GameTierShopRepository.GetTierItemData(unlockedGamePassTier);
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> item in tierItemData)
		{
			if (item.Value.Count > 0 && IsTierItemAnAccessItem(item.Key, item.Value[0]))
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		int num = 0;
		foreach (List<MVWorldObjectClient> value in dictionary.Values)
		{
			num += value.Count;
		}
		return num;
	}

	private bool IsTierItemAnAccessItem(MVWorldObjectDocumentationType worldObjectType, MVWorldObjectClient item)
	{
		return worldObjectType == MVWorldObjectDocumentationType.Lever || worldObjectType == MVWorldObjectDocumentationType.PressurePlate || item is MVTeleporter;
	}
}
