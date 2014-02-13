using System;

public class DeterministicSyncedInterval
{
	private readonly int range;

	private int nextTickThres;

	public DeterministicSyncedInterval(int id, int range)
	{
		this.range = range;
		int intWithinRange = GetIntWithinRange(id, range);
		int environmentTick = WaitForTicks.GetEnvironmentTick(0);
		intWithinRange = CalcOffset(environmentTick, intWithinRange, range);
		nextTickThres = environmentTick + intWithinRange;
	}

	public bool Update()
	{
		bool result = false;
		int environmentTick = WaitForTicks.GetEnvironmentTick(0);
		if (environmentTick >= nextTickThres)
		{
			result = true;
			int num = environmentTick - nextTickThres;
			num %= range;
			nextTickThres = environmentTick + (range - num);
		}
		return result;
	}

	private int CalcOffset(int curTime, int offset, int range)
	{
		int num = curTime % range;
		if (num > offset)
		{
			offset = range - (num - offset);
			return offset;
		}
		if (num < offset)
		{
			offset -= num;
			return offset;
		}
		return offset;
	}

	private int GetIntWithinRange(int seed, int range)
	{
		seed = Math.Abs(Noise(seed));
		return seed % range;
	}

	private int Noise(int seed)
	{
		seed = 36969 * (seed & 0xFFFF) + (seed >> 16);
		seed = 18000 * (seed & 0xFFFF) + (seed >> 16);
		return (seed << 16) + seed;
	}
}
