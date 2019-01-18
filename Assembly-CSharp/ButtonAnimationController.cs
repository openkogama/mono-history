using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IEventSystemHandler
{
	private enum ButtonType : byte
	{
		Square,
		Small
	}

	private const float pressedSquareMoveAmount = -25f;

	private const float hoverSquareMoveAmount = 10f;

	private const float disableSquareMoveAmount = -10f;

	private const float pressedSmallMoveAmount = -6f;

	private const float hoverSmallMoveAmount = 5f;

	private const float disableSmallMoveAmount = -1f;

	private float pressedMoveAmount = -25f;

	private float hoverMoveAmount = 10f;

	private float disableMoveAmount = -10f;

	[SerializeField]
	private Transform transformToMove;

	[SerializeField]
	private Button button;

	[SerializeField]
	private ButtonType buttonType;

	private float originalValue;

	private short buttonPressedState;

	private short buttonHighlightedState;

	private void Start()
	{
		originalValue = transformToMove.localPosition.y;
		HandleButtonDisabled();
		SetMoveAmount();
	}

	private void Update()
	{
		if (!button.IsInteractable())
		{
			HandleButtonDisabled();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && (button.interactable || buttonPressedState != 0))
		{
			buttonPressedState--;
			if (buttonPressedState == 0)
			{
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
			}
			if (buttonPressedState == 0 && buttonHighlightedState > 0)
			{
				buttonHighlightedState--;
				OnPointerEnter(eventData);
			}
			HandleButtonDisabled();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (button.interactable)
		{
			buttonHighlightedState++;
			if (buttonPressedState == 0)
			{
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + hoverMoveAmount, transformToMove.localPosition.z);
			}
			else
			{
				buttonPressedState--;
				OnPointerDown(eventData);
			}
			HandleButtonDisabled();
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
			HandleButtonDisabled();
			button.OnDeselect(eventData);
			button.enabled = false;
			button.enabled = true;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && button.interactable)
		{
			buttonPressedState++;
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + pressedMoveAmount, transformToMove.localPosition.z);
			HandleButtonDisabled();
		}
	}

	private void OnDisable()
	{
		buttonPressedState = 0;
		buttonHighlightedState = 0;
		transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue, transformToMove.localPosition.z);
		HandleButtonDisabled();
	}

	private void HandleButtonDisabled()
	{
		if (!button.interactable)
		{
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + disableMoveAmount, transformToMove.localPosition.z);
		}
	}

	private void SetMoveAmount()
	{
		switch (buttonType)
		{
		case ButtonType.Square:
			pressedMoveAmount = -25f;
			hoverMoveAmount = 10f;
			disableMoveAmount = -10f;
			break;
		case ButtonType.Small:
			pressedMoveAmount = -6f;
			hoverMoveAmount = 5f;
			disableMoveAmount = -1f;
			break;
		}
	}
}
