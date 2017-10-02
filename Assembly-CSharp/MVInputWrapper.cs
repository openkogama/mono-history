using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

internal static class MVInputWrapper
{
	public class InputSuppression
	{
		private int suppressionFrame;

		public virtual bool IsSuppressed
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

		public InputSuppression()
		{
			suppressionFrame = 0;
		}

		public InputSuppression(bool b)
		{
			IsSuppressed = b;
		}

		public static implicit operator bool(InputSuppression a)
		{
			return a.IsSuppressed;
		}
	}

	public class InputSuppressionWithReset : InputSuppression
	{
		public override bool IsSuppressed
		{
			get
			{
				return base.IsSuppressed;
			}
			set
			{
				base.IsSuppressed = value;
				if (value)
				{
					((DesktopDefaultKeyboardMapping)inputMap).Reset();
				}
			}
		}
	}

	public static InputSuppressionWithReset isInputSuppressed = new InputSuppressionWithReset();

	public static InputSuppression isShortcutKeysSuppressed = new InputSuppression();

	public static InputSuppressionWithReset isInGameInputSuppressed = new InputSuppressionWithReset();

	private static IKogamaInputMap inputMap = new DesktopDefaultKeyboardMapping();

	public static bool IsInputSuppressed
	{
		get
		{
			return isInputSuppressed;
		}
		set
		{
			isInputSuppressed.IsSuppressed = value;
		}
	}

	public static bool IsShortcutKeysSuppressed
	{
		get
		{
			return isShortcutKeysSuppressed;
		}
		set
		{
			isShortcutKeysSuppressed.IsSuppressed = value;
		}
	}

	public static bool IsInGameInputSuppressed
	{
		get
		{
			return isInGameInputSuppressed;
		}
		set
		{
			isInGameInputSuppressed.IsSuppressed = value;
		}
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
		if (IsInputSuppressed && control != KogamaControls.PointerSelect && control != KogamaControls.PointerSelectAlt)
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
