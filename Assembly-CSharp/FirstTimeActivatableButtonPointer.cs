using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatableButtonPointer : FirstTimeActivatableElementBase
{
	protected int bubbleId = -1;

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
	protected Button skipElement;

	[SerializeField]
	protected bool skipAllowed = true;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	public override void OnShow()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
		ShowBubble();
	}

	private void CreateBubble()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
			if (skipAllowed)
			{
				Button button = Object.Instantiate(skipElement);
				button.onClick.AddListener(SkipEvent);
				x.AddFirstElement(bubbleId, (RectTransform)button.transform);
			}
		});
	}

	protected void ShowBubble()
	{
		button.onClick.AddListener(OnShown);
		CreateBubble();
	}

	private void Clear()
	{
		button.onClick.RemoveListener(OnShown);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesWithId(bubbleId);
		});
	}

	protected virtual void OnShown()
	{
		Clear();
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		Object.Destroy(this);
	}
}
