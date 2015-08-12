public class XPEventQueueTourist : XPEventQueue
{
	protected override void RequestXp(XPData xpData)
	{
		xpProgress.Update(xpProgress.XP + xpData.XPAmount, xpData.XPId);
	}
}
