using System;
using UnityEngine;

public class SessionLocatorPing : IUpdatecontrollerSubscriber
{
	private WaitForTicks waitForTicks = new WaitForTicks(0);

	private int pingIntervalInMilliSeconds = 60000;

	private bool pingSend;

	public SessionLocatorPing()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void UpdateControllerUpdate()
	{
		if (waitForTicks.TimeIsUp && !pingSend)
		{
			pingSend = true;
			AsyncWWWManager.WWWRequest(new GetRequest(MVGameController.GameSessionData.pingURL, WWWCallBack));
		}
	}

	public static void LeaveSession()
	{
		Debug.Log("LeaveSession");
		AsyncWWWManager.WWWRequest(new GetRequest(MVGameController.GameSessionData.disconnectURL, null));
	}

	private void WWWCallBack(WWW result)
	{
		if (!string.IsNullOrEmpty(result.error))
		{
			Debug.Log("Ping failed url: " + result.url);
			ErrorCallback(result.error);
		}
		else
		{
			waitForTicks = new WaitForTicks(pingIntervalInMilliSeconds);
			pingSend = false;
		}
	}

	private void ErrorCallback(string errorString)
	{
		try
		{
			Debug.LogError(errorString);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in session locator error callback " + ex.Message);
		}
		Debug.Log("Quiting from session locator error callback");
		MVGameController.ApplicationQuit(new QuitConnectionError());
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
