using UnityEngine;
using UnityEngine.EventSystems;

public class PopupSlideshowCreator : MonoBehaviour
{
	[SerializeField]
	private PopupSlideshowController popupSlideshow;

	public void OnClick()
	{
		PopupSlideshowController slideshow = Object.Instantiate(popupSlideshow);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(slideshow.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}
}
