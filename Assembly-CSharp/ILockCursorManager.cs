using System;

public interface ILockCursorManager
{
	Action<bool> OnCursorLockChanged { get; set; }

	bool CursorLock { get; set; }

	bool CursorLockWithoutCallback { set; }
}
