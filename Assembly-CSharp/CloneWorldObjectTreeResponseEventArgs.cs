using System;

public class CloneWorldObjectTreeResponseEventArgs : EventArgs
{
	public readonly int RootId;

	public readonly bool Success;

	public CloneWorldObjectTreeResponseEventArgs(bool success, int rootId)
	{
		Success = success;
		RootId = rootId;
	}
}
