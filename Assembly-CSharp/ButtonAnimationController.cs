using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private const float pressedMovePercentage = 0.14285f;

	private const float hoverMovePercentage = 67f / (273f * (float)Math.PI);

	[SerializeField]
	private Transform transformToMove;

	[SerializeField]
	private RectTransform buttonRectTransform;

	[SerializeField]
	private Button button;

	private float originalValue;

	private short buttonPressedState;

	private short buttonHighlightedState;

	private void Start()
	{
		originalValue = transformToMove.localPosition.y;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (button.interactable || buttonPressedState != 0)
		{
			buttonPressedState--;
			if (buttonPressedState == 0)
			{
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
			}
			if (buttonPressedState == 0 && buttonHighlightedState > 0)
			{
				buttonHighlightedState--;
				OnPointerEnter(null);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (button.interactable)
		{
			buttonHighlightedState++;
			if (buttonPressedState == 0)
			{
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + buttonRectTransform.rect.height * (67f / (273f * (float)Math.PI)), transformToMove.localPosition.z);
				return;
			}
			buttonPressedState--;
			OnPointerDown(null);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (button.interactable || buttonHighlightedState != 0)
		{
			buttonHighlightedState--;
			if (buttonHighlightedState == 0)
			{
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (button.interactable)
		{
			buttonPressedState++;
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue - buttonRectTransform.rect.height * 0.14285f, transformToMove.localPosition.z);
		}
	}

	private void OnDisable()
	{
		buttonPressedState = 0;
		buttonHighlightedState = 0;
		transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
	}
}
