using System;

public class EditStateEventArgs : EventArgs
{
	public readonly bool BeingEdited;

	public EditStateEventArgs(bool beingEdited)
	{
		BeingEdited = beingEdited;
	}
}
