using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRolesSkillDataManager : MonoBehaviour
{
	[Serializable]
	public struct SkillData
	{
		public string type;

		public string name;

		public string description;

		public SpawnRoleSkillIconController iconPrefab;

		public SkillSettingBase settingsPrefab;

		public float zeroValue;
	}

	[SerializeField]
	private List<SkillData> skillDataList;

	[SerializeField]
	private List<string> defenceSkillsList;

	[SerializeField]
	private List<string> offenceSkillsList;

	[SerializeField]
	private List<string> tacticalSkillsList;

	public SpawnRoleSkillIconController GetImageClone(string skillType, ColorStyle iconColorStyle = ColorStyle.Gray, ColorStyle iconBackgroundColorStyle = ColorStyle.OffWhite, float imageWidth = 256f, float imageHeight = 256f)
	{
		for (int i = 0; i < skillDataList.Count; i++)
		{
			if (skillType == skillDataList[i].type)
			{
				SpawnRoleSkillIconController spawnRoleSkillIconController = UnityEngine.Object.Instantiate(skillDataList[i].iconPrefab);
				spawnRoleSkillIconController.ChangeColor(Styles.GetColor(iconColorStyle), Styles.GetColor(iconBackgroundColorStyle));
				spawnRoleSkillIconController.ChangeSize(imageWidth, imageHeight);
				return spawnRoleSkillIconController;
			}
		}
		Debug.LogError("SkillDataManager can not create icon for skill " + skillType + " because it is not in the dataList!");
		return null;
	}

	public string GetNameText(string skillType)
	{
		for (int i = 0; i < skillDataList.Count; i++)
		{
			if (skillType == skillDataList[i].type)
			{
				return skillDataList[i].name;
			}
		}
		Debug.LogError("SkillDataManager can not create name text for skill " + skillType + " because it is not in the dataList!");
		return string.Empty;
	}

	public SkillSettingBase GetSkillsSettingsClone(string skillType)
	{
		for (int i = 0; i < skillDataList.Count; i++)
		{
			if (skillType == skillDataList[i].type)
			{
				return UnityEngine.Object.Instantiate(skillDataList[i].settingsPrefab);
			}
		}
		Debug.LogError("SkillDataManager can not create skill settings prefab for skill " + skillType + " because it is not in the dataList!");
		return null;
	}

	public SkillCategory GetSkillsCategory(string skillKey)
	{
		if (defenceSkillsList.Contains(skillKey))
		{
			return SkillCategory.Defence;
		}
		if (offenceSkillsList.Contains(skillKey))
		{
			return SkillCategory.Offence;
		}
		if (tacticalSkillsList.Contains(skillKey))
		{
			return SkillCategory.Tactical;
		}
		Debug.LogError(skillKey + " is not in any skill category lists in skillDataManager!");
		return SkillCategory.Defence;
	}

	public string GetSkillDescription(string skillType, object settingValue, int skillCost)
	{
		for (int i = 0; i < skillDataList.Count; i++)
		{
			if (skillType == skillDataList[i].type)
			{
				return string.Format(skillDataList[i].description, Styles.ColorToHex(GetValueColor(skillCost)), settingValue);
			}
		}
		Debug.LogError("SkillDataManager can not create skill description for skill " + skillType + " because it is not in the dataList!");
		return string.Empty;
	}

	public float GetZeroValue(string skillType)
	{
		for (int i = 0; i < skillDataList.Count; i++)
		{
			if (skillType == skillDataList[i].type)
			{
				return skillDataList[i].zeroValue;
			}
		}
		Debug.LogError("SkillDataManager can not get skill " + skillType + "s zero value because it is not in the dataList!");
		return 0f;
	}

	public static Color GetCostColor(int skillCost)
	{
		if (skillCost < 0)
		{
			return Styles.GetColor(ColorStyle.NegativeRed);
		}
		return Styles.GetColor(ColorStyle.OffWhite);
	}

	private Color GetValueColor(int skillCost)
	{
		if (skillCost > 0)
		{
			return Styles.GetColor(ColorStyle.PositiveGreen);
		}
		if (skillCost < 0)
		{
			return Styles.GetColor(ColorStyle.NegativeRed);
		}
		return Styles.GetColor(ColorStyle.OffGray);
	}
}
