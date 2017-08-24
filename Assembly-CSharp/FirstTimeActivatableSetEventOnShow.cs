public class FirstTimeActivatableSetEventOnShow : FirstTimeActivatableElementBase
{
	private bool setFirstTimeEvent;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	private void Update()
	{
		if (setFirstTimeEvent)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
			setFirstTimeEvent = false;
		}
	}

	public override void OnShow()
	{
		setFirstTimeEvent = true;
	}
}
