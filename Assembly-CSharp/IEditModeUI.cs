using System;

public interface IEditModeUI
{
	bool IsInPlayInEditMode { get; }

	Action<EditModeChangeArgs> EditModeChange { get; set; }

	ClientShopRepository ClientShopRepository { get; set; }

	PlayerInventoryRepository PlayerInventoryRepository { get; set; }

	bool IsGridSnap();
}
