using UnityEngine;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

public struct APFloatCalcLinear : IAttributePointFloatCalculator
{
	private float exchangeRate;

	public int Calc(float f)
	{
		return Mathf.FloorToInt(f * exchangeRate);
	}

	public APFloatCalcLinear(float exchangeRate)
	{
		this.exchangeRate = exchangeRate;
	}

	public override string ToString()
	{
		return $"APFloatCalcLinear. exchangeRate {exchangeRate}.";
	}
}
