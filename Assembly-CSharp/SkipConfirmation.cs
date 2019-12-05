using UnityEngine;
using UnityEngine.EventSystems;

public class SkipConfirmation : MonoBehaviour
{
	[SerializeField]
	private PopElement popElement;

	[SerializeField]
	private ConfirmationPopup popup;

	public void CreateConfirmationPopup()
	{
		ConfirmationPopup confirm = Object.Instantiate(popup);
		confirm.Initialize(TM._("You'll miss out on a lot of XP. Do you really want to skip?"), HandleResult, TM._("Skip?"));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(confirm.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void HandleResult(bool confirmed, ConfirmationPopup popup)
	{
		popup.Pop();
		if (confirmed)
		{
			popElement.PopGroups();
		}
	}
}
