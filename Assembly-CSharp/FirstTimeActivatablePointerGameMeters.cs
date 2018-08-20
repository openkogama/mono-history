using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePointerGameMeters : FirstTimeActivatableElementBase
{
	[SerializeField]
	private Vector2 pointerBodyDirectionOffset;

	[SerializeField]
	private List<RectTransform> bubbleContent;

	[SerializeField]
	private RectTransform pointToTransform;

	[SerializeField]
	private float bubbleLifetimeWhenVisible = float.MaxValue;

	[SerializeField]
	private float visibleDuration;

	private bool visible;

	private float currentTime;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	private bool isUpdating;

	private int bubbleId = -1;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	private void Update()
	{
		if (!visible)
		{
			return;
		}
		float axis = MVInputWrapper.GetAxis("Mouse X");
		float axis2 = MVInputWrapper.GetAxis("Mouse Y");
		if (axis > 0f || axis2 > 0f)
		{
			isUpdating = true;
		}
		if (isUpdating)
		{
			currentTime += Time.deltaTime;
			if (currentTime >= visibleDuration)
			{
				OnShown();
			}
		}
	}

	public override void OnShow()
	{
		visible = true;
		CreateBubble();
	}

	private void CreateBubble()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhenVisible, bubbleContent, transform);
		});
	}

	private void Clear()
	{
		visible = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesWithId(bubbleId);
		});
	}

	private void OnShown()
	{
		Clear();
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		Destroy();
	}

	private void Destroy()
	{
		Object.Destroy(this);
	}
}
