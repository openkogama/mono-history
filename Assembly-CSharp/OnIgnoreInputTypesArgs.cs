using System;

public class OnIgnoreInputTypesArgs : EventArgs
{
	public IgnoreInputTypes inputTypes;

	public OnIgnoreInputTypesArgs(IgnoreInputTypes inputTypes)
	{
		this.inputTypes = inputTypes;
	}
}
