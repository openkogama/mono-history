using UnityEngine;
using UnityEngine.EventSystems;

public class LoadErrorController : MonoBehaviour
{
	[SerializeField]
	private GotoMarketPlacePopup gototMarketPlacePopup;

	private void Start()
	{
		GotoMarketPlacePopup popup = Object.Instantiate(gototMarketPlacePopup);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void OnDone()
	{
	}
}
