using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonMobile : MonoBehaviour
{
	[SerializeField]
	private Image countdownFill;

	public void Play()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVGameControllerBase.IPlayModeUI.InLobbyState = false;
			if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			}
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.enabled)
			{
				countdownFill.enabled = true;
			}
		}
		else if (countdownFill.enabled)
		{
			countdownFill.enabled = false;
		}
	}
}
