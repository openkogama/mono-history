using UnityEngine;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

public struct APFloatCalcZeroValueLinear : IAttributePointFloatCalculator
{
	private readonly float exchangeRatePositive;

	private readonly float exchangeRateNegative;

	private readonly float zeroValue;

	public APFloatCalcZeroValueLinear(float zeroValue, float exchangeRate)
		: this(zeroValue, exchangeRate, exchangeRate)
	{
	}

	public APFloatCalcZeroValueLinear(float zeroValue, float exchangeRatePositive, float exchangeRateNegative)
	{
		this.exchangeRatePositive = exchangeRatePositive;
		this.exchangeRateNegative = exchangeRateNegative;
		this.zeroValue = zeroValue;
	}

	public int Calc(float i)
	{
		float num = i - zeroValue;
		float num2 = ((num >= 0f) ? exchangeRatePositive : exchangeRateNegative);
		return Mathf.RoundToInt(num * num2);
	}

	public override string ToString()
	{
		return $"APFloatCalcZeroValueLinear. exchangeRatePositive {exchangeRatePositive}. exchangeRateNegative {exchangeRateNegative}. zeroValue {zeroValue}.";
	}
}
