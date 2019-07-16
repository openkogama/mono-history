using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterPrototypeSettings;

public static class GameBoosterPrototypeSettingsManager
{
	public const int defaultMinGold = 5;

	public const int defaultMaxGold = 50;

	public const int defaultValGold = 10;

	public const int defaultMinTimeSeconds = 600;

	public const int defaultMaxTimeSeconds = 600;

	public const int defaultValTimeSeconds = 600;

	public const string root = "GameBoosterPrototypeSetting";

	public static KogamaSettingWrapperBase CreateGameBoosterSettingPrototypes()
	{
		KogamaSettingsCollectionBase result = new KogamaSettingsCollectionBase("GameBoosterPrototypeSetting", null);
		CreateDefaultFloat(2f, 1.2f, 3f, "JumpPower", result);
		CreateDefaultInt(10, 5, 25, "Speed", result);
		CreateDefaultInt(50, 25, 100, "Health", result);
		CreateDefaultBool("GameCoinBoost", result);
		CreateDefaultBool("Ammo", result);
		return result;
	}

	public static Dictionary<object, object> GetDefaultSettingsSubSet(List<string> defaultSettingsSubset)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		Dictionary<string, GameBoosterSettingWithGoldSetting> settingsDictionary = GetSettingsDictionary(CreateGameBoosterSettingPrototypes());
		foreach (string item in defaultSettingsSubset)
		{
			GameBoosterSettingWithGoldSetting obj = settingsDictionary[item];
			Dictionary<object, object> subTree = KogamaSettingTools.GetSubTree(obj);
			CommonUtils.PartialUpdateHashtable(dictionary, subTree);
		}
		return dictionary;
	}

	public static List<GameBoosterSettingWithGoldSetting> GetSettingsSettingsList(KogamaSettingWrapperBase root)
	{
		List<GameBoosterSettingWithGoldSetting> inv = new List<GameBoosterSettingWithGoldSetting>();
		KogamaSettingTools.Traverse(root, (KogamaSettingWrapperBase settingNode) =>
		{
			if (settingNode is GameBoosterSettingWithGoldSetting)
			{
				inv.Add((GameBoosterSettingWithGoldSetting)settingNode);
			}
		});
		return inv;
	}

	public static Dictionary<string, GameBoosterSettingWithGoldSetting> GetSettingsDictionary(KogamaSettingWrapperBase root)
	{
		Dictionary<string, GameBoosterSettingWithGoldSetting> inv = new Dictionary<string, GameBoosterSettingWithGoldSetting>();
		KogamaSettingTools.Traverse(root, (KogamaSettingWrapperBase settingNode) =>
		{
			if (settingNode is GameBoosterSettingWithGoldSetting)
			{
				inv.Add(settingNode.Key, (GameBoosterSettingWithGoldSetting)settingNode);
			}
		});
		return inv;
	}

	public static GameBoosterSettingWithGoldSetting CreateDefaultBool(string key, KogamaSettingsCollectionBase root)
	{
		GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting = CreateDefaultGameBoosterSettingWithGoldSetting(key, root);
		KogamaSettingBoolBase kogamaSetting = new KogamaSettingBoolBase("sk", value: true, gameBoosterSettingWithGoldSetting);
		gameBoosterSettingWithGoldSetting.AddChild(kogamaSetting);
		return gameBoosterSettingWithGoldSetting;
	}

	public static GameBoosterSettingWithGoldSetting CreateDefaultInt(int defaultVal, int minVal, int maxVal, string key, KogamaSettingsCollectionBase root)
	{
		GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting = CreateDefaultGameBoosterSettingWithGoldSetting(key, root);
		KogamaSettingNumericBase<int> kogamaSetting = new KogamaSettingNumericBase<int>("sk", defaultVal, minVal, maxVal, gameBoosterSettingWithGoldSetting);
		gameBoosterSettingWithGoldSetting.AddChild(kogamaSetting);
		return gameBoosterSettingWithGoldSetting;
	}

	public static GameBoosterSettingWithGoldSetting CreateDefaultFloat(float defaultVal, float minVal, float maxVal, string key, KogamaSettingsCollectionBase root)
	{
		GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting = CreateDefaultGameBoosterSettingWithGoldSetting(key, root);
		KogamaSettingNumericBase<float> kogamaSetting = new KogamaSettingNumericBase<float>("sk", defaultVal, minVal, maxVal, gameBoosterSettingWithGoldSetting);
		gameBoosterSettingWithGoldSetting.AddChild(kogamaSetting);
		return gameBoosterSettingWithGoldSetting;
	}

	private static GameBoosterSettingWithGoldSetting CreateDefaultGameBoosterSettingWithGoldSetting(string key, KogamaSettingsCollectionBase root)
	{
		GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting = new GameBoosterSettingWithGoldSetting(key, root);
		gameBoosterSettingWithGoldSetting.AddChild(CreateGoldSetting(gameBoosterSettingWithGoldSetting));
		gameBoosterSettingWithGoldSetting.AddChild(CreateTimeSetting(gameBoosterSettingWithGoldSetting));
		root.AddChild(gameBoosterSettingWithGoldSetting);
		return gameBoosterSettingWithGoldSetting;
	}

	private static KogamaSettingNumericBase<int> CreateGoldSetting(GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting)
	{
		return new KogamaSettingNumericBase<int>("gsk", 10, 5, 50, gameBoosterSettingWithGoldSetting);
	}

	private static KogamaSettingNumericBase<int> CreateTimeSetting(GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting)
	{
		return new KogamaSettingNumericBase<int>("tsk", 600, 600, 600, gameBoosterSettingWithGoldSetting);
	}
}
