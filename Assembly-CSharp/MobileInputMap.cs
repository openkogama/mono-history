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
		}
	};

	public bool GetBooleanControl(KogamaControls control, KeyState keyState, int index = -1)
	{
		if (!ButtonMapping.Keys.Contains(control))
		{
			Debug.LogWarning("Not implemented on mobile " + control);
		}
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
		return false;
	}
}
