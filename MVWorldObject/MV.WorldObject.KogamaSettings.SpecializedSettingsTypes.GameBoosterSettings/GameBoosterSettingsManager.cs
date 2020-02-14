using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterPrototypeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings;

public class GameBoosterSettingsManager
{
	private readonly Dictionary<object, object> woData;

	public List<GameBoosterSettingWithGoldSetting> InactiveGameBoosterSettingsList => GameBoosterPrototypeSettingsManager.GetSettingsSettingsList(InactiveGameBoosterSettings);

	public KogamaSettingWrapperBase InactiveGameBoosterSettings
	{
		get
		{
			KogamaSettingWrapperBase kogamaSettingWrapperBase = GameBoosterPrototypeSettingsManager.CreateGameBoosterSettingPrototypes();
			Dictionary<object, object> dictionary = KogamaSettingTools.KogamaSettingsToDictionary(kogamaSettingWrapperBase);
			CommonUtils.PartialRemoveFromHashtable(dictionary, woData, acceptMissingValuesInTarget: true);
			CommonUtils.PruneEmptyDictionaries(dictionary);
			return KogamaSettingTools.CreateFromValues(dictionary, kogamaSettingWrapperBase, KogamaSettingsFactory.KogamaSettingValueFactory);
		}
	}

	public List<GameBoosterSettingWithGoldSetting> ActiveSettingsList => GameBoosterPrototypeSettingsManager.GetSettingsSettingsList(ActiveSettings);

	public KogamaSettingWrapperBase ActiveSettings
	{
		get
		{
			KogamaSettingWrapperBase prototypeRoot = GameBoosterPrototypeSettingsManager.CreateGameBoosterSettingPrototypes();
			return KogamaSettingTools.CreateFromValues(woData, prototypeRoot, KogamaSettingsFactory.KogamaSettingValueFactory);
		}
	}

	public GameBoosterSettingsManager(Dictionary<object, object> data)
	{
		woData = data;
	}
}
