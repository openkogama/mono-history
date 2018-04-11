using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButtonMobile : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private EnterPlaySessionRoundCountDown enterPlaySessionRoundCountDownPrefab;

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
		else
		{
			EnterPlaySessionRoundCountDown enterPlaySessionRoundCountDown = Object.Instantiate(enterPlaySessionRoundCountDownPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(enterPlaySessionRoundCountDown.gameObject, UIPushOption.Blocking);
			});
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			text.text = TM._("New round starts in: ") + MVGameControllerBase.Game.NetworkGameStateListener.CountdownInSeconds;
			if (!text.enabled)
			{
				text.enabled = true;
			}
		}
		else if (text.enabled)
		{
			text.enabled = false;
		}
	}
}
