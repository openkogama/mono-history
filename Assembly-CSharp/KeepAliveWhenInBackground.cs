using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class KeepAliveWhenInBackground : MonoBehaviour
{
	private static readonly TimeSpan SendOutGoingCommandsTimeSpan = new TimeSpan(0, 0, 0, 10);

	private static DateTime pauseTime;

	private bool destroyed;

	private bool applicationPause;

	private SessionLocatorPing sessionLocatorPing;

	private PhotonPeer peer;

	public void Initialize(SessionLocatorPing sessionLocatorPing, PhotonPeer peer)
	{
		this.sessionLocatorPing = sessionLocatorPing;
		this.peer = peer;
	}

	private void OnApplicationPause(bool applicationPause)
	{
		Debug.LogWarning("applicationPause " + applicationPause);
		this.applicationPause = applicationPause;
		if (applicationPause)
		{
			pauseTime = DateTime.Now;
			peer.LimitOfUnreliableCommands = 1;
			SupportClass.CallInBackground(PauseUpdate);
		}
		else
		{
			peer.LimitOfUnreliableCommands = 0;
		}
	}

	private bool PauseUpdate()
	{
		while (peer.SendAcksOnly())
		{
			Debug.LogWarning("Sending acks");
		}
		if (DateTime.Now - pauseTime < SendOutGoingCommandsTimeSpan)
		{
			while (peer.SendOutgoingCommands())
			{
				Debug.LogWarning("SendOutgoingCommands");
			}
		}
		AsyncWWWManager.Update();
		AwayMonitor.Update();
		sessionLocatorPing.UpdateControllerUpdate();
		return applicationPause && !destroyed;
	}

	private void OnDestroy()
	{
		destroyed = true;
	}
}
