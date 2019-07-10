using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CloseApp : MonoBehaviour
{
	public void Close()
	{
		UnityAction<bool, ConfirmationPopup> quit = (bool confirmation, ConfirmationPopup popup) =>
		{
			if (confirmation)
			{
				MVGameControllerBase.ApplicationQuit(null);
			}
			popup.Pop();
		};
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Quit game?"), quit, string.Empty);
		});
	}
}
