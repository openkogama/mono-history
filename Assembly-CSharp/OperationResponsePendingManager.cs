using System.Collections.Generic;
using ExitGames.Client.Photon;
using MV.Common;
using UnityEngine;

public class OperationResponsePendingManager
{
	private HashSet<MVOperationCodes> pendingOperations = new HashSet<MVOperationCodes>();

	private PhotonPeer peer;

	public OperationResponsePendingManager(PhotonPeer peer)
	{
		this.peer = peer;
	}

	public bool AddOperationCodeToPending(MVOperationCodes operationCode, Dictionary<byte, object> data)
	{
		if (IsOperationPending(operationCode))
		{
			Debug.LogWarning("Operation is already pending!");
			return false;
		}
		pendingOperations.Add(operationCode);
		peer.OpCustom((byte)operationCode, data, sendReliable: true);
		return true;
	}

	public void TryRemovePendingOperation(MVOperationCodes operationCode)
	{
		if (pendingOperations.Contains(operationCode))
		{
			pendingOperations.Remove(operationCode);
		}
	}

	public bool IsOperationPending(MVOperationCodes operationCode)
	{
		if (pendingOperations.Contains(operationCode))
		{
			return true;
		}
		return false;
	}
}
