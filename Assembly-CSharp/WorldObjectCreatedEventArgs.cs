using System;

public class WorldObjectCreatedEventArgs : EventArgs
{
	public readonly MVWorldObjectClient WorldObject;

	public WorldObjectCreatedEventArgs(MVWorldObjectClient wo)
	{
		WorldObject = wo;
	}
}
