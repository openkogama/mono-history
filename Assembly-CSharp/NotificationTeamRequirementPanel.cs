using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class NotificationTeamRequirementPanel : NotificationRequirementPanel
{
	[SerializeField]
	private Image requirementImage;

	[SerializeField]
	private Sprite redNotificationIcon;

	[SerializeField]
	private Sprite greenNotificationIcon;

	[SerializeField]
	private Sprite blueNotificationIcon;

	[SerializeField]
	private Sprite yellowNotificationIcon;

	public override void OnToggleEnabled(object team, Sprite checkmarkSprite, bool enabled)
	{
		checkmark.sprite = checkmarkSprite;
		textField.text = team.ToString();
		switch ((MVTeam)(int)team)
		{
		case MVTeam.Red:
			requirementImage.sprite = redNotificationIcon;
			break;
		case MVTeam.Green:
			requirementImage.sprite = greenNotificationIcon;
			break;
		case MVTeam.Blue:
			requirementImage.sprite = blueNotificationIcon;
			break;
		case MVTeam.Yellow:
			requirementImage.sprite = yellowNotificationIcon;
			break;
		}
	}
}
