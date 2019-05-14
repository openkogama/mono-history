using UnityEngine;

public class QuitConnectionError : QuitBaseCallback
{
	public readonly bool gotoDisconnectPage;

	public QuitConnectionError()
	{
		if (MVGameControllerBase.Game == null)
		{
			Debug.LogWarning("Game is null");
		}
		else if (MVGameControllerBase.Game.ConnState == MVConnState.DisconnectedByUser)
		{
			Debug.LogWarning("Not going to disconnect page as disconnect was by user");
		}
		else
		{
			gotoDisconnectPage = true;
		}
	}

	public void OnQuit()
	{
		if (gotoDisconnectPage)
		{
			BrowserCommGotoRequests.GotoDisconnected();
		}
	}
}
