using UnityEngine;
using UnityEngine.EventSystems;

public class CloseApp : MonoBehaviour
{
	public void Close()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Quit game?"), Callback, string.Empty);
		});
	}

	private void Callback(bool wantToQuit, ConfirmationPopup confirmationPopup)
	{
		if (wantToQuit)
		{
			MVGameControllerBase.ApplicationQuit(null);
		}
		confirmationPopup.Pop();
	}
}
