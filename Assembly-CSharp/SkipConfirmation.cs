using UnityEngine;
using UnityEngine.EventSystems;

public class SkipConfirmation : MonoBehaviour
{
	[SerializeField]
	private PopElement popElement;

	public void CreateConfirmationPopup()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("You'll miss out on a lot of XP. Do you really want to skip?"), HandleResult, TM._("Skip?"));
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
