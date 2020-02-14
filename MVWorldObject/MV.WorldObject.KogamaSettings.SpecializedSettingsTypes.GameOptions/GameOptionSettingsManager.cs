using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameOptions.GameOptionsPrototypeSettings;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameOptions;

public class GameOptionSettingsManager
{
	private readonly Dictionary<object, object> woData;

	public List<KogamaSettingValueWrapperBase> GetOptions
	{
		get
		{
			KogamaSettingWrapperBase root = KogamaSettingTools.CreatePrototypeWithUserValues(woData, GameOptionsPrototypeManager.CreateOptionSettingPrototypes(), KogamaSettingsFactory.KogamaSettingValueFactory);
			return GetSettingsSettingsList(root);
		}
	}

	public GameOptionSettingsManager(Dictionary<object, object> woData)
	{
		this.woData = woData;
	}

	private static List<KogamaSettingValueWrapperBase> GetSettingsSettingsList(KogamaSettingWrapperBase root)
	{
		List<KogamaSettingValueWrapperBase> inv = new List<KogamaSettingValueWrapperBase>();
		KogamaSettingTools.Traverse(root, (KogamaSettingWrapperBase settingNode) =>
		{
			if (settingNode is KogamaSettingValueWrapperBase)
			{
				inv.Add((KogamaSettingValueWrapperBase)settingNode);
			}
		});
		return inv;
	}
}
