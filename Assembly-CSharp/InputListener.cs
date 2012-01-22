using UnityEngine;

internal class InputListener : MonoBehaviour, IInputHandler
{
	private AvatarController controller;

	public int Priority => InputHandlerPriority.GAME;

	public void Init(AvatarController controller)
	{
		this.controller = controller;
	}

	public void Awake()
	{
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
	}

	public bool HandleInput()
	{
		if (MVInputWrapper.GetKeyDown((KeyCode)119))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Up);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)115))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Down);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)97))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Left);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)100))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Right);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)32))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Jump);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)119))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Up);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)115))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Down);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)97))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Left);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)100))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Right);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)32))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Jump);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)273))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Up);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)274))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Down);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)276))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Left);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)275))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Right);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)273))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Up);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)274))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Down);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)276))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Left);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)275))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Right);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)304) || MVInputWrapper.GetKeyUp((KeyCode)303))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Shift);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)304) || MVInputWrapper.GetKeyDown((KeyCode)303))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Shift);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)105))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.ToggleCloack);
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323))
		{
			controller.HandleInput(NetworkInputActionCodes.Down, NetworkInputKeyCodes.Fire);
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			controller.HandleInput(NetworkInputActionCodes.Up, NetworkInputKeyCodes.Fire);
		}
		return false;
	}
}
