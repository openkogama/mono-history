using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private const float pressedMovePercentage = 0.14285f;

	private const float hoverMovePercentage = 67f / (273f * (float)Math.PI);

	[SerializeField]
	private Transform transformToMove;

	[SerializeField]
	private RectTransform buttonRectTransform;

	private float originalValue;

	private short buttonPressedState;

	private short buttonHighlightedState;

	private void Start()
	{
		originalValue = transformToMove.localPosition.y;
	}

	public void OnPointerUp(PointerEventData eventData)
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

	public void OnPointerEnter(PointerEventData eventData)
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

	public void OnPointerExit(PointerEventData eventData)
	{
		buttonHighlightedState--;
		if (buttonHighlightedState == 0)
		{
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		buttonPressedState++;
		transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue - buttonRectTransform.rect.height * 0.14285f, transformToMove.localPosition.z);
	}
}
