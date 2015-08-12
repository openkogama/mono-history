using System;

public class WaitForTicks
{
	public readonly int startTicks;

	private readonly uint maxTicks;

	public bool TimeIsUp
	{
		get
		{
			uint num = (uint)(MVGameController.Game.ServerTimeInMilliSeconds - startTicks);
			return num >= maxTicks;
		}
	}

	public WaitForTicks(int milliseconds)
	{
		double num = milliseconds;
		if (num > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("milliseconds", milliseconds, "Cannot wait for more than Int32.MaxValue milliseconds");
		}
		maxTicks = (uint)num;
		startTicks = MVGameController.Game.ServerTimeInMilliSeconds;
	}

	public static int GetEnvironmentTick(int deltaMilliseconds)
	{
		return MVGameController.Game.ServerTimeInMilliSeconds + deltaMilliseconds;
	}

	public static int Diff(int startTicks)
	{
		return MVGameController.Game.ServerTimeInMilliSeconds - startTicks;
	}
}
