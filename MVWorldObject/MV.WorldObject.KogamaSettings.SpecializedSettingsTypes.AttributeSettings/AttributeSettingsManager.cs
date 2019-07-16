using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.Client;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributePrototypeSettings;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;

public class AttributeSettingsManager
{
	private readonly SettingsManager settingsManager;

	private readonly AttributeSettingWoType attributeSettingWoType;

	private readonly Dictionary<object, object> woData;

	public KogamaSettingWrapperBase AvailableAttributeSettings
	{
		get
		{
			KogamaSettingWrapperBase root = AttributePrototypeSettingsManager.GetRoot(attributeSettingWoType);
			Dictionary<object, object> dictionary = KogamaSettingTools.KogamaSettingsToDictionary(root);
			CommonUtils.PartialRemoveFromHashtable(dictionary, woData, acceptMissingValuesInTarget: true);
			return KogamaSettingTools.CreateFromValues(dictionary, root, AttributeSettingsFactory.KogamaSettingValueFactoryAttributeSettings);
		}
	}

	public KogamaSettingWrapperBase Settings
	{
		get
		{
			KogamaSettingWrapperBase root = AttributePrototypeSettingsManager.GetRoot(attributeSettingWoType);
			return KogamaSettingTools.CreateFromValues(woData, root, AttributeSettingsFactory.KogamaSettingValueFactoryAttributeSettings);
		}
	}

	public AttributeSettingsManager(Dictionary<object, object> data, AttributeSettingWoType attributeSettingWoType, SettingsReporter settingsReporter)
	{
		woData = data;
		this.attributeSettingWoType = attributeSettingWoType;
		settingsManager = new SettingsManager(settingsReporter);
	}

	public void UpdateSetting(KogamaSettingValueWrapperBase attributeSetting)
	{
		settingsManager.UpdateSetting(attributeSetting);
	}

	public void RemoveAvatarSetting(KogamaSettingValueWrapperBase attributeSetting)
	{
		settingsManager.RemoveSetting(attributeSetting);
	}

	public void Submit()
	{
		settingsManager.Submit();
	}
}
