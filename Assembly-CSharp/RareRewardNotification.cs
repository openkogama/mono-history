using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class RareRewardNotification : Notification
{
	[SerializeField]
	private GameObject EpicPanel;

	[SerializeField]
	private GameObject LegendaryPanel;

	[SerializeField]
	private Text text;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		RewardRarity rewardRarity = (RewardRarity)data[(byte)11];
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[(int)data[(byte)9]];
		string arg = TypeToText((RewardType)data[(byte)5]);
		int num = (int)data[(byte)4];
		text.text = string.Format(TM._("{0} won {1}{2} from spins!"), mVPlayer.Username, num, arg);
		switch (rewardRarity)
		{
		case RewardRarity.Epic:
			EpicPanel.gameObject.SetActive(value: true);
			LegendaryPanel.gameObject.SetActive(value: false);
			break;
		case RewardRarity.Legendary:
			LegendaryPanel.gameObject.SetActive(value: true);
			EpicPanel.gameObject.SetActive(value: false);
			break;
		}
	}

	private string TypeToText(RewardType type)
	{
		return type switch
		{
			RewardType.GoldReward => TM._(" gold"), 
			RewardType.TestReward => " test", 
			RewardType.XPReward => TM._(" xp"), 
			_ => TM._("ERROR"), 
		};
	}
}
