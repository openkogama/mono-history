using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElementStateHold : MonoBehaviour
{
	[SerializeField]
	private Image pendingFriendship;

	[SerializeField]
	private Image localPlayerImage;

	public void Initialize(MVPlayer player, Friend friend)
	{
		if (!player.IsAnonymous && !MVGameControllerBase.Game.LocalPlayer.IsAnonymous)
		{
			if (friend != null && friend.status == FriendStatus.Pending && MVGameControllerBase.Game.Friends.Friends.ContainsValue(friend))
			{
				pendingFriendship.gameObject.SetActive(value: true);
			}
			if (player.ProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
			{
				localPlayerImage.gameObject.SetActive(value: true);
			}
		}
	}
}
