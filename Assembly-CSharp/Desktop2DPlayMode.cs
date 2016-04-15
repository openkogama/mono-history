using UnityEngine;

public class Desktop2DPlayMode : DesktopDefaultKeyboardMapping
{
	public Desktop2DPlayMode()
	{
		keyMapping.Add(KogamaControls.MoveForward, new KeyCode[0]);
		keyMapping.Add(KogamaControls.MoveBackwards, new KeyCode[0]);
		keyMapping.Add(KogamaControls.MoveLeft, new KeyCode[2]
		{
			KeyCode.A,
			KeyCode.LeftArrow
		});
		keyMapping.Add(KogamaControls.MoveRight, new KeyCode[2]
		{
			KeyCode.D,
			KeyCode.RightArrow
		});
		keyMapping.Add(KogamaControls.Jump, new KeyCode[2]
		{
			KeyCode.UpArrow,
			KeyCode.Space
		});
		keyMapping.Add(KogamaControls.Fire, new KeyCode[3]
		{
			KeyCode.Mouse0,
			KeyCode.Keypad0,
			KeyCode.LeftShift
		});
	}
}
