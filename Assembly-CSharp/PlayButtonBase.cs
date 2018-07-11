using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonBase : MonoBehaviour
{
	[SerializeField]
	private Image countdownFill;

	protected void UpdateButton()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.gameObject.activeSelf)
			{
				countdownFill.gameObject.SetActive(value: true);
			}
		}
		else if (countdownFill.gameObject.activeSelf)
		{
			countdownFill.gameObject.SetActive(value: false);
		}
	}
}
