using System;

public interface ILockCursorManager
{
	Action<bool> OnCursorLockChanged { get; set; }

	bool LockCursor { get; set; }

	bool HasFocusAndLockCursor { get; }
}
