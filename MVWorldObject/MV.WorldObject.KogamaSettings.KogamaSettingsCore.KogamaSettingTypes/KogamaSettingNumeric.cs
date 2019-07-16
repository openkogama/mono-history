using System;
using MV.WorldObject.AntiCheat;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingNumeric<T> : KogamaSettingBase where T : IComparable<T>
{
	public readonly RangeValidator<T> RangeValidator;

	public override KogamaSettingValueType KogamaSettingValueType
	{
		get
		{
			if (typeof(T) == typeof(int))
			{
				return KogamaSettingValueType.Int;
			}
			if (typeof(T) == typeof(float))
			{
				return KogamaSettingValueType.Float;
			}
			throw new Exception("Unknown type " + typeof(T));
		}
	}

	public T NumericValue
	{
		get
		{
			return (T)Value;
		}
		set
		{
			T val = RangeValidator.Validate(value, fixIfInValid: true);
			Value = val;
		}
	}

	public KogamaSettingNumeric(T value, T min, T max)
	{
		RangeValidator = new RangeValidator<T>(min, max);
		RangeValidator.Validate(value, fixIfInValid: false);
		NumericValue = value;
	}

	public override string ToString()
	{
		return $"Value {Value}. {RangeValidator}";
	}
}
