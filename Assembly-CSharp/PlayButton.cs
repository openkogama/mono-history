using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButton : PlayButtonBase, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private TimedPlayReward timedPlayReward;

	[SerializeField]
	private Button button;

	[SerializeField]
	private bool shouldConfirmPlay;

	private bool isMouseOver;

	public Action OnPlayButtonPressed;

	public void OnPointerUp(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			if (shouldConfirmPlay)
			{
				ConfirmPlay();
			}
			else
			{
				Play();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			if (shouldConfirmPlay)
			{
				ConfirmPlay();
			}
			else
			{
				Play();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}

	public void Play()
	{
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		HandlePlayAvailable();
		if (timedPlayReward != null && timedPlayReward.IsClaimable)
		{
			timedPlayReward.ClaimReward();
		}
		if (!HandlePlayAvailable())
		{
			StartPlaying();
		}
	}

	private void ConfirmPlay()
	{
		if (OnPlayButtonPressed != null && !HandlePlayAvailable())
		{
			OnPlayButtonPressed();
		}
	}

	private void Update()
	{
		UpdateButton();
	}

	private void OnEnable()
	{
		button.interactable = true;
	}

	private bool HandlePlayAvailable()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		if (flag || flag2)
		{
			button.interactable = false;
			if (!shouldConfirmPlay)
			{
				MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
			}
			return true;
		}
		return false;
	}

	protected override void OnCountDownEnd()
	{
		if (button.interactable)
		{
			return;
		}
		if (shouldConfirmPlay)
		{
			ConfirmPlay();
		}
		else if (!MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			StartPlaying();
		}
		else
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
			if (shouldPop)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
				{
					handler.Pop();
				});
			}
		}
		button.interactable = true;
	}
}
