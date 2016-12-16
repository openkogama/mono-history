using UnityEngine;
using UnityEngine.EventSystems;

public class HelpController : MonoBehaviour
{
	[SerializeField]
	private GameObject helpScreen;

	public void OnClick()
	{
		GameObject popup = Object.Instantiate(helpScreen);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}
}
