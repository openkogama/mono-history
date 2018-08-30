using System;

public class RewardStateDataEventArgs : EventArgs
{
	public readonly TimeSpan timeSpan;

	public readonly int amountGold;

	public RewardStateDataEventArgs(int timeInSeconds, int amountGold)
	{
		timeSpan = new TimeSpan(0, 0, 0, timeInSeconds);
		this.amountGold = amountGold;
	}
}
