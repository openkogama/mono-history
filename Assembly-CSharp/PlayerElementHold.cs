using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElementHold : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private PlayerElementStateHold state;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Color friendNameColor;

	[SerializeField]
	private Color localPlayerNameColor;

	public void Initialize(MVPlayer player)
	{
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted)
		{
			playerName.color = friendNameColor;
		}
		if (MVGameControllerBase.Game.LocalPlayer.ActorNr == player.ActorNr)
		{
			playerName.color = localPlayerNameColor;
		}
		playerName.text = player.Username;
		score.text = player.GetGameStat(GameStatCounterType.Kill).ToString();
		state.Initialize(player, friendByProfileID);
	}
}
