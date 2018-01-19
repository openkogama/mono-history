using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

internal static class MVInputWrapper
{
	public class InputSuppression
	{
		private int suppressionFrame;

		protected virtual bool IsSuppressed
		{
			get
			{
				int num = Mathf.Abs(Time.frameCount - suppressionFrame);
				return num < 2;
			}
			set
			{
				suppressionFrame = (value ? Time.frameCount : 0);
			}
		}

		protected InputSuppression(bool a)
		{
			IsSuppressed = a;
		}

		public static implicit operator InputSuppression(bool a)
		{
			return new InputSuppression(a);
		}

		public static implicit operator bool(InputSuppression a)
		{
			return a.IsSuppressed;
		}
	}

	private static InputSuppression isInputAllSuppressed = false;

	private static InputSuppression isShortcutKeysSuppressed = false;

	private static InputSuppression isInGameInputSuppressed = false;

	private static IKogamaInputMap inputMap;

	public static bool IsAllInputSuppressed => isInputAllSuppressed;

	public static bool IsShortcutKeysSuppressed => (bool)isShortcutKeysSuppressed || IsAllInputSuppressed;

	public static bool IsInGameInputSuppressed => (bool)isInGameInputSuppressed || IsAllInputSuppressed;

	public static void SuppressAllInput()
	{
		isInputAllSuppressed = true;
	}

	public static void SuppressShortcutKeys()
	{
		isShortcutKeysSuppressed = true;
	}

	public static void SuppressInGameInput()
	{
		isInGameInputSuppressed = true;
	}

	public static void SetInputMap(IKogamaInputMap inputMap)
	{
		MVInputWrapper.inputMap = inputMap;
	}

	public static bool GetBooleanControl(KogamaControls control)
	{
		return GetBooleanControl(control, KeyState.Pressed);
	}

	public static bool GetBooleanControlDown(KogamaControls control)
	{
		return GetBooleanControl(control, KeyState.Down);
	}

	public static bool GetBooleanControlUp(KogamaControls control)
	{
		return GetBooleanControl(control, KeyState.Up);
	}

	private static bool GetBooleanControl(KogamaControls control, KeyState keyState)
	{
		if (IsAllInputSuppressed && control != KogamaControls.PointerSelect && control != KogamaControls.PointerSelectAlt)
		{
			return false;
		}
		if (inputMap == null)
		{
			return false;
		}
		return inputMap.GetBooleanControl(control, keyState);
	}

	public static Vector3 GetPointerPosition()
	{
		return Input.mousePosition;
	}

	public static float GetAxis(string axis)
	{
		if (IsInGameInputSuppressed)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		if (IsInGameInputSuppressed)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxisRaw(axis);
	}

	public static void ResetInput()
	{
		((DesktopDefaultKeyboardMapping)inputMap).Reset();
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
}
