using UnityEngine;

public class SpawnRoleSkillInfoButton : MonoBehaviour
{
	[SerializeField]
	private GamePassesTextBubble infoTextBubble;

	private string skillType;

	private SpawnRolesSkillDataManager skillDataManager;

	private object skillValue;

	private int skillCost;

	public void Initialize(string skillType, object skillValue, int skillCost, SpawnRolesSkillDataManager skillDataManager)
	{
		this.skillType = skillType;
		this.skillValue = skillValue;
		this.skillDataManager = skillDataManager;
		this.skillCost = skillCost;
	}

	public void UpdateSkillValue(object skillValue)
	{
		this.skillValue = skillValue;
	}

	public void UpdateSkillCost(int skillCost)
	{
		this.skillCost = skillCost;
	}

	public void ShowInfo()
	{
		infoTextBubble.Activate(skillDataManager.GetSkillDescription(skillType, skillValue, skillCost));
	}
}
