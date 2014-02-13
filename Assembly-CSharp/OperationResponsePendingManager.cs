using System.Collections.Generic;
using ExitGames.Client.Photon.Lite;
using MV.Common;
using UnityEngine;

public class OperationResponsePendingManager
{
	private HashSet<MVOperationCodes> pendingOperations = new HashSet<MVOperationCodes>();

	private LitePeer peer;

	public OperationResponsePendingManager(LitePeer peer)
	{
		this.peer = peer;
	}

	public void AddOperationCodeToPending(MVOperationCodes operationCode, Dictionary<byte, object> data)
	{
		if (IsOperationPending(operationCode))
		{
			Debug.LogError((object)"Operation is already pending!");
			return;
		}
		pendingOperations.Add(operationCode);
		peer.OpCustom((byte)operationCode, data, sendReliable: true);
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
