using System;

public class OnWorldObjectRegisterResponseEventArgs : EventArgs
{
	public int worldObjectID;

	public OnWorldObjectRegisterResponseEventArgs(int worldObjectID)
	{
		this.worldObjectID = worldObjectID;
	}
}
