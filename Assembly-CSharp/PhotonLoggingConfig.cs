using System;
using ExitGames.Client.Photon;
using UnityEngine;

[Serializable]
public struct PhotonLoggingConfig
{
	[SerializeField]
	public DebugLevel defaultDebugLevel;

	[SerializeField]
	public DebugLevel untilConnectedDebugLevel;
}
