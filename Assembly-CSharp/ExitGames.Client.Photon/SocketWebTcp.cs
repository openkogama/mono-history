using System;
using System.Collections;
using UnityEngine;

namespace ExitGames.Client.Photon;

internal class SocketWebTcp : IPhotonSocket
{
	private class MonoBehavior_ : MonoBehaviour
	{
	}

	private WebSocket sock;

	private readonly object syncer = new object();

	private GameObject websocketConnectionObject;

	internal const int ALL_HEADER_BYTES = 9;

	internal const int TCP_HEADER_BYTES = 7;

	internal const int MSG_HEADER_BYTES = 2;

	public SocketWebTcp(PeerBase npeer)
		: base(npeer)
	{
		ServerAddress = npeer.ServerAddress;
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			Listener.DebugReturn(DebugLevel.INFO, "new SocketWebTcp() " + ServerAddress);
		}
		Protocol = MVGameControllerBase.GameSessionData.ConnectionProtocol;
		PollReceive = false;
	}

	public override bool Connect()
	{
		State = PhotonSocketState.Connecting;
		if (websocketConnectionObject != null)
		{
			UnityEngine.Object.Destroy(websocketConnectionObject);
		}
		websocketConnectionObject = new GameObject("websocketConnectionObject");
		MonoBehaviour monoBehaviour = websocketConnectionObject.AddComponent<MonoBehavior_>();
		UnityEngine.Object.DontDestroyOnLoad(websocketConnectionObject);
		sock = new WebSocket(new Uri(ServerAddress));
		monoBehaviour.StartCoroutine(sock.Connect());
		monoBehaviour.StartCoroutine(ReceiveLoop());
		return true;
	}

	public override bool Disconnect()
	{
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			Listener.DebugReturn(DebugLevel.INFO, "SocketTcp.Disconnect()");
		}
		State = PhotonSocketState.Disconnecting;
		lock (syncer)
		{
			if (sock != null)
			{
				try
				{
					sock.Close();
				}
				catch (Exception ex)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Exception in Disconnect(): " + ex);
				}
				sock = null;
			}
		}
		if (websocketConnectionObject != null)
		{
			UnityEngine.Object.Destroy(websocketConnectionObject);
		}
		State = PhotonSocketState.Disconnected;
		return true;
	}

	public override PhotonSocketError Send(byte[] data, int length)
	{
		if (State != PhotonSocketState.Connected)
		{
			return PhotonSocketError.Skipped;
		}
		try
		{
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				Listener.DebugReturn(DebugLevel.ALL, "Sending: " + SupportClass.ByteArrayToString(data));
			}
			sock.Send(data);
		}
		catch (Exception ex)
		{
			Listener.DebugReturn(DebugLevel.ERROR, "Cannot send. " + ex.Message);
			HandleException(StatusCode.Exception);
			return PhotonSocketError.Exception;
		}
		return PhotonSocketError.Success;
	}

	public override PhotonSocketError Receive(out byte[] data)
	{
		data = null;
		return PhotonSocketError.NoData;
	}

	public IEnumerator ReceiveLoop()
	{
		Listener.DebugReturn(DebugLevel.INFO, "ReceiveLoop()");
		while (sock == null || !sock.Connected)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (sock.Error != null)
		{
			Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread due to error: " + sock.Error);
			yield break;
		}
		if (ReportDebugOfLevel(DebugLevel.ALL))
		{
			Listener.DebugReturn(DebugLevel.ALL, "Receiving by websocket. this.State: " + State);
		}
		State = PhotonSocketState.Connected;
		while (State == PhotonSocketState.Connected)
		{
			if (sock.Error != null)
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread (inside loop) due to error: " + sock.Error);
				yield break;
			}
			byte[] inBuff = sock.Recv();
			if (inBuff == null || inBuff.Length == 0)
			{
				yield return new WaitForSeconds(0.1f);
				continue;
			}
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				Listener.DebugReturn(DebugLevel.ALL, "TCP << " + inBuff.Length + " = " + SupportClass.ByteArrayToString(inBuff));
			}
			if (inBuff[0] == 240)
			{
				Listener.DebugReturn(DebugLevel.INFO, "ReceiveLoop() - 8");
				HandleReceivedDatagram(inBuff, inBuff.Length, willBeReused: false);
				continue;
			}
			if (inBuff.Length > 0)
			{
				HandleReceivedDatagram(inBuff, inBuff.Length, willBeReused: false);
			}
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				Listener.DebugReturn(DebugLevel.ALL, "TCP < " + inBuff.Length + " = " + SupportClass.ByteArrayToString(inBuff));
			}
		}
		Disconnect();
	}
}
