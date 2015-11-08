using System;
using UnityEngine;

public class LockCursorManager : MonoBehaviour, ILockCursorManager
{
	private bool hasFocus = true;

	private bool wantsCursorLock;

	private bool prevCursorLock;

	private Action<bool> onCursorLockChanged;

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
			return Cursor.lockState == CursorLockMode.Locked;
		}
		set
		{
			wantsCursorLock = value;
		}
	}

	public bool HasFocusAndLockCursor => hasFocus && Cursor.lockState == CursorLockMode.Locked;

	private void LateUpdate()
	{
		if (UXUtils.UXScreen.Fullscreen)
		{
			hasFocus = true;
		}
		HandleCursorLock();
		Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
	}

	private void OnApplicationFocus(bool focus)
	{
		hasFocus = focus;
	}

	private void HandleCursorLock()
	{
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		if (!uXDialogFactory.DialogOpen && hasFocus && wantsCursorLock)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (prevCursorLock != (Cursor.lockState == CursorLockMode.Locked))
		{
			OnCursorLockChanged(Cursor.lockState == CursorLockMode.Locked);
			prevCursorLock = Cursor.lockState == CursorLockMode.Locked;
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				wantsCursorLock = false;
			}
		}
	}
}
