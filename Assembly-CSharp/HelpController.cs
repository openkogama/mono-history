using UnityEngine;
using UnityEngine.EventSystems;

public class HelpController : MonoBehaviour
{
	[SerializeField]
	private GameObject helpScreen;

	private bool pressed;

	public void OnClick()
	{
		GameObject popup = Object.Instantiate(helpScreen);
		pressed = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, OnClosed, UIGroupFlags.Popup);
		});
	}

	private void Update()
	{
		if (pressed)
		{
			MVInputWrapper.SuppressAllInput();
		}
	}

	private void OnClosed()
	{
		pressed = false;
	}
}
