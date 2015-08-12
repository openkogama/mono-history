using System;

public class MVLocalPlayerTourist : MVLocalPlayer
{
	public MVLocalPlayerTourist(int actorNumber, int profileID, string userName, string regionCode)
		: base(actorNumber, profileID, userName, regionCode)
	{
		xpEventQueue = new XPEventQueueTourist();
	}

	public override void InitializeLeveling(InitialLevelData initialLevelData)
	{
		base.InitializeLeveling(initialLevelData);
		XPEventQueue xPEventQueue = xpEventQueue;
		xPEventQueue.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(xPEventQueue.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPProgressDataChangeTourist));
	}

	private void OnXPProgressDataChangeTourist(XPProgressData xpProgress)
	{
		if (xpProgress.XPLimitExceeded)
		{
			Level++;
		}
		else
		{
			SendXpProgressEvent(xpProgress);
		}
	}
}
