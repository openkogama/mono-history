using MV.WorldObject.AntiCheat;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingEnum : KogamaSettingBase
{
	public readonly RangeValidator<int> RangeValidator;

	public override KogamaSettingValueType KogamaSettingValueType => KogamaSettingValueType.Enum;

	public int EnumValue
	{
		get
		{
			return (int)Value;
		}
		set
		{
			int num = RangeValidator.Validate(value, fixIfInValid: true);
			Value = num;
		}
	}

	public KogamaSettingEnum(int value, int min, int max)
	{
		RangeValidator = new RangeValidator<int>(min, max);
		RangeValidator.Validate(value, fixIfInValid: false);
		EnumValue = value;
	}

	public override string ToString()
	{
		return $"Value {Value}. {RangeValidator}";
	}
}
