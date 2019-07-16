using System;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

[Flags]
public enum AttributeSettingsExclusivityFlag
{
	None = 0,
	Dodge = 1,
	Jump = 2,
	Run = 4
}
