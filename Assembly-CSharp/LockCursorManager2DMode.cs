using System;
using UnityEngine;

public class LockCursorManager2DMode : MonoBehaviour, ILockCursorManager
{
	private bool hasFocus = true;

	private bool wantsCursorLock;

	private Action<bool> onCursorLockChanged;

	private bool showingCrosshairCursor;

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

	public bool UnLockCursorOverride
	{
		set
		{
		}
	}

	public bool HasFocusAndLockCursor => hasFocus && wantsCursorLock;

	private void LateUpdate()
	{
		if (Screen.fullScreen)
		{
			hasFocus = true;
		}
		HandleCursorLock();
		HandleCrosshairCursor();
	}

	private void OnDisable()
	{
		LateUpdate();
	}

	private void HandleCrosshairCursor()
	{
		bool flag = MVGameControllerBase.IPlayModeUI.GetCrossHair().Visible && !MVGameControllerBase.IPlayModeUI.InLobbyState;
		if (flag && !showingCrosshairCursor)
		{
			Cursor.SetCursor(PrefabPool.Instance.CrosshairCursor, new Vector2(16f, 16f), CursorMode.Auto);
			showingCrosshairCursor = true;
		}
		else if (!flag && showingCrosshairCursor)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			showingCrosshairCursor = false;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		hasFocus = focus;
		if (!focus)
		{
			wantsCursorLock = false;
		}
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
