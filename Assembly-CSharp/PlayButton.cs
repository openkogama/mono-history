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

	private bool isMouseOver;

	public void OnPointerUp(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			Play();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (isMouseOver && eventData.button == PointerEventData.InputButton.Left)
		{
			Play();
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
		HandleRoundEnded();
		if (timedPlayReward != null && timedPlayReward.IsClaimable)
		{
			timedPlayReward.ClaimReward();
		}
		if (!HandleRoundEnded())
		{
			StartPlaying();
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

	private bool HandleRoundEnded()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			button.interactable = false;
			MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
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
		if (!MVGameControllerDesktop.LockCursorManager.CursorLock)
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
