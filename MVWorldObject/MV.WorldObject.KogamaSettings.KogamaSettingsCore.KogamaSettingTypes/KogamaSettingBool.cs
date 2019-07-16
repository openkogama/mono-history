namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingBool : KogamaSettingBase
{
	public bool ValueBool
	{
		get
		{
			return (bool)Value;
		}
		set
		{
			Value = value;
		}
	}

	public override KogamaSettingValueType KogamaSettingValueType => KogamaSettingValueType.Bool;

	public KogamaSettingBool(bool value)
	{
		ValueBool = value;
	}

	public override string ToString()
	{
		return $"Value {Value}";
	}
}
