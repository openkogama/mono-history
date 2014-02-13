using System;

public class InitializedGameQueryDataEventArgs : EventArgs
{
	public readonly MVWorldObjectClient RootWO;

	public readonly int InstigatorActorNumber;

	public InitializedGameQueryDataEventArgs(MVWorldObjectClient rootWO, int instigatorActorNumber)
	{
		RootWO = rootWO;
		InstigatorActorNumber = instigatorActorNumber;
	}
}
