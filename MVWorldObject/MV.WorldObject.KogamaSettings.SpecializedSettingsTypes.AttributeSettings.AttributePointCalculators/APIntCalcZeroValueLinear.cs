using UnityEngine;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

public struct APIntCalcZeroValueLinear : IAttributePointIntCalculator
{
	private float exchangeRate;

	private int zeroValue;

	public int Calc(int i)
	{
		int num = i - zeroValue;
		return Mathf.FloorToInt((float)num * exchangeRate);
	}

	public APIntCalcZeroValueLinear(float exchangeRate, int zeroValue)
	{
		this.exchangeRate = exchangeRate;
		this.zeroValue = zeroValue;
	}

	public override string ToString()
	{
		return $"APIntCalcZeroValueLinear. exchangeRate {exchangeRate}. zeroValue {zeroValue}.";
	}
}
