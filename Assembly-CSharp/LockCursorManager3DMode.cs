using System;
using UnityEngine;

public class LockCursorManager3DMode : MonoBehaviour, ILockCursorManager
{
	public Action<bool> OnCursorLockChanged { get; set; }

	public bool CursorLock
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
		set
		{
			if (value)
			{
				LockCursor();
			}
			else
			{
				UnlockCursor();
			}
		}
	}

	protected void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			UnlockCursor();
		}
	}

	protected void LockCursor()
	{
		if (!CursorLock)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			OnCursorLockChanged(CursorLock);
		}
	}

	protected void UnlockCursor()
	{
		if (CursorLock)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			OnCursorLockChanged(CursorLock);
		}
	}
}
