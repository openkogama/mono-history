using UnityEngine;
using UnityEngine.UI;

public class OwnerToolController : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text text;

	public void Initialize(string playerNameString)
	{
		playerName.text = playerNameString;
		text.text = playerNameString + " " + TM._("will be kicked and unable to rejoin.");
	}

	public void OnKickClicked()
	{
		MVPlayer player = GetPlayer(playerName.text);
		if (player != null)
		{
			OwnerOps.RevokeEditRightsAndKick(this, player);
		}
		else
		{
			Debug.LogWarning("Player is not present in session.");
		}
	}

	private static MVPlayer GetPlayer(string userName)
	{
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value.UserProfileData.UserName == userName)
			{
				return value;
			}
		}
		return null;
	}
}
