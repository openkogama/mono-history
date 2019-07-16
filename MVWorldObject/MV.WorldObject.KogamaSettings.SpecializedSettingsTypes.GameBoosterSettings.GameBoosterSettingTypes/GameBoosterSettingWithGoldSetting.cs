using System;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;

namespace MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;

public class GameBoosterSettingWithGoldSetting : KogamaSettingsCollectionBase
{
	public const string settingKey = "sk";

	public const string goldSettingKey = "gsk";

	public const string timeSettingKey = "tsk";

	public KogamaSettingNumericBase<int> GoldPrice => (KogamaSettingNumericBase<int>)children["gsk"];

	public KogamaSettingNumericBase<int> BoostTime => (KogamaSettingNumericBase<int>)children["tsk"];

	public KogamaSettingValueWrapperBase Setting => (KogamaSettingValueWrapperBase)children["sk"];

	public GameBoosterSettingWithGoldSetting(string key, KogamaSettingsCollectionBase kogamaSettingsCollection)
		: base(key, kogamaSettingsCollection)
	{
	}

	public override void AddChild(KogamaSettingWrapperBase kogamaSetting)
	{
		if (kogamaSetting.Key != "sk" && kogamaSetting.Key != "gsk" && kogamaSetting.Key != "tsk")
		{
			throw new Exception("Only child with setting key is accepted");
		}
		base.AddChild(kogamaSetting);
	}

	public override KogamaSettingsCollectionBase CopyWithOutChildren(KogamaSettingsCollectionBase parent)
	{
		return new GameBoosterSettingWithGoldSetting(Key, parent);
	}

	public void Validate()
	{
		if (!children.ContainsKey("sk"))
		{
			throw new Exception("Missing settingKey");
		}
		if (!children.ContainsKey("gsk"))
		{
			throw new Exception("Missing goldSettingKey");
		}
		if (!children.ContainsKey("tsk"))
		{
			throw new Exception("Missing timeSettingKey");
		}
		if (children.Count != 3)
		{
			throw new Exception("Unknown keys present in Gold with settings");
		}
		if (Parent.Key != "GameBoosterPrototypeSetting")
		{
			throw new Exception("Invalid root");
		}
		if (Parent.Parent != null)
		{
			throw new Exception("Parent is not root");
		}
	}

	public override string ToString()
	{
		return string.Format("{0}. Setting: {1} GoldSetting: {2} TimeSetting: {3}", base.ToString(), children["sk"], children["gsk"], children["tsk"]);
	}
}
