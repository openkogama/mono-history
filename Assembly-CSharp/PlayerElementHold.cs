using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElementHold : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text rank;

	[SerializeField]
	private Text score;

	[SerializeField]
	private GameObject memberUI;

	[SerializeField]
	private Text memberRank;

	[SerializeField]
	private Image nonMemberUI;

	[SerializeField]
	private List<Image> backgrounds;

	[SerializeField]
	private Image redDot;

	public void Initialize(MVPlayer player, GameStatCounterType typeToDisplay, int scoreValue)
	{
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		bool flag = friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted;
		redDot.gameObject.SetActive(value: false);
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Pending)
		{
			bool flag2 = MVGameControllerBase.Game.Friends.Friends.ContainsValue(friendByProfileID);
			redDot.gameObject.SetActive(!flag2);
		}
		if (flag)
		{
			playerName.color = Styles.GetColor(ColorStyle.FriendGreen);
			for (int i = 0; i < backgrounds.Count; i++)
			{
				backgrounds[i].color = Styles.GetColor(ColorStyle.FriendListBackground);
			}
		}
		if (player == MVGameControllerBase.Game.LocalPlayer)
		{
			for (int j = 0; j < backgrounds.Count; j++)
			{
				backgrounds[j].color = Styles.GetColor(ColorStyle.LocalPlayerBackground);
			}
		}
		if (player.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost))
		{
			ActivateSubscriberUI(flag);
		}
		else
		{
			memberUI.SetActive(value: false);
		}
		playerName.text = player.UserProfileData.UserName;
		if (typeToDisplay == GameStatCounterType.None)
		{
			score.gameObject.SetActive(value: false);
			return;
		}
		score.gameObject.SetActive(value: true);
		score.text = WinningConditionControl.MakeIntoScoreText(scoreValue, typeToDisplay);
	}

	public void UpdateScoreIndex()
	{
		string text = (transform.GetSiblingIndex() + 1).ToString();
		if (!memberUI.activeSelf)
		{
			rank.text = text;
		}
		memberRank.text = text;
	}

	private void ActivateSubscriberUI(bool isFriend)
	{
		memberUI.SetActive(value: true);
		nonMemberUI.enabled = false;
		rank.text = string.Empty;
		if (!isFriend)
		{
			playerName.color = Styles.GetColor(ColorStyle.OffWhite);
		}
		score.color = Styles.GetColor(ColorStyle.OffWhite);
		for (int i = 0; i < backgrounds.Count; i++)
		{
			backgrounds[i].color = Styles.GetColor(ColorStyle.Gray);
		}
	}
}
