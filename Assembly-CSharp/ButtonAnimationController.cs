using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimationController : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IEventSystemHandler
{
	private const float pressedMoveAmount = -25f;

	private const float hoverMoveAmount = 10f;

	private const float disableMoveAmount = -10f;

	[SerializeField]
	private Transform transformToMove;

	[SerializeField]
	private Button button;

	private float originalValue;

	private short buttonPressedState;

	private short buttonHighlightedState;

	private void Start()
	{
		originalValue = transformToMove.localPosition.y;
		HandleButtonDisabled();
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
				transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + 10f, transformToMove.localPosition.z);
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
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + -25f, transformToMove.localPosition.z);
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
			transformToMove.localPosition = new Vector3(transformToMove.localPosition.x, originalValue + -10f, transformToMove.localPosition.z);
		}
	}
}
