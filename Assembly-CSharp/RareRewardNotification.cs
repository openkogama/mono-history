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
		RewardRarity rewardRarity = (RewardRarity)(byte)data[(byte)11];
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)9]];
		string text = TypeToText((RewardType)(int)data[(byte)5]);
		int num = (int)data[(byte)4];
		this.text.text = mVPlayer.Username + " won " + num + text + " from spins!";
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
			RewardType.GoldReward => " gold", 
			RewardType.TestReward => " test", 
			RewardType.XPReward => " xp", 
			_ => "ERROR", 
		};
	}
}
