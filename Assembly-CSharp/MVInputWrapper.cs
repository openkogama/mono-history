using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

internal static class MVInputWrapper
{
	private static int inputSuppressedFrame = 0;

	private static int inputInGameInputSuppressedFrame = 0;

	private static int suppressShortcutKeysFrame = 0;

	private static float prevMouseUpTime;

	private static Dictionary<string, bool> usedAxes = new Dictionary<string, bool>();

	private static IKogamaInputMap inputMap = new DesktopDefaultKeyboardMapping();

	public static bool IsInputSuppressed
	{
		get
		{
			if (Mathf.Abs(Time.frameCount - inputSuppressedFrame) < 2)
			{
				return true;
			}
			return false;
		}
		set
		{
			inputSuppressedFrame = Time.frameCount;
		}
	}

	public static bool IsShortcutKeysSuppressed
	{
		get
		{
			if (Mathf.Abs(Time.frameCount - suppressShortcutKeysFrame) < 2)
			{
				return true;
			}
			return false;
		}
		set
		{
			suppressShortcutKeysFrame = Time.frameCount;
		}
	}

	public static bool IsInGameInputSuppressed
	{
		get
		{
			if (Mathf.Abs(Time.frameCount - inputInGameInputSuppressedFrame) < 2)
			{
				return true;
			}
			return false;
		}
		set
		{
			inputInGameInputSuppressedFrame = Time.frameCount;
		}
	}

	public static void SetInputMap(IKogamaInputMap inputMap)
	{
		MVInputWrapper.inputMap = inputMap;
	}

	public static bool GetBooleanControl(KogamaControls control, bool forceKeyUse = false)
	{
		return GetBooleanControl(control, KeyState.Pressed, forceKeyUse);
	}

	public static bool GetBooleanControlDown(KogamaControls control, bool forceKeyUse = false)
	{
		return GetBooleanControl(control, KeyState.Down, forceKeyUse);
	}

	public static bool GetBooleanControlUp(KogamaControls control, bool forceKeyUse = false)
	{
		return GetBooleanControl(control, KeyState.Up, forceKeyUse);
	}

	private static bool GetBooleanControl(KogamaControls control, KeyState keyState, bool forceKeyUse)
	{
		if (!forceKeyUse && IsInputSuppressed && control != KogamaControls.PointerSelect && control != KogamaControls.PointerSelectAlt)
		{
			return false;
		}
		if (inputMap == null)
		{
			return false;
		}
		return inputMap.GetBooleanControl(control, keyState);
	}

	public static bool InputCharActive(KeyCode key)
	{
		return Input.GetKey(key);
	}

	public static bool InputCharActiveDown(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}

	public static string GetStringInput()
	{
		return Input.inputString;
	}

	public static Vector3 GetPointerPosition()
	{
		return Input.mousePosition;
	}

	public static bool DebugGetKeyDown(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}

	public static bool DebugGetKeyDown(string st)
	{
		return Input.GetKeyDown(st);
	}

	public static bool DebugGetKey(KeyCode key)
	{
		return Input.GetKey(key);
	}

	public static bool DebugGetKey(string st)
	{
		return Input.GetKey(st);
	}

	public static bool DebugGetKeyUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}

	public static bool DebugGetKeyUp(string st)
	{
		return Input.GetKeyUp(st);
	}

	public static float GetAxis(string axis)
	{
		if (IsInputSuppressed || IsInGameInputSuppressed)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		if (IsInputSuppressed || IsInGameInputSuppressed)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxisRaw(axis);
	}
}
