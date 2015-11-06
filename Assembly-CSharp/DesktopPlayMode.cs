using UnityEngine;

public class DesktopPlayMode : DesktopDefaultKeyboardMapping
{
	public DesktopPlayMode()
	{
		keyMapping.Add(KogamaControls.MoveForward, new KeyCode[2]
		{
			KeyCode.W,
			KeyCode.UpArrow
		});
		keyMapping.Add(KogamaControls.MoveBackwards, new KeyCode[2]
		{
			KeyCode.S,
			KeyCode.DownArrow
		});
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
			KeyCode.Space,
			KeyCode.Keypad0
		});
		keyMapping.Add(KogamaControls.Fire, new KeyCode[1] { KeyCode.Mouse0 });
	}
}
