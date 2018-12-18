using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelRewardNotification : Notification
{
	[SerializeField]
	private Text goldAmount;

	[SerializeField]
	private Text levelText;

	protected override NotificationLifetime Lifetime => NotificationLifetime.SuperHigh;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		levelText.text = string.Format(TM._("Level {0} Unlocks"), MVGameControllerBase.Game.LevelRewardsManager.NextReward.Key);
		goldAmount.text = MVGameControllerBase.Game.LevelRewardsManager.NextReward.Value + "!";
	}
}
