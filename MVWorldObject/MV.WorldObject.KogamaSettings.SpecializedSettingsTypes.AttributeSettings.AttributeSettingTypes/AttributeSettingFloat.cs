using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public class AttributeSettingFloat : KogamaSettingNumericBase<float>, IAttributeSetting
{
	public readonly IAttributePointFloatCalculator Calculator;

	public int AttributeValue => Calculator.Calc(KogamaSettingNumeric.NumericValue);

	public AttributeSettingsExclusivityFlag ExclusivityFlag { get; private set; }

	private AttributeSettingFloat(string key, float value, float min, float max, IAttributePointFloatCalculator calculator, KogamaSettingsCollectionBase parent)
		: base(key, value, min, max, parent)
	{
		Calculator = calculator;
	}

	public AttributeSettingFloat(string key, float value, float min, float max, IAttributePointFloatCalculator calculator, AttributeSettingsExclusivityFlag attributeSettingsExclusivityFlag, KogamaSettingsCollectionBase parent)
		: this(key, value, min, max, calculator, parent)
	{
		ExclusivityFlag = attributeSettingsExclusivityFlag;
	}

	public override string ToString()
	{
		return $"AttributeValue {AttributeValue}. AttributePointExchangeRate {Calculator} ExclusivityFlag {ExclusivityFlag}.  {KogamaSettingNumeric}";
	}
}
