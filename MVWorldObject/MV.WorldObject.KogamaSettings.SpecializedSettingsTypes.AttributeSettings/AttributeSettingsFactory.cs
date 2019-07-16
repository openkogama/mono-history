using System;
using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;

public static class AttributeSettingsFactory
{
	public static KogamaSettingValueWrapperBase KogamaSettingValueFactoryAttributeSettings(KeyValuePair<object, object> valuePair, KogamaSettingValueWrapperBase kogamaSettingBasePrototype, KogamaSettingsCollectionBase parent)
	{
		switch (kogamaSettingBasePrototype.KogamaSetting.KogamaSettingValueType)
		{
		case KogamaSettingValueType.Bool:
		{
			AttributeSettingBool attributeSettingBool = (AttributeSettingBool)kogamaSettingBasePrototype;
			return new AttributeSettingBool(kogamaSettingBasePrototype.Key, (bool)valuePair.Value, attributeSettingBool.AttributePointsValue, attributeSettingBool.ExclusivityFlag, parent);
		}
		case KogamaSettingValueType.Int:
		{
			AttributeSettingInt attributeSettingInt = (AttributeSettingInt)kogamaSettingBasePrototype;
			return new AttributeSettingInt(kogamaSettingBasePrototype.Key, (int)valuePair.Value, attributeSettingInt.KogamaSettingNumeric.RangeValidator.min, attributeSettingInt.KogamaSettingNumeric.RangeValidator.max, attributeSettingInt.Calculator, attributeSettingInt.ExclusivityFlag, parent);
		}
		case KogamaSettingValueType.Enum:
		{
			AttributeSettingEnum attributeSettingEnum = (AttributeSettingEnum)kogamaSettingBasePrototype;
			return new AttributeSettingEnum(attributeSettingEnum.Key, (int)valuePair.Value, attributeSettingEnum.valueAttributeValueMap, attributeSettingEnum.KogamaSettingEnum.RangeValidator.min, attributeSettingEnum.KogamaSettingEnum.RangeValidator.max, attributeSettingEnum.ExclusivityFlag, parent);
		}
		case KogamaSettingValueType.Float:
		{
			AttributeSettingFloat attributeSettingFloat = (AttributeSettingFloat)kogamaSettingBasePrototype;
			return new AttributeSettingFloat(attributeSettingFloat.Key, (float)valuePair.Value, attributeSettingFloat.KogamaSettingNumeric.RangeValidator.min, attributeSettingFloat.KogamaSettingNumeric.RangeValidator.max, attributeSettingFloat.Calculator, attributeSettingFloat.ExclusivityFlag, parent);
		}
		default:
			throw new NotImplementedException();
		}
	}
}
