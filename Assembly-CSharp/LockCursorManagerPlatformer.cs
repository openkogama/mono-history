using System;
using UnityEngine;

public class LockCursorManagerPlatformer : MonoBehaviour, ILockCursorManager
{
	private bool hasFocus = true;

	private bool wantsCursorLock;

	private bool crosshairVisible;

	private Action<bool> onCursorLockChanged;

	[SerializeField]
	private Texture2D crosshairCursor;

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

	public MVGUICrossHairLegacy GUICrossHairLegacy { private get; set; }

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
		if (GUICrossHairLegacy != null && crosshairVisible != GUICrossHairLegacy.Visible)
		{
			crosshairVisible = GUICrossHairLegacy.Visible;
			if (crosshairVisible)
			{
				Debug.Log("Set cursor to crosshair");
				Cursor.SetCursor(crosshairCursor, new Vector2(16f, 16f), CursorMode.Auto);
			}
			else
			{
				Debug.Log("Set to default");
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
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
