namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingBoolBase : KogamaSettingValueWrapperBase
{
	public readonly KogamaSettingBool KogamaSettingBool;

	public bool ValueBool
	{
		get
		{
			return KogamaSettingBool.ValueBool;
		}
		set
		{
			KogamaSettingBool.ValueBool = value;
		}
	}

	public override IKogamaSetting KogamaSetting => KogamaSettingBool;

	public KogamaSettingBoolBase(string key, bool value, KogamaSettingsCollectionBase parent)
		: base(key, parent)
	{
		KogamaSettingBool = new KogamaSettingBool(value);
		KogamaSettingBool.OnValueChange += KogamaSettingOnOnValueChange;
	}
}
