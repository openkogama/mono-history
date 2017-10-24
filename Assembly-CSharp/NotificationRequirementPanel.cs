using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotificationRequirementPanel : MonoBehaviour, INotificationRequirementPanel, IEventSystemHandler
{
	[SerializeField]
	protected Image checkmark;

	[SerializeField]
	protected Text textField;

	public virtual void OnToggleEnabled(object text, Sprite checkmarkSprite, bool enabled)
	{
		checkmark.sprite = checkmarkSprite;
		textField.text = text.ToString();
	}
}
