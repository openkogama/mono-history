using System;

public class OnTransferWosResponseEventArgs : EventArgs
{
	public readonly bool success;

	public OnTransferWosResponseEventArgs(bool success)
	{
		this.success = success;
	}
}
