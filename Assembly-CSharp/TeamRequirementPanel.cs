using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class TeamRequirementPanel : NotificationRequirementPanel
{
	[SerializeField]
	private Image redNotificationIcon;

	[SerializeField]
	private Image greenNotificationIcon;

	[SerializeField]
	private Image blueNotificationIcon;

	[SerializeField]
	private Image yellowNotificationIcon;

	private Image activeNotificationIcon;

	public override void OnToggleEnabled(object team, Sprite checkmarkSprite, bool enabled)
	{
		checkmark.sprite = checkmarkSprite;
		textField.text = team.ToString();
		switch ((MVTeam)(int)team)
		{
		case MVTeam.Red:
			break;
		case MVTeam.Green:
			break;
		case MVTeam.Blue:
			break;
		case MVTeam.Yellow:
			break;
		}
	}
}
