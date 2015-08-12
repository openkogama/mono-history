public class SessionTime : IUpdatecontrollerSubscriber
{
	private WaitForTicks waitForTicks;

	private int waitTime = 60000;

	public SessionTime()
	{
		waitForTicks = new WaitForTicks(waitTime);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void UpdateControllerUpdate()
	{
		if (waitForTicks.TimeIsUp)
		{
			waitForTicks = new WaitForTicks(waitTime);
			GameSessionCounters.Increment(GameSessionCounterType.Session1Min);
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
