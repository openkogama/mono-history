namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingEnumBase : KogamaSettingValueWrapperBase
{
	public readonly KogamaSettingEnum KogamaSettingEnum;

	public override IKogamaSetting KogamaSetting => KogamaSettingEnum;

	public int EnumValue
	{
		get
		{
			return KogamaSettingEnum.EnumValue;
		}
		set
		{
			KogamaSettingEnum.EnumValue = value;
		}
	}

	public KogamaSettingEnumBase(string key, int value, int min, int max, KogamaSettingsCollectionBase parent)
		: base(key, parent)
	{
		KogamaSettingEnum = new KogamaSettingEnum(value, min, max);
	}
}
