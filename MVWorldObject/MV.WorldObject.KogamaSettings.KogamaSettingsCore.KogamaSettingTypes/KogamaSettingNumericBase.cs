using System;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

public class KogamaSettingNumericBase<T> : KogamaSettingValueWrapperBase where T : IComparable<T>
{
	public readonly KogamaSettingNumeric<T> KogamaSettingNumeric;

	public override IKogamaSetting KogamaSetting => KogamaSettingNumeric;

	public T NumericValue
	{
		get
		{
			return KogamaSettingNumeric.NumericValue;
		}
		set
		{
			KogamaSettingNumeric.NumericValue = value;
		}
	}

	public KogamaSettingNumericBase(string key, T value, T min, T max, KogamaSettingsCollectionBase parent)
		: base(key, parent)
	{
		KogamaSettingNumeric = new KogamaSettingNumeric<T>(value, min, max);
	}
}
