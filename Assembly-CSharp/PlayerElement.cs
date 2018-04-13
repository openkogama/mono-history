using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElement : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text rank;

	[SerializeField]
	private PlayerElementState state;

	[SerializeField]
	private Text score;

	[SerializeField]
	private List<Image> backgrounds;

	public string PlayerName => playerName.text;

	public void Initialize(MVPlayer player, GameStatCounterType typeToDisplay, int scoreValue)
	{
		if (player == MVGameControllerBase.Game.LocalPlayer)
		{
			for (int i = 0; i < backgrounds.Count; i++)
			{
				backgrounds[i].color = Styles.GetColor(ColorStyle.LocalPlayerBackground);
			}
		}
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted)
		{
			playerName.color = Styles.GetColor(ColorStyle.FriendGreen);
			for (int j = 0; j < backgrounds.Count; j++)
			{
				backgrounds[j].color = Styles.GetColor(ColorStyle.FriendListBackground);
			}
		}
		playerName.text = player.Username;
		if (typeToDisplay == GameStatCounterType.None || typeToDisplay == GameStatCounterType.Flag)
		{
			score.gameObject.SetActive(value: false);
		}
		else
		{
			score.gameObject.SetActive(value: true);
			score.text = WinningConditionControl.MakeIntoScoreText(scoreValue, typeToDisplay);
		}
		state.Initialize(player, friendByProfileID);
	}

	public void UpdateScoreIndex()
	{
		rank.text = (transform.GetSiblingIndex() + 1).ToString();
	}
}
