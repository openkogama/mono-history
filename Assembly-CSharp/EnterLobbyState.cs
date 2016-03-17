using UnityEngine;

public class EnterLobbyState : MonoBehaviour
{
	public void LobbyState()
	{
		MVGameControllerBase.IPlayModeUI.InLobbyState = true;
	}
}
