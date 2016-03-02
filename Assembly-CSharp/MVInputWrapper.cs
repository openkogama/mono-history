using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

internal static class MVInputWrapper
{
	public static bool ignoreInGameInput = false;

	public static bool ignoreAllKeys = false;

	public static bool hasLostFocus = false;

	private static float prevMouseUpTime;

	private static Dictionary<string, bool> usedAxes = new Dictionary<string, bool>();

	private static Vector3 prevMousePos = default;

	private static DateTime latestMouseMoveTime = DateTime.Now;

	private static IKogamaInputMap inputMap = null;

	public static DateTime LatestMouseMoveTime => latestMouseMoveTime;

	public static void SetInputMap(IKogamaInputMap inputMap)
	{
		MVInputWrapper.inputMap = inputMap;
	}

	public static void Update()
	{
		if (Input.mousePosition != prevMousePos || GetAxisRaw("Mouse ScrollWheel") > Mathf.Epsilon || GetAxisRaw("Mouse X") > Mathf.Epsilon || GetAxisRaw("Mouse Y") > Mathf.Epsilon)
		{
			prevMousePos = Input.mousePosition;
			latestMouseMoveTime = DateTime.Now;
		}
	}

	public static void Reset()
	{
		hasLostFocus = false;
	}

	public static bool GetBooleanControl(KogamaControls control, bool forceKeyUse = false, int index = -1)
	{
		return GetBooleanControl(control, KeyState.Pressed, forceKeyUse, index);
	}

	public static bool GetBooleanControlDown(KogamaControls control, bool forceKeyUse = false, int index = -1)
	{
		return GetBooleanControl(control, KeyState.Down, forceKeyUse, index);
	}

	public static bool GetBooleanControlUp(KogamaControls control, bool forceKeyUse = false, int index = -1)
	{
		return GetBooleanControl(control, KeyState.Up, forceKeyUse, index);
	}

	private static bool GetBooleanControl(KogamaControls control, KeyState keyState, bool forceKeyUse, int index = -1)
	{
		if (!forceKeyUse && ignoreAllKeys && control != KogamaControls.PointerSelect && control != KogamaControls.PointerSelectAlt)
		{
			return false;
		}
		if (inputMap == null)
		{
			return false;
		}
		return inputMap.GetBooleanControl(control, keyState, index);
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
		if (ignoreAllKeys)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		if (ignoreAllKeys)
		{
			return 0f;
		}
		return CrossPlatformInputManager.GetAxisRaw(axis);
	}
}
