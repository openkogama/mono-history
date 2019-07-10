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
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		if (flag)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.gameObject.activeSelf)
			{
				countdownFill.gameObject.SetActive(value: true);
			}
		}
		else if (flag2)
		{
			float num = MVGameControllerBase.LocalPlayer.RespawnTime - Time.time;
			float fillAmount = num / MVGameControllerBase.LocalPlayer.RespawnDuration;
			countdownFill.fillAmount = fillAmount;
			if (!countdownFill.gameObject.activeSelf)
			{
				countdownFill.gameObject.SetActive(value: true);
			}
		}
		else if (countdownFill.gameObject.activeSelf)
		{
			countdownFill.gameObject.SetActive(value: false);
		}
		if (!flag && !flag2)
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
