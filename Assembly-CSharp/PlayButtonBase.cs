using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonBase : MonoBehaviour
{
	[SerializeField]
	private Text countdownTest;

	[SerializeField]
	private Text playText;

	[SerializeField]
	private Text pleaseWaitText;

	protected void UpdateButton()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = MVGameControllerDesktop.LockCursorManager.LockCursor && flag;
		if (pleaseWaitText.gameObject.activeSelf != flag2)
		{
			pleaseWaitText.gameObject.SetActive(flag2);
		}
		if (playText.gameObject.activeSelf == flag2)
		{
			playText.gameObject.SetActive(!flag2);
		}
		if (flag)
		{
			countdownTest.text = TM._("New round starts in: ") + MVGameControllerBase.Game.NetworkGameStateListener.CountdownInSeconds;
			if (!countdownTest.gameObject.activeSelf)
			{
				countdownTest.gameObject.SetActive(value: true);
			}
		}
		else if (countdownTest.gameObject.activeSelf)
		{
			countdownTest.gameObject.SetActive(value: false);
		}
	}
}
