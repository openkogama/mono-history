using System;
using UnityEngine;

public class LockCursorManager3DMode : MonoBehaviour, ILockCursorManager
{
	private class OverrideCursorUnLockState
	{
		private int activeFrame;

		public readonly bool cursorVisibleBeforeOverride;

		public readonly CursorLockMode cursorLockModeBeforeOverride;

		public bool Active
		{
			get
			{
				if (Mathf.Abs(Time.frameCount - activeFrame) < 2)
				{
					return true;
				}
				return false;
			}
			set
			{
				activeFrame = Time.frameCount;
			}
		}

		public OverrideCursorUnLockState(bool cursorVisible, CursorLockMode cursorLockMode)
		{
			cursorVisibleBeforeOverride = cursorVisible;
			cursorLockModeBeforeOverride = cursorLockMode;
			Active = true;
		}
	}

	private bool hasFocus = true;

	private bool wantsCursorLock;

	private bool prevCursorLock;

	private OverrideCursorUnLockState overrideCursorUnLockState;

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
			if (overrideCursorUnLockState != null)
			{
				return overrideCursorUnLockState.cursorLockModeBeforeOverride == CursorLockMode.Locked;
			}
			return Cursor.lockState == CursorLockMode.Locked;
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
			if (overrideCursorUnLockState == null)
			{
				overrideCursorUnLockState = new OverrideCursorUnLockState(Cursor.visible, Cursor.lockState);
			}
			overrideCursorUnLockState.Active = value;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public bool HasFocusAndLockCursor
	{
		get
		{
			if (overrideCursorUnLockState != null)
			{
				return overrideCursorUnLockState.cursorLockModeBeforeOverride == CursorLockMode.Locked && hasFocus;
			}
			return hasFocus && Cursor.lockState == CursorLockMode.Locked;
		}
	}

	private void LateUpdate()
	{
		if (overrideCursorUnLockState != null)
		{
			if (!overrideCursorUnLockState.Active)
			{
				Cursor.visible = overrideCursorUnLockState.cursorVisibleBeforeOverride;
				Cursor.lockState = overrideCursorUnLockState.cursorLockModeBeforeOverride;
				overrideCursorUnLockState = null;
			}
		}
		else
		{
			if (Screen.fullScreen)
			{
				hasFocus = true;
			}
			HandleCursorLock();
			Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
		}
	}

	private void OnDisable()
	{
		LateUpdate();
	}

	private void OnApplicationFocus(bool focus)
	{
		hasFocus = focus;
	}

	private void HandleCursorLock()
	{
		if (hasFocus && wantsCursorLock)
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
