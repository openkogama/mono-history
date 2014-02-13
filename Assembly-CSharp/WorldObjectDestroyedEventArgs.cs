using System;

public class WorldObjectDestroyedEventArgs : EventArgs
{
	public readonly int WordObjectID;

	public WorldObjectDestroyedEventArgs(int woID)
	{
		WordObjectID = woID;
	}
}
