using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;

public class WorldObjectSkillDataManager
{
	private KogamaSettingsCollectionBase skillData;

	public void Initialize(KogamaSettingWrapperBase settings)
	{
		if (settings != null)
		{
			KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)settings;
			skillData = kogamaSettingsCollectionBase;
		}
	}

	public bool HasSkill(string skillKey)
	{
		if (skillData == null)
		{
			return false;
		}
		return skillData.Children.ContainsKey(skillKey);
	}

	public float GetSkillFloatValue(string skillKey)
	{
		return ((AttributeSettingFloat)skillData.Children[skillKey]).NumericValue;
	}

	public int GetSkillIntValue(string skillKey)
	{
		return ((AttributeSettingInt)skillData.Children[skillKey]).NumericValue;
	}
}
