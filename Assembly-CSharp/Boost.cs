using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;

public class Boost
{
	private string description;

	public BoostType Type { get; private set; }

	public object Value
	{
		get
		{
			MVGameOptionDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameOptionDataObject>();
			List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
			activeSettingsList.AddRange(singletonWorldObject.GameBoosterSettingsManager.InactiveGameBoosterSettingsList);
			for (int i = 0; i < activeSettingsList.Count; i++)
			{
				if (BoostKey == activeSettingsList[i].Key)
				{
					return activeSettingsList[i].Setting.KogamaSetting.Value;
				}
			}
			return 0;
		}
	}

	public string BoostKey { get; private set; }

	public string Description
	{
		get
		{
			return string.Format(description, Value);
		}
		private set
		{
			description = value;
		}
	}

	public string ValueDescription { get; private set; }

	public string EditTitle { get; private set; }

	public bool AllowedForGame { get; set; }

	public Boost(BoostType type, string boostKey, string desc, string valueDesc, string title, bool allowedForGame)
	{
		Type = type;
		BoostKey = boostKey;
		Description = desc;
		ValueDescription = valueDesc;
		EditTitle = title;
		AllowedForGame = allowedForGame;
	}
}
