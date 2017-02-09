using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MobileInputMap : IKogamaInputMap
{
	private Dictionary<KogamaControls, string[]> ButtonMapping = new Dictionary<KogamaControls, string[]>
	{
		{
			KogamaControls.Jump,
			new string[1] { "Jump" }
		},
		{
			KogamaControls.Use,
			new string[1] { "Use" }
		},
		{
			KogamaControls.Fire,
			new string[1] { "Fire" }
		},
		{
			KogamaControls.Respawn,
			new string[1] { "Respawn" }
		},
		{
			KogamaControls.DropCurrentItem,
			new string[1] { "DropWeapon" }
		},
		{
			KogamaControls.Holster,
			new string[1] { "Holster" }
		}
	};

	private Dictionary<KogamaControls, KeyCode> KeyCodeMapping = new Dictionary<KogamaControls, KeyCode> { 
	{
		KogamaControls.Escape,
		KeyCode.Escape
	} };

	public bool GetBooleanControl(KogamaControls control, KeyState keyState, int index = -1)
	{
		if (ButtonMapping.Keys.Contains(control))
		{
			string[] array = ButtonMapping[control];
			foreach (string name in array)
			{
				switch (keyState)
				{
				case KeyState.Pressed:
					return CrossPlatformInputManager.GetButton(name);
				case KeyState.Down:
					return CrossPlatformInputManager.GetButtonDown(name);
				case KeyState.Up:
					return CrossPlatformInputManager.GetButtonUp(name);
				}
			}
		}
		else if (KeyCodeMapping.Keys.Contains(control))
		{
			switch (keyState)
			{
			case KeyState.Pressed:
				return Input.GetKey(KeyCodeMapping[control]);
			case KeyState.Down:
				return Input.GetKeyDown(KeyCodeMapping[control]);
			case KeyState.Up:
				return Input.GetKeyUp(KeyCodeMapping[control]);
			}
		}
		else
		{
			Debug.LogWarning("Not implemented on mobile " + control);
		}
		return false;
	}
}
