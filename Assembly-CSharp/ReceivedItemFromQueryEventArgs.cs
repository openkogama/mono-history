using System;
using MV.WorldObject;

public class ReceivedItemFromQueryEventArgs : EventArgs
{
	public readonly BytePacker KoGaMaData;

	public readonly int InstigatorActorNumber;

	public ReceivedItemFromQueryEventArgs(BytePacker koGaMaData, int instigatorActorNumber)
	{
		KoGaMaData = koGaMaData;
		InstigatorActorNumber = instigatorActorNumber;
	}
}
