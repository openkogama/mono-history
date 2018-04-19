using System.Collections.Generic;
using MV.Common;
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
	private List<Image> backgrounds;

	public void Initialize(MVPlayer player, GameStatCounterType typeToDisplay, int scoreValue)
	{
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted)
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
		playerName.text = player.Username;
		if (typeToDisplay == GameStatCounterType.None || typeToDisplay == GameStatCounterType.Flag)
		{
			score.gameObject.SetActive(value: false);
			return;
		}
		score.gameObject.SetActive(value: true);
		score.text = WinningConditionControl.MakeIntoScoreText(scoreValue, typeToDisplay);
	}

	public void UpdateScoreIndex()
	{
		rank.text = (transform.GetSiblingIndex() + 1).ToString();
	}
}
