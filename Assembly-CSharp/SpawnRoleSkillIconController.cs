using UnityEngine;
using UnityEngine.UI;

public class SpawnRoleSkillIconController : MonoBehaviour
{
	[SerializeField]
	private Image skillIcon;

	[SerializeField]
	private Image negativeIcon;

	[SerializeField]
	private Image negativeBackgroundIcon;

	public void ChangeColor(Color newIconColor, Color newBackgroundColor)
	{
		skillIcon.color = newIconColor;
		negativeIcon.color = newIconColor;
		negativeBackgroundIcon.color = newBackgroundColor;
	}

	public void HandleNegativeState(int skillCost)
	{
		bool active = skillCost < 0;
		negativeIcon.gameObject.SetActive(active);
		negativeBackgroundIcon.gameObject.SetActive(active);
	}

	public void ChangeSize(float width, float height)
	{
		skillIcon.rectTransform.sizeDelta = new Vector2(width, height);
	}
}
