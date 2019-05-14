using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesTouristInformationPopup : MonoBehaviour
{
	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void OnSignupClicked()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}
}
