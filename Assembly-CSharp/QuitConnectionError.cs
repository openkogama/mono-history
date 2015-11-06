using UnityEngine;

public class QuitConnectionError : QuitBaseCallback
{
	public void OnQuit()
	{
		if (MVGameControllerBase.Game == null)
		{
			Debug.LogWarning("Game is null");
		}
		else if (MVGameControllerBase.Game.ConnState == MVConnState.DisconnectedByUser)
		{
			Debug.LogWarning("Not going to disconnect page as disconnect was by user");
			return;
		}
		BrowserComm.ToJavaScript.ExternalCall("gotoDisconnectedPage");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.disconnectedURL);
	}
}
