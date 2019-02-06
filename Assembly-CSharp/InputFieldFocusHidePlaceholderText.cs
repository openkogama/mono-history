using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldFocusHidePlaceholderText : MonoBehaviour, ISelectHandler, IDeselectHandler, IEventSystemHandler
{
	[SerializeField]
	private Text placeholderText;

	public void OnSelect(BaseEventData data)
	{
		placeholderText.gameObject.SetActive(value: false);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		placeholderText.gameObject.SetActive(value: true);
	}
}
