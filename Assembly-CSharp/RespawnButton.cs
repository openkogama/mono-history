using UnityEngine;

public class RespawnButton : MonoBehaviour
{
	public void Respawn()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.KillSelf();
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
	}
}
