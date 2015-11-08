using System;
using UnityEngine;

public class LockCursorManagerPlatformer : MonoBehaviour, ILockCursorManager
{
	private bool hasFocus = true;

	private bool wantsCursorLock;

	private Action<bool> onCursorLockChanged;

	private bool prevCursorLock;

	public Action<bool> OnCursorLockChanged
	{
		get
		{
			return onCursorLockChanged;
		}
		set
		{
			onCursorLockChanged = value;
		}
	}

	public bool LockCursor
	{
		get
		{
			return wantsCursorLock;
		}
		set
		{
			wantsCursorLock = value;
		}
	}

	public bool HasFocusAndLockCursor => hasFocus && wantsCursorLock;

	private void LateUpdate()
	{
		if (UXUtils.UXScreen.Fullscreen)
		{
			hasFocus = true;
		}
		HandleCursorLock();
	}

	private void OnApplicationFocus(bool focus)
	{
		hasFocus = focus;
	}

	private void HandleCursorLock()
	{
		bool flag = hasFocus && wantsCursorLock;
		if (prevCursorLock != flag)
		{
			OnCursorLockChanged(flag);
			prevCursorLock = flag;
		}
	}
}
