using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public class AttributeSettingBool : KogamaSettingBoolBase, IAttributeSetting
{
	public readonly int AttributePointsValue;

	public int AttributeValue
	{
		get
		{
			if (ValueBool)
			{
				return AttributePointsValue;
			}
			return 0;
		}
	}

	public AttributeSettingsExclusivityFlag ExclusivityFlag { get; private set; }

	private AttributeSettingBool(string key, bool value, int attributePointsValue, KogamaSettingsCollectionBase kogamaSettingsCollection)
		: base(key, value, kogamaSettingsCollection)
	{
		AttributePointsValue = attributePointsValue;
	}

	public AttributeSettingBool(string key, bool value, int attributePointsValue, AttributeSettingsExclusivityFlag attributeSettingsExclusivityFlag, KogamaSettingsCollectionBase kogamaSettingsCollection)
		: this(key, value, attributePointsValue, kogamaSettingsCollection)
	{
		ExclusivityFlag = attributeSettingsExclusivityFlag;
	}

	public override string ToString()
	{
		return $"AttributeValue {AttributeValue}. AttributePointsValue {AttributePointsValue}. ExclusivityFlag {ExclusivityFlag}. {KogamaSettingBool}";
	}
}
