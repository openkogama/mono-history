using MV.WorldObject.KogamaSettings.KogamaSettingsCore;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public interface IAttributeSetting
{
	int AttributeValue { get; }

	AttributeSettingsExclusivityFlag ExclusivityFlag { get; }

	IKogamaSetting KogamaSetting { get; }

	string Key { get; }
}
