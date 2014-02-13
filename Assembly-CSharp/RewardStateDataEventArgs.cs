using System;

public class RewardStateDataEventArgs : EventArgs
{
	public readonly TimeSpan timeSpan;

	public readonly int amountGold;

	public readonly int amountSilver;

	public RewardStateDataEventArgs(int timeInSeconds, int amountGold, int amountSilver)
	{
		timeSpan = new TimeSpan(0, 0, 0, timeInSeconds);
		this.amountGold = amountGold;
		this.amountSilver = amountSilver;
	}
}
