using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenuButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private Text text;

	public void Initialize(string buttonText, UnityAction onClickCallback)
	{
		text.text = buttonText;
		button.onClick.AddListener(onClickCallback);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandlePointerDownOnContextMenuButton x, BaseEventData y) =>
		{
			x.PointerIsDown();
		});
	}
}
