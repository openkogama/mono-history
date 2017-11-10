using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPointerController : MonoBehaviour
{
	private int bubbleId;

	[SerializeField]
	protected Button button;

	[SerializeField]
	protected RectTransform pointToTransform;

	[SerializeField]
	protected Vector2 pointerBodyDirectionOffset;

	[SerializeField]
	protected List<RectTransform> bubbleContent;

	[SerializeField]
	protected float bubbleLifetimeWhileShown = float.MaxValue;

	[SerializeField]
	private Button openButton;

	private int slotToHighlight;

	private int categoryToOpen;

	private Button open;

	private void Start()
	{
		PlayerInventoryRepository playerInventoryRepository = MVGameControllerBase.IEditModeUI.PlayerInventoryRepository;
		playerInventoryRepository.OnInventoryItemAdded = (Action<int, int>)Delegate.Combine(playerInventoryRepository.OnInventoryItemAdded, new Action<int, int>(CreateBubble));
	}

	public void CreateBubble(int category, int slot)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopToGroup(UIGroupFlags.MainUI);
		});
		ClearImmediate();
		slotToHighlight = slot;
		categoryToOpen = category;
		button.onClick.AddListener(RemoveBubbles);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
			open = UnityEngine.Object.Instantiate(openButton);
			open.onClick.AddListener(OpenAtSlot);
			x.AddElement(bubbleId, (RectTransform)open.transform);
		});
	}

	private void RemoveBubbles()
	{
		ClearImmediate();
	}

	private void OpenAtSlot()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.ActivateAtCategoryWithSlot(UIPushOption.Blocking, categoryToOpen, slotToHighlight);
		});
		ClearImmediate();
	}

	private void ClearImmediate()
	{
		button.onClick.RemoveListener(RemoveBubbles);
		if (open != null)
		{
			open.onClick.RemoveListener(OpenAtSlot);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesOfTypeImmediately(bubbleId);
		});
	}
}
