using MV.Common;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
	public void Play()
	{
		MVGameControllerBase.IPlayModeUI.InLobbyState = false;
		if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState == AvatarRuntimeState.Hidden)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		}
	}
}
