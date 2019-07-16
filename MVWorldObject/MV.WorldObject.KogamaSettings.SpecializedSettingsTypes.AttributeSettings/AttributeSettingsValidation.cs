using System;
using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;

public static class AttributeSettingsValidation
{
	public static bool ValidateUpdate(KogamaSettingWrapperBase rootDelta, KogamaSettingWrapperBase rootDestination)
	{
		AttributeSettingsExclusivityFlag destinationExclusivityFlag = AttributeSettingsExclusivityFlag.None;
		KogamaSettingTools.Traverse(rootDestination, (KogamaSettingWrapperBase baseWrapper) =>
		{
			if (baseWrapper is IAttributeSetting)
			{
				IAttributeSetting attributeSetting = (IAttributeSetting)baseWrapper;
				destinationExclusivityFlag |= attributeSetting.ExclusivityFlag;
			}
		});
		return ValidateUpdateRecursion(rootDelta, rootDestination, destinationExclusivityFlag);
	}

	private static bool ValidateUpdateRecursion(KogamaSettingWrapperBase rootDelta, KogamaSettingWrapperBase rootDestination, AttributeSettingsExclusivityFlag destinationExclusivityFlag)
	{
		if (rootDestination == null)
		{
			return true;
		}
		if (rootDelta == null)
		{
			throw new Exception("delta root can not be null");
		}
		if (rootDelta is KogamaSettingsCollectionBase)
		{
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)rootDelta;
			foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
			{
				KogamaSettingWrapperBase value = child.Value;
				if (value is KogamaSettingsCollectionBase)
				{
					if (!ValidateUpdateRecursion(value, rootDestination, destinationExclusivityFlag))
					{
						return false;
					}
				}
				else if (!ValidateDeltaChild((IAttributeSetting)value, rootDestination, destinationExclusivityFlag))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool ValidateDeltaChild(IAttributeSetting rootDeltaChild, KogamaSettingWrapperBase rootDestination, AttributeSettingsExclusivityFlag destinationExclusivityFlag)
	{
		bool flag = KogamaSettingTools.RootDestinationContains((KogamaSettingWrapperBase)rootDeltaChild, rootDestination);
		bool flag2 = (destinationExclusivityFlag & rootDeltaChild.ExclusivityFlag) != 0;
		if (!flag && flag2)
		{
			return false;
		}
		return true;
	}
}
