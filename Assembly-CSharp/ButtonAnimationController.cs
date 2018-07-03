using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour
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

	public void OnButtonPressed()
	{
		buttonPressedState++;
		transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue - buttonRectTransform.rect.height * 0.14285f, transformToMove.localPosition.z);
	}

	public void OnButtonUnPressed()
	{
		buttonPressedState--;
		if (buttonPressedState == 0)
		{
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
		}
		if (buttonPressedState == 0 && buttonHighlightedState > 0)
		{
			buttonHighlightedState--;
			OnButtonMouseIn();
		}
	}

	public void OnButtonMouseIn()
	{
		buttonHighlightedState++;
		if (buttonPressedState == 0)
		{
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + buttonRectTransform.rect.height * (67f / (273f * (float)Math.PI)), transformToMove.localPosition.z);
			return;
		}
		buttonPressedState--;
		OnButtonPressed();
	}

	public void OnButtonMouseOut()
	{
		buttonHighlightedState--;
		if (buttonHighlightedState == 0)
		{
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
		}
	}
}
