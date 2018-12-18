using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButtonBase : MonoBehaviour
{
	[SerializeField]
	private Image countdownFill;

	[SerializeField]
	protected bool shouldPop;

	protected void UpdateButton()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		if (flag)
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
		if (!flag)
		{
			OnCountDownEnd();
		}
	}

	protected void StartPlaying()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}

	protected virtual void OnCountDownEnd()
	{
	}
}
