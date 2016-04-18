using System;
using System.Collections.Generic;
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
			AsyncWWWManager.WWWRequest(new GetRequest(MVGameControllerBase.GameSessionData.pingURL, WWWCallBack, WWWRequestPriority.ExecuteWhileSyncronizing));
		}
	}

	public static void LeaveSession()
	{
		Debug.Log("LeaveSession");
		AsyncWWWManager.WWWRequest(new GetRequest(MVGameControllerBase.GameSessionData.disconnectURL, null, WWWRequestPriority.ExecuteWhileSyncronizing));
	}

	private void WWWCallBack(WWW result)
	{
		if (!string.IsNullOrEmpty(result.error))
		{
			ErrorCallback(result);
			return;
		}
		waitForTicks = new WaitForTicks(pingIntervalInMilliSeconds);
		pingSend = false;
	}

	private void ErrorCallback(WWW result)
	{
		try
		{
			Debug.Log("Ping failed url: " + result.url);
			Debug.Log("Response headers");
			foreach (KeyValuePair<string, string> responseHeader in result.responseHeaders)
			{
				Debug.LogFormat("{0} {1}", responseHeader.Key, responseHeader.Value);
			}
			Debug.LogError("Ping failed");
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in session locator error callback " + ex.Message);
		}
		Debug.Log("Quiting from session locator error callback");
		Coroutines.StartCoroutine(WaitForFrames.Frames(5, DoApplicationQuit));
	}

	private void DoApplicationQuit()
	{
		MVGameControllerBase.ApplicationQuit(new QuitConnectionError());
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
