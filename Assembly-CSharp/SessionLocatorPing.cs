using System;
using System.Collections.Generic;
using UnityEngine;

public class SessionLocatorPing : IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	private WaitForTicksLocal waitForTicks = new WaitForTicksLocal(0);

	private int pingIntervalInMilliSeconds = 60000;

	private bool pingInFlight;

	private bool connectionLost;

	public SessionLocatorPing()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void UpdateControllerUpdate()
	{
		InternalUpdate();
		if (connectionLost)
		{
			CloseApplication();
		}
	}

	public void BackgroundUpdate()
	{
		InternalUpdate();
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	public static void LeaveSession()
	{
		AsyncWWWManager.WWWRequest(new GetRequest(MVGameControllerBase.GameSessionData.disconnectURL, null, WWWRequestPriority.ExecuteIgnoreAllConstraints));
	}

	private void InternalUpdate()
	{
		if (waitForTicks.TimeIsUp && !pingInFlight)
		{
			pingInFlight = true;
			AsyncWWWManager.WWWRequest(new GetRequest(MVGameControllerBase.GameSessionData.pingURL, WWWCallBack, WWWRequestPriority.ExecuteIgnoreAllConstraints));
		}
	}

	private void WWWCallBack(WWW result)
	{
		if (string.IsNullOrEmpty(result.error))
		{
			waitForTicks = new WaitForTicksLocal(pingIntervalInMilliSeconds);
			pingInFlight = false;
		}
		else
		{
			OnPingError(result);
		}
	}

	private void OnPingError(WWW result)
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
			Debug.LogError("Error in session locator error callback: " + ex.Message);
		}
		connectionLost = true;
	}

	private void CloseApplication()
	{
		Debug.Log("Quiting due to connection error, attempting to ping session locator.");
		Coroutines.Start(WaitForFrames.Frames(5, () =>
		{
			MVGameControllerBase.ApplicationQuit(new QuitConnectionError());
		}));
	}
}
