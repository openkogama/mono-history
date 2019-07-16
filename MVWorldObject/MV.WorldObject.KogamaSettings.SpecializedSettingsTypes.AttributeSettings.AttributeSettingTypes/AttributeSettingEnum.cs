using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public class AttributeSettingEnum : KogamaSettingEnumBase, IAttributeSetting
{
	public readonly Dictionary<int, int> valueAttributeValueMap;

	public AttributeSettingsExclusivityFlag ExclusivityFlag { get; private set; }

	public int AttributeValue => valueAttributeValueMap[EnumValue];

	private AttributeSettingEnum(string key, int value, Dictionary<int, int> valueAttributeValueMap, int min, int max, KogamaSettingsCollectionBase parent)
		: base(key, value, min, max, parent)
	{
		this.valueAttributeValueMap = valueAttributeValueMap;
	}

	public AttributeSettingEnum(string key, int value, Dictionary<int, int> valueAttributeValueMap, int min, int max, AttributeSettingsExclusivityFlag attributeSettingsExclusivityFlag, KogamaSettingsCollectionBase parent)
		: this(key, value, valueAttributeValueMap, min, max, parent)
	{
		ExclusivityFlag = attributeSettingsExclusivityFlag;
	}

	public override string ToString()
	{
		string text = "";
		foreach (KeyValuePair<int, int> item in valueAttributeValueMap)
		{
			text += $"[{item.Key}. {item.Value}]";
		}
		return $"AttributeValue {AttributeValue}. AttributePointExchangeRate {text}. ExclusivityFlag {ExclusivityFlag}. {KogamaSettingEnum}";
	}
}
