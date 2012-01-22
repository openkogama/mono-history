using System;

namespace MV.WorldObject;

public class LastInstanceRemovedEventArgs : EventArgs
{
	public int prototypeID;

	public LastInstanceRemovedEventArgs(int prototypeID)
	{
		this.prototypeID = prototypeID;
	}
}
