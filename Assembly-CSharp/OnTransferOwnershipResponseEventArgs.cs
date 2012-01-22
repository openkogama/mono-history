using System;

public class OnTransferOwnershipResponseEventArgs : EventArgs
{
	public int worldObjectID;

	public int ownerActorNr;

	public bool success;

	public OnTransferOwnershipResponseEventArgs(int worldObjectID, int ownerActorNr, bool success)
	{
		this.worldObjectID = worldObjectID;
		this.ownerActorNr = ownerActorNr;
		this.success = success;
	}
}
