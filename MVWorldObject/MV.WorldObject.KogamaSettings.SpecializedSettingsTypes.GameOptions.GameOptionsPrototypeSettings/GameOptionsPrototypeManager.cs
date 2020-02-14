using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameOptions.GameOptionsPrototypeSettings;

public static class GameOptionsPrototypeManager
{
	public const string root = "GameOptionsRoot";

	public static KogamaSettingWrapperBase CreateOptionSettingPrototypes()
	{
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = new KogamaSettingsCollectionBase("GameOptionsRoot", null);
		KogamaSettingBoolBase kogamaSetting = new KogamaSettingBoolBase("AllowRevive", value: true, kogamaSettingsCollectionBase);
		kogamaSettingsCollectionBase.AddChild(kogamaSetting);
		return kogamaSettingsCollectionBase;
	}
}
