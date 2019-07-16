using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.KogamaSettingsCore;

public static class KogamaSettingsFactory
{
	public static KogamaSettingValueWrapperBase KogamaSettingValueFactory(KeyValuePair<object, object> valuePair, KogamaSettingValueWrapperBase kogamaSettingBasePrototype, KogamaSettingsCollectionBase parent)
	{
		switch (kogamaSettingBasePrototype.KogamaSetting.KogamaSettingValueType)
		{
		case KogamaSettingValueType.Bool:
			return new KogamaSettingBoolBase(kogamaSettingBasePrototype.Key, (bool)valuePair.Value, parent);
		case KogamaSettingValueType.Int:
		{
			KogamaSettingNumericBase<int> kogamaSettingNumericBase2 = (KogamaSettingNumericBase<int>)kogamaSettingBasePrototype;
			return new KogamaSettingNumericBase<int>(kogamaSettingBasePrototype.Key, (int)valuePair.Value, kogamaSettingNumericBase2.KogamaSettingNumeric.RangeValidator.min, kogamaSettingNumericBase2.KogamaSettingNumeric.RangeValidator.max, parent);
		}
		case KogamaSettingValueType.Enum:
		{
			KogamaSettingEnumBase kogamaSettingEnumBase = (KogamaSettingEnumBase)kogamaSettingBasePrototype;
			return new KogamaSettingEnumBase(kogamaSettingEnumBase.Key, (int)valuePair.Value, kogamaSettingEnumBase.KogamaSettingEnum.RangeValidator.min, kogamaSettingEnumBase.KogamaSettingEnum.RangeValidator.max, parent);
		}
		case KogamaSettingValueType.Float:
		{
			KogamaSettingNumericBase<float> kogamaSettingNumericBase = (KogamaSettingNumericBase<float>)kogamaSettingBasePrototype;
			return new KogamaSettingNumericBase<float>(kogamaSettingBasePrototype.Key, (float)valuePair.Value, kogamaSettingNumericBase.KogamaSettingNumeric.RangeValidator.min, kogamaSettingNumericBase.KogamaSettingNumeric.RangeValidator.max, parent);
		}
		default:
			return null;
		}
	}
}
