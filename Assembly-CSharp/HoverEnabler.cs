using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEnabler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private GameObject objectToEnable;

	[SerializeField]
	private GameObject objectToDisable;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (objectToEnable != null && !objectToEnable.activeSelf)
		{
			objectToEnable.SetActive(value: true);
		}
		if (objectToDisable != null && objectToDisable.activeSelf)
		{
			objectToDisable.SetActive(value: false);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (objectToEnable != null && objectToEnable.activeSelf)
		{
			objectToEnable.SetActive(value: false);
		}
		if (objectToDisable != null && !objectToDisable.activeSelf)
		{
			objectToDisable.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if (objectToEnable != null && objectToEnable.activeSelf)
		{
			objectToEnable.SetActive(value: false);
		}
		if (objectToDisable != null && !objectToDisable.activeSelf)
		{
			objectToDisable.SetActive(value: true);
		}
	}
}
