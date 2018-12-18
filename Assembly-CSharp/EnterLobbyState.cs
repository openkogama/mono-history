using UnityEngine;

public class EnterLobbyState : MonoBehaviour
{
	public void LobbyState()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}
}
