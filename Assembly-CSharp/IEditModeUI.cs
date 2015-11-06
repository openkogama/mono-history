using System;

public interface IEditModeUI
{
	Action<EditModeChangeArgs> EditModeChange { get; set; }

	bool IsGridSnap();
}
