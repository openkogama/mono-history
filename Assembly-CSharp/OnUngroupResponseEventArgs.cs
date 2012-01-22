using System;

public class OnUngroupResponseEventArgs : EventArgs
{
	public int worldObjectID;

	public bool success;

	public OnUngroupResponseEventArgs(int worldObjectID, bool success)
	{
		this.worldObjectID = worldObjectID;
		this.success = success;
	}
}
