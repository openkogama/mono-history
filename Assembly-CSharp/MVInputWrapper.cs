using System;
using System.Collections.Generic;
using UnityEngine;

internal static class MVInputWrapper
{
	public static bool ignoreAllKeys = false;

	private static float prevMouseUpTime;

	private static Vector3 prevMouseUpPos = default;

	private static HashSet<KeyCode> keyUsed = new HashSet<KeyCode>();

	private static HashSet<KeyCode> keyDownUsed = new HashSet<KeyCode>();

	private static HashSet<KeyCode> keyUpUsed = new HashSet<KeyCode>();

	private static Dictionary<string, bool> usedAxes = new Dictionary<string, bool>();

	private static Dictionary<char, KeyCode[]> char2keyCode = new Dictionary<char, KeyCode[]>
	{
		{
			'A',
			new KeyCode[1] { (KeyCode)97 }
		},
		{
			'B',
			new KeyCode[1] { (KeyCode)98 }
		},
		{
			'C',
			new KeyCode[1] { (KeyCode)99 }
		},
		{
			'D',
			new KeyCode[1] { (KeyCode)100 }
		},
		{
			'E',
			new KeyCode[1] { (KeyCode)101 }
		},
		{
			'F',
			new KeyCode[1] { (KeyCode)102 }
		},
		{
			'G',
			new KeyCode[1] { (KeyCode)103 }
		},
		{
			'H',
			new KeyCode[1] { (KeyCode)104 }
		},
		{
			'I',
			new KeyCode[1] { (KeyCode)105 }
		},
		{
			'J',
			new KeyCode[1] { (KeyCode)106 }
		},
		{
			'K',
			new KeyCode[1] { (KeyCode)107 }
		},
		{
			'L',
			new KeyCode[1] { (KeyCode)108 }
		},
		{
			'M',
			new KeyCode[1] { (KeyCode)109 }
		},
		{
			'N',
			new KeyCode[1] { (KeyCode)110 }
		},
		{
			'O',
			new KeyCode[1] { (KeyCode)111 }
		},
		{
			'P',
			new KeyCode[1] { (KeyCode)112 }
		},
		{
			'Q',
			new KeyCode[1] { (KeyCode)113 }
		},
		{
			'R',
			new KeyCode[1] { (KeyCode)114 }
		},
		{
			'S',
			new KeyCode[1] { (KeyCode)115 }
		},
		{
			'T',
			new KeyCode[1] { (KeyCode)116 }
		},
		{
			'U',
			new KeyCode[1] { (KeyCode)117 }
		},
		{
			'V',
			new KeyCode[1] { (KeyCode)118 }
		},
		{
			'W',
			new KeyCode[1] { (KeyCode)119 }
		},
		{
			'X',
			new KeyCode[1] { (KeyCode)120 }
		},
		{
			'Y',
			new KeyCode[1] { (KeyCode)121 }
		},
		{
			'Z',
			new KeyCode[1] { (KeyCode)122 }
		},
		{
			'0',
			new KeyCode[2]
			{
				(KeyCode)48,
				(KeyCode)256
			}
		},
		{
			'1',
			new KeyCode[2]
			{
				(KeyCode)49,
				(KeyCode)257
			}
		},
		{
			'2',
			new KeyCode[2]
			{
				(KeyCode)50,
				(KeyCode)258
			}
		},
		{
			'3',
			new KeyCode[2]
			{
				(KeyCode)51,
				(KeyCode)259
			}
		},
		{
			'4',
			new KeyCode[2]
			{
				(KeyCode)52,
				(KeyCode)260
			}
		},
		{
			'5',
			new KeyCode[2]
			{
				(KeyCode)53,
				(KeyCode)261
			}
		},
		{
			'6',
			new KeyCode[2]
			{
				(KeyCode)54,
				(KeyCode)262
			}
		},
		{
			'7',
			new KeyCode[2]
			{
				(KeyCode)55,
				(KeyCode)263
			}
		},
		{
			'8',
			new KeyCode[2]
			{
				(KeyCode)56,
				(KeyCode)264
			}
		},
		{
			'9',
			new KeyCode[2]
			{
				(KeyCode)57,
				(KeyCode)265
			}
		},
		{
			' ',
			new KeyCode[1] { (KeyCode)32 }
		},
		{
			'\b',
			new KeyCode[1] { (KeyCode)8 }
		}
	};

	static MVInputWrapper()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void Reset()
	{
		keyDownUsed.RemoveWhere((KeyCode keyCode) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return !Input.GetKeyDown(keyCode);
		});
		keyUsed.RemoveWhere((KeyCode keyCode) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return !Input.GetKey(keyCode);
		});
		keyUpUsed.RemoveWhere((KeyCode keyCode) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return !Input.GetKeyUp(keyCode);
		});
	}

	public static void RegisterKeyAsUsed(char c)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (char2keyCode.TryGetValue(char.ToUpper(c), out var value))
		{
			KeyCode[] array = value;
			foreach (KeyCode key in array)
			{
				RegisterKeyDownAsUsed(key);
				RegisterKeyAsUsed(key);
				RegisterKeyUpAsUsed(key);
			}
		}
		else
		{
			Debug.LogWarning((object)$"Character '{c}' with code U+{(int)c:x4} ({(int)c}) could not be registered as used.");
		}
	}

	public static void RegisterKeyDownAsUsed(KeyCode key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		keyDownUsed.Add(key);
	}

	public static void RegisterKeyAsUsed(KeyCode key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		keyUsed.Add(key);
	}

	public static void RegisterKeyUpAsUsed(KeyCode key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		keyUpUsed.Add(key);
	}

	public static bool KeyDownHasBeenUsed(KeyCode key)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ignoreAllKeys || keyDownUsed.Contains(key);
	}

	public static bool KeyHasBeenUsed(KeyCode key)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ignoreAllKeys || keyUsed.Contains(key);
	}

	public static bool KeyUpHasBeenUsed(KeyCode key)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ignoreAllKeys || keyUpUsed.Contains(key);
	}

	public static bool GetKey(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKey(key, useKey: false);
	}

	public static bool GetKey(KeyCode key, bool useKey)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKey(key, useKey, (Func<KeyCode, bool>)Input.GetKey, keyUsed);
	}

	public static bool GetKeyDown(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKeyDown(key, useKey: false);
	}

	public static bool GetKeyDown(KeyCode key, bool useKey)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKey(key, useKey, (Func<KeyCode, bool>)Input.GetKeyDown, keyDownUsed);
	}

	public static bool GetKeyUp(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKeyUp(key, useKey: false);
	}

	public static bool GetKeyUp(KeyCode key, bool useKey)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GetKey(key, useKey, (Func<KeyCode, bool>)Input.GetKeyUp, keyUpUsed);
	}

	private static bool GetKey(KeyCode key, bool useKey, Func<KeyCode, bool> inputFun, HashSet<KeyCode> usedKeyCodes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		bool flag = inputFun(key);
		if (flag)
		{
			if (usedKeyCodes.Contains(key))
			{
				return false;
			}
			if (useKey)
			{
				usedKeyCodes.Add(key);
			}
		}
		return flag;
	}

	public static bool GetDoubleClick(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (KeyHasBeenUsed(key))
		{
			return false;
		}
		if (GetKeyUp(key))
		{
			if ((double)(Time.time - prevMouseUpTime) < 0.5)
			{
				Vector3 val = Input.mousePosition - prevMouseUpPos;
				if (val.magnitude < 5f)
				{
					prevMouseUpPos = Input.mousePosition;
					prevMouseUpTime = Time.time;
					return true;
				}
			}
			prevMouseUpPos = Input.mousePosition;
			prevMouseUpTime = Time.time;
		}
		return false;
	}

	public static float GetAxis(string axis)
	{
		if (ignoreAllKeys)
		{
			return 0f;
		}
		return Input.GetAxis(axis);
	}

	public static float GetAxisRaw(string axis)
	{
		if (ignoreAllKeys)
		{
			return 0f;
		}
		return Input.GetAxisRaw(axis);
	}
}
