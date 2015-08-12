using System;
using UnityEngine;

public class LockCursorManager : MonoBehaviour
{
	private static bool hasFocus = true;

	private static bool wantsCursorLock;

	private static bool prevCursorLock;

	public static Action<bool> OnCursorLockChanged;

	public static bool LockCursor
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

	public static bool HasFocusAndLockCursor => hasFocus && Cursor.lockState == CursorLockMode.Locked;

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
