using System;

public class OnRequestedPrototypeCreatedEventArgs : EventArgs
{
	public int prototypeId;

	public OnRequestedPrototypeCreatedEventArgs(int prototypeId)
	{
		this.prototypeId = prototypeId;
	}
}
