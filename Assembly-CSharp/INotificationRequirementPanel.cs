using UnityEngine;
using UnityEngine.EventSystems;

public interface INotificationRequirementPanel : IEventSystemHandler
{
	void OnToggleEnabled(object text, Sprite checkmarkSprite, bool enabled);
}
