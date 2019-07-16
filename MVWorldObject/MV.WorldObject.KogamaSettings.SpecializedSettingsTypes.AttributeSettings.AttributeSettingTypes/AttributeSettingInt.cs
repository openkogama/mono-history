using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePointCalculators;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public class AttributeSettingInt : KogamaSettingNumericBase<int>, IAttributeSetting
{
	public readonly IAttributePointIntCalculator Calculator;

	public int AttributeValue => Calculator.Calc(KogamaSettingNumeric.NumericValue);

	public AttributeSettingsExclusivityFlag ExclusivityFlag { get; private set; }

	private AttributeSettingInt(string key, int value, int min, int max, IAttributePointIntCalculator calculator, KogamaSettingsCollectionBase parent)
		: base(key, value, min, max, parent)
	{
		Calculator = calculator;
	}

	public AttributeSettingInt(string key, int value, int min, int max, IAttributePointIntCalculator calculator, AttributeSettingsExclusivityFlag attributeSettingsExclusivityFlag, KogamaSettingsCollectionBase parent)
		: this(key, value, min, max, calculator, parent)
	{
		ExclusivityFlag = attributeSettingsExclusivityFlag;
	}

	public override string ToString()
	{
		return $"AttributeValue {AttributeValue}. AttributePointExchangeRate {Calculator}. ExclusivityFlag {ExclusivityFlag}.  {KogamaSettingNumeric}";
	}
}
