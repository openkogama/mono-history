using UnityEngine;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

public struct APIntCalcZeroValueLinear : IAttributePointIntCalculator
{
	private readonly float exchangeRatePositive;

	private readonly float exchangeRateNegative;

	private int zeroValue;

	public APIntCalcZeroValueLinear(int zeroValue, float exchangeRate)
		: this(zeroValue, exchangeRate, exchangeRate)
	{
	}

	public APIntCalcZeroValueLinear(int zeroValue, float exchangeRatePositive, float exchangeRateNegative)
	{
		this.exchangeRatePositive = exchangeRatePositive;
		this.exchangeRateNegative = exchangeRateNegative;
		this.zeroValue = zeroValue;
	}

	public int Calc(int i)
	{
		int num = i - zeroValue;
		float num2 = (((float)num >= 0f) ? exchangeRatePositive : exchangeRateNegative);
		return Mathf.FloorToInt((float)num * num2);
	}

	public override string ToString()
	{
		return $"APIntCalcZeroValueLinear. exchangeRatePositive {exchangeRatePositive}. exchangeRateNegative {exchangeRateNegative}. zeroValue {zeroValue}.";
	}
}
