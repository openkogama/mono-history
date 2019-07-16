using UnityEngine;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

public struct APIntCalcLinear : IAttributePointIntCalculator
{
	private float exchangeRate;

	public int Calc(int i)
	{
		return Mathf.FloorToInt((float)i * exchangeRate);
	}

	public APIntCalcLinear(float exchangeRate)
	{
		this.exchangeRate = exchangeRate;
	}

	public override string ToString()
	{
		return $"APIntCalcLinear. exchangeRate {exchangeRate}.";
	}
}
