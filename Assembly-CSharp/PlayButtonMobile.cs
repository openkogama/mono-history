using System;
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
	protected bool shouldPop;

	[SerializeField]
	protected Button button;

	public Action OnPlayButtonPressed;

	public void Play()
	{
		if (timedPlayReward != null && timedPlayReward.IsClaimable)
		{
			timedPlayReward.ClaimReward();
		}
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		if (!flag && !flag2)
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

	public virtual void OnConfirmPlay()
	{
		if (OnPlayButtonPressed != null)
		{
			OnPlayButtonPressed();
		}
	}

	private void Update()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		if (flag)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.enabled)
			{
				countdownFill.enabled = true;
			}
			return;
		}
		if (flag2)
		{
			float num = MVGameControllerBase.LocalPlayer.RespawnTime - Time.time;
			float fillAmount = num / MVGameControllerBase.LocalPlayer.RespawnDuration;
			countdownFill.fillAmount = fillAmount;
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
			OnCountdownEnd();
			button.interactable = true;
		}
	}

	protected virtual void StartPlaying()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
		}
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}

	protected virtual void OnCountdownEnd()
	{
		StartPlaying();
	}
}
