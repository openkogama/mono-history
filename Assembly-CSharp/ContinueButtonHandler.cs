using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinueButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public Action OnClick;

	private bool isMoveOverButton;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && isMoveOverButton)
		{
			Debug.Log("cursor lock pointer down");
			if (OnClick != null)
			{
				Debug.Log("Locking cursor");
				OnClick();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMoveOverButton = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMoveOverButton = false;
	}
}
