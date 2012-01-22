using System;

public class TriggerEventArgs : EventArgs
{
	public int instigatorWOID;

	public TriggerEventArgs(int woid)
	{
		instigatorWOID = woid;
	}
}
