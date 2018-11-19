using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : PlayButtonBase
{
	[SerializeField]
	private Button button;

	public void Play()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		HandleRoundEnded();
	}

	private void Update()
	{
		UpdateButton();
	}

	private void OnEnable()
	{
		button.interactable = true;
	}

	private void HandleRoundEnded()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			button.interactable = false;
		}
	}
}
