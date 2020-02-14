using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnterPlaySessionRoundCountDown : MonoBehaviour
{
	[SerializeField]
	private Text text;

	private void Update()
	{
		text.text = TM._("Round starts in: ") + MVGameControllerBase.Game.NetworkGameStateListener.CountdownInSeconds;
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
			if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
			}
		}
	}
}
