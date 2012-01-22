using System;

public class LinearCurve
{
	private LinearCurveKey[] keys;

	public LinearCurve(LinearCurveKey[] keys)
	{
		if (keys.Length == 0)
		{
			throw new Exception("Cannot evaulate piecewise linear curve with no key points.");
		}
		this.keys = keys;
	}

	public float Evaluate(float t)
	{
		for (int i = 1; i < keys.Length; i++)
		{
			LinearCurveKey linearCurveKey = keys[i];
			if (t < linearCurveKey.t)
			{
				LinearCurveKey linearCurveKey2 = keys[i - 1];
				return (linearCurveKey.value - linearCurveKey2.value) * (t - linearCurveKey2.t) / (linearCurveKey.t - linearCurveKey2.t) + linearCurveKey2.value;
			}
		}
		return keys[keys.Length - 1].value;
	}
}
