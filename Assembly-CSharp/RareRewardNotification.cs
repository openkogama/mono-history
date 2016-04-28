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

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		RewardRarity rewardRarity = (RewardRarity)(byte)data[NotificationDataType.Amount];
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)9]];
		text.text = string.Concat(mVPlayer.Username, " won an ", rewardRarity, " reward using spins!");
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
}
