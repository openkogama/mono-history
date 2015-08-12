using UnityEngine;

public class QuitConnectionError : QuitBaseCallback
{
	public void OnQuit()
	{
		if (MVGameController.Game == null)
		{
			Debug.LogWarning("Game is null");
		}
		else if (MVGameController.Game.ConnState == MVConnState.DisconnectedByUser)
		{
			Debug.LogWarning("Not going to disconnect page as disconnect was by user");
			return;
		}
		BrowserComm.ToJavaScript.ExternalCall("gotoDisconnectedPage");
		BrowserComm.ExecuteBrowserRequest(MVGameController.GameSessionData.disconnectedURL);
	}
}
