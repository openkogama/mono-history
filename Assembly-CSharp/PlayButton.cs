public class PlayButton : PlayButtonBase
{
	public void Play()
	{
		MVGameControllerDesktop.LockCursorManager.LockCursor = true;
	}

	private void Update()
	{
		UpdateButton();
	}
}
