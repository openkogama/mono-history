using System;
using MV.Common;

public class GameStateChangeEventArgs : EventArgs
{
	public int actorNr;

	public MVGameStateReason reason;

	public GameStateChangeEventArgs(int actorNr, MVGameStateReason reason)
	{
		this.actorNr = actorNr;
		this.reason = reason;
	}
}
