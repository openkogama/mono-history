using System;

public class SelectedEventArgs : EventArgs
{
	public readonly bool Selected;

	public SelectedEventArgs(bool selected)
	{
		Selected = selected;
	}
}
