public class PlayButton : PlayButtonBase
{
	public void Play()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
	}

	private void Update()
	{
		UpdateButton();
	}
}
