public class AvatarAccessoryControllerPlayMode : AvatarAccessoryController
{
	private AccessoryMover accessoryMover;

	public bool AccessoryMoveOverride
	{
		get
		{
			return MVGameController.WOCM.AvatarLocal.Body.AccessoryMoveOverride;
		}
		set
		{
			MVGameController.WOCM.AvatarLocal.Body.AccessoryMoveOverride = value;
		}
	}

	public void Initialize()
	{
		accessoryMover = new AccessoryMover();
	}

	public void HandleInput()
	{
		accessoryMover.MoveAccessory();
	}
}
