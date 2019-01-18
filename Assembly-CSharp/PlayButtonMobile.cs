using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButtonMobile : MonoBehaviour
{
	[SerializeField]
	private Image countdownFill;

	[SerializeField]
	private TimedPlayReward timedPlayReward;

	[SerializeField]
	private bool shouldPop;

	[SerializeField]
	private Button button;

	public void Play()
	{
		if (timedPlayReward != null && timedPlayReward.IsClaimable)
		{
			timedPlayReward.ClaimReward();
		}
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			StartPlaying();
		}
		else
		{
			button.interactable = false;
		}
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
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
			return;
		}
		if (countdownFill.enabled)
		{
			countdownFill.enabled = false;
		}
		if (!button.interactable)
		{
			StartPlaying();
			button.interactable = true;
		}
	}

	private void StartPlaying()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}
}
