using System;

namespace MV.WorldObject.AntiCheat;

public class RangeValidator<T> where T : IComparable<T>
{
	public T min;

	public T max;

	public RangeValidator()
	{
	}

	public RangeValidator(T min, T max)
	{
		this.min = min;
		this.max = max;
	}

	public void ValidateRange()
	{
		ref T reference = ref max;
		T other = min;
		if (reference.CompareTo(other) < 0)
		{
			throw new Exception("Min range greater than max range");
		}
	}

	public T Validate(T value, bool fixIfInValid)
	{
		T other = min;
		if (value.CompareTo(other) < 0)
		{
			if (fixIfInValid)
			{
				return min;
			}
			throw new Exception("value <minVal");
		}
		T other2 = max;
		if (value.CompareTo(other2) > 0)
		{
			if (fixIfInValid)
			{
				return max;
			}
			throw new Exception("value > maxVal");
		}
		return value;
	}

	public override string ToString()
	{
		return $"min {min}. max {max}.";
	}
}
