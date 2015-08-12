using System;

public class WaitForTicksLocal
{
	public readonly int startTicks;

	private readonly uint maxTicks;

	public bool TimeIsUp
	{
		get
		{
			uint num = (uint)(Environment.TickCount - startTicks);
			return num >= maxTicks;
		}
	}

	public WaitForTicksLocal(int milliseconds)
	{
		double num = milliseconds;
		if (num > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("milliseconds", milliseconds, "Cannot wait for more than Int32.MaxValue milliseconds");
		}
		maxTicks = (uint)num;
		startTicks = Environment.TickCount;
	}

	public static int GetEnvironmentTick(int deltaMilliseconds)
	{
		return Environment.TickCount + deltaMilliseconds;
	}

	public static int Diff(int startTicks)
	{
		return Environment.TickCount - startTicks;
	}
}
